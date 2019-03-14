import networkx as nx
import matplotlib.pyplot as plt
import random
import items as it
import places as pl
import people as pe
import times as t

world = 5

def buildWorld(numCities, infestation):     #todo alternate worlds?
    global world

    capidx = 0
    it.createItem(0)

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,1)

    t.createCalendar()      #todo
    web.graph['hour'] = 6

    #finds city with most roads out, names it the capital
    for i in web.nodes:
        if len(web.edges(i)) > len(web.edges(capidx)):
            capidx = i

#    nx.draw(web)
#    plt.show()

    #construct and describe roads
    with open('roaddesc.txt') as f:
        lines = f.read().splitlines()
    
    for (u,v,w) in web.edges(data=True):
        temp = random.sample(range(len(lines)),2)
        
        w['description'] = "a " + str(lines[temp[0]]) + ", " + str(lines[temp[1]]) + " road"
        w['length'] = random.randrange(2,10)
        w['known'] = 0

    #give each node sites and people
    for x in range(len(web.nodes)):
        web.nodes[x]['sites'] = [pl.createSite(1, "Bakery"),
                                 pl.createSite(2, "Weapon Shop"),
                                 pl.createSite(3, "Armory")]

                                #todo food sites generate food supply

                                #todo every site has inventory and wants and will buy from neighboring nodes for those goods
        web.nodes[x]['monsters'] =  pe.createPerson(1, random.randrange(infestation+3,2*(infestation+4))) #todo add savagery attr to nodes which affect number and strength of krogs

    druidLoc = random.randrange(len(web.nodes))
    web.nodes[druidLoc]['sites'].append(pl.createSite(4,"Druid Circle"))

    return web, capidx
