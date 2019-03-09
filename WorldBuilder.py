import networkx as nx
import matplotlib.pyplot as plt
import random
from items import *
from places import *

def buildWorld(numCities):
    capidx = 0

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,1)

    #finds city with most roads out, names it the capital
    for i in web.nodes:
        if len(web.edges(i)) > len(web.edges(capidx)):
            capidx = i

#    nx.draw(web)
#    plt.show()

#construct and describe roads
    with open(r"C:\Users\Matt\Documents\GitHub\Krog\roaddesc.txt") as f:
        lines = f.read().splitlines()
    
    for (u,v,w) in web.edges(data=True):
        temp = random.sample(range(len(lines)),2)
        
        w['description'] = "a " + str(lines[temp[0]]) + ", " + str(lines[temp[1]]) + " road"
        w['length'] = random.randrange(2,10)

#give each node sites
    for x in range(len(web.nodes)):
        web.nodes[x]['sites'] = [createSite(random.randrange(100),'food',x,1,createItem(0,"Curative Herbs",random.randrange(100))),
                                 createSite(random.randrange(100),'armor',x,1,createItem(0,"Padded Clothing",random.randrange(100))),
                                 createSite(random.randrange(100),'weapons',x,1,createItem(0,"Short Sword",random.randrange(100)))]

    return(web, capidx)
