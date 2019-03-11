import networkx as nx
import matplotlib.pyplot as plt
import random
import item as it
import places as pl
import people as pe

def buildWorld(numCities):
    capidx = 0
    it.createItem(0)

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,1)

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

#give each node sites and people
    for x in range(len(web.nodes)):
        web.nodes[x]['sites'] = [pl.createSite(random.randrange(100),'food',x,1,[it.createItem(5),it.createItem(5)]),
                                 pl.createSite(random.randrange(100),'armor',x,1,[it.createItem(2),it.createItem(4)]),
                                 pl.createSite(random.randrange(100),'weapons',x,1,[it.createItem(3),it.createItem(1)])]
                                #todo some sites are outside the town, require being found. uses civil attr
                                #todo food sites generate food supply
                                #todo druid ciricle site can tell how many krogs are left in area
                                #todo every site has inventory and wants and will buy from neighboring nodes for those goods
        web.nodes[x]['monsters'] =  pe.createPerson(1) #todo spawn set number of monsters in each node

    return web, capidx
