import pickle as p

import networkx as nx

import items as it
import people as pe
import places as pl
import random as r
import times as t

world = 5

def buildWorld(numCities, infestation):     #todo alternate worlds?
    global world

    capidx = 0
    it.createItem(0)

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,1)

    web = t.createCalendar(web)

    #finds city with most roads out, names it the capital
    for i in web.nodes:
        if len(web.edges(i)) > len(web.edges(capidx)):
            capidx = i

    #construct and describe roads
    with open('roaddesc.txt') as f:
        lines = f.read().splitlines()
    
    for (u,v,w) in web.edges(data=True):
        temp = r.sample(range(len(lines)), 2)
        
        w['description'] = "a " + str(lines[temp[0]]) + ", " + str(lines[temp[1]]) + " road"
        w['length'] = r.randrange(2, 10)
        w['known'] = 0

    #give each node sites and people
    for x in range(len(web.nodes)):
        web.nodes[x]['label'] = int(x)
        web.nodes[x]['sites'] = [pl.createPlace(1, "Bakery"),
                                 pl.createPlace(2, "Weapon Shop"),
                                 pl.createPlace(3, "Armory"),
                                 pl.createPlace(5, "Tavern & Inn")]

                                #todo food sites generate food supply

                                #todo every site has inventory and wants and will buy from neighboring nodes for those goods

        numKrog = r.randrange(infestation + 3, 2 * (infestation + 4))
        web.nodes[x]['monsters'] = pe.createPerson(1,
                                                   numKrog)  # todo add savagery attr to nodes which affect number and strength of krogs
        # todo randomize krog growth times

    web.nodes[r.randrange(len(web.nodes))]['sites'].append(
        pl.createPlace(4, "Druid Circle"))  # create Druid Circle in a random node
    web.nodes[r.randrange(len(web.nodes))]['monsters'].append(pl.createPerson(4))  # create Krog Hill in a random node

    web.graph['instability'] = 0
    web.graph['capital'] = capidx

    return web


def worldInfo():
    print("The capital city is " + str(world.graph['capital']))


def saveWorld():
    global world

    nx.write_gml(world, r'world/world.kr')

    with open(r"world/items.kr", "wb") as pit:
        p.dump(it.items, pit)
    pit.close()
    with open(r"world/places.kr", "wb") as ppl:
        p.dump(pl.places, ppl)
    ppl.close()
    with open(r"world/persons.kr", "wb") as ppe:
        p.dump(pe.persons, ppe)
    ppe.close()
    with open(r"world/history.kr", "wb") as phi:
        p.dump(t.history, phi)
    phi.close()
    with open(r"world/obituary.kr", "wb") as obi:
        p.dump(pe.futureDead, obi)
    obi.close()
    t.printHistory()


def loadWorld():
    global world

    world = nx.read_gml(r'world/world.kr')
    world = nx.convert_node_labels_to_integers(world)

    with open(r"world/items.kr", "rb") as pit:
        it.items = p.load(pit)
    pit.close()
    with open(r"world/places.kr", "rb") as ppl:
        pl.places = p.load(ppl)
    ppl.close()
    with open(r"world/persons.kr", "rb") as ppe:
        pe.persons = p.load(ppe)
    ppe.close()
    with open(r"world/history.kr", "rb") as phi:
        t.history = p.load(phi)
    phi.close()
    with open(r"world/obituary.kr", "rb") as obi:
        pe.futureDead = p.load(obi)
    obi.close()


def resetWorld():  # todo I think dead monster inventory loot resets
    global world

    world = nx.read_gml('world/worldStart.kr')
    world = nx.convert_node_labels_to_integers(world)

    with open(r"world/itemsStart.kr", "rb") as pit:
        it.items = p.load(pit)
    pit.close()
    with open(r"world/placesStart.kr", "rb") as ppl:
        pl.places = p.load(ppl)
    ppl.close()
    with open(r"world/personsStart.kr", "rb") as ppe:
        pe.persons = p.load(ppe)
    ppe.close()
    with open(r"world/history.kr", "rb") as phi:
        t.history = p.load(phi)
    phi.close()
    with open(r"world/obituary.kr", "rb") as obi:
        pe.futureDead = p.load(obi)
    obi.close()

    # todo new characters don't keep known info about locs and roads?
    # todo can find journal of dead characters to learn all stuff they knew


def saveWorldState():
    nx.write_gml(world, 'world/worldStart.kr')  # saves the world state for future characters

    with open(r"world/itemsStart.kr", "wb") as pit:
        p.dump(it.items, pit)
    pit.close()
    with open(r"world/placesStart.kr", "wb") as ppl:
        p.dump(pl.places, ppl)
    ppl.close()
    with open(r"world/personsStart.kr", "wb") as ppe:
        p.dump(pe.persons, ppe)
    ppe.close()
