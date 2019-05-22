import pickle as p
import random as r
import os
import networkx as nx
import items as it
import people as pe
import places as pl
import times as t

world = 5

class road:
    def __init__(self, desc, length, type, known, roughness, travellers):
        self.desc = desc
        self.length = length
        self.type = type
        self.known = known
        self.roughness = roughness
        self.travellers = travellers

def buildWorld(numCities, infestation):     #todo alternate worlds?
    global world

    capidx = 0
    #it.createItem(0)
    it.initItemTypeList()

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

        type = 'road'
        desc = f"a {str(lines[temp[0]])}, {str(lines[temp[1]])} {type}"
        lent = r.randrange(8, 25)
        kn = 0

        w['route'] = road(desc,lent,type,kn,1,[])

    #give each node sites and people
    for x in range(len(web.nodes)):
        web.nodes[x]['name'] = randomName('city')

        web.nodes[x]['label'] = int(x)
        web.nodes[x]['sites'] = [pl.createPlace(1),
                                 pl.createPlace(2),
                                 pl.createPlace(3),
                                 pl.createPlace(5),
                                 pl.createPlace(7)]

                                #todo food sites generate food supply

                                #todo every site has inventory and wants and will buy from neighboring nodes for those goods

        numKrog = r.randrange(infestation + 3, 2 * (infestation + 4))

        web.nodes[x]['monsters'] = pe.createPerson(pTID=1, number=numKrog, location=x)
        # todo add savagery attr to nodes which affect number and strength of krogs
        # todo randomize krog growth times

    # create Druid Circle in a random node
    #web.nodes[r.randrange(len(web.nodes))]['sites'].append(pl.createPlace(4, "Druid Circle"))
    createSiteAtRandomLoc(web, 4, 'Druid Circle')
    createSiteAtRandomLoc(web, 8, 'Hunter Camp')

    # create Krog Hill in a random node
    web.nodes[r.randrange(len(web.nodes))]['monsters'].append(pl.createPlace(4))

    web.graph['instability'] = 0
    web.graph['capital'] = capidx

    kkloc = 3 #todo dont do it this way lol
    #kkloc = r.randrange(r.randrange(len(web.nodes)))
    pe.createBoss()  # todo re-add but change to normal person of kingkrog object
    pe.kingKrog.location = kkloc

    return web

def createSiteAtRandomLoc(web, sTID, name):
    web.nodes[r.randrange(len(web.nodes))]['sites'].append(pl.createPlace(sTID, name))

def worldInfo():
    print("The capital city is " + str(world.graph['capital']))

def saveWorld():
    global world

    nx.write_gpickle(world, r'world/world.kr')
    # nx.write_gml(world, r'world/world.kr')

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
    #todo time is not loaded
    world = nx.read_gpickle(r'world/world.kr')
    #world = nx.read_gml(r'world/world.kr')
    world = nx.convert_node_labels_to_integers(world)

    with open(r"world/items.kr", "rb") as pit:
        it.items = p.load(pit)

    with open(r"world/places.kr", "rb") as ppl:
        pl.places = p.load(ppl)

    with open(r"world/persons.kr", "rb") as ppe:
        pe.persons = p.load(ppe)

    with open(r"world/history.kr", "rb") as phi:
        t.history = p.load(phi)

    with open(r"world/obituary.kr", "rb") as obi:
        pe.futureDead = p.load(obi)

    pe.findBoss()

def openInitialWorld():  # todo I think dead monster inventory loot resets
    global world

    world = nx.read_gpickle(r'world/worldStart.kr')
    #world = nx.read_gml('world/worldStart.kr')
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

    pe.findBoss()

    # todo new characters don't keep known info about locs and roads?
    # todo can find journal of dead characters to learn all stuff they knew

def saveWorldState():
    if not os.path.exists('world'):
        os.makedirs('world')

    nx.write_gpickle(world, r'world/worldStart.kr')
    #nx.write_gml(world, 'world/worldStart.kr')  # saves the world state for future characters

    with open(r"world/itemsStart.kr", "wb") as pit:
        p.dump(it.items, pit)
    pit.close()
    with open(r"world/placesStart.kr", "wb") as ppl:
        p.dump(pl.places, ppl)
    ppl.close()
    with open(r"world/personsStart.kr", "wb") as ppe:
        p.dump(pe.persons, ppe)
    ppe.close()

def randomName(type):
    name = ""
    cnsnnts = ['B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'QU', 'R', 'S', 'T', 'V', 'W', 'X', 'Z']
    oe = ['A', 'E', 'U', 'I', 'O', 'Y']
    if type == 'city':
        for i in range(r.randrange(2,4)):
            name += cnsnnts[r.randrange(len(cnsnnts)-1)] + oe[r.randrange(len(oe)-1)]
    return name