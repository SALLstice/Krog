import networkx as nx
import matplotlib.pyplot as plt
import random

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

    with open(r"C:\Users\Matt\Documents\GitHub\Krog\roaddesc.txt") as f:
        lines = f.read().splitlines()
    
    for (u,v,w) in web.edges(data=True):
        temp = random.sample(range(len(lines)),2)
        
        w['description'] = "a " + str(lines[temp[0]]) + ", " + str(lines[temp[1]]) + " road"
        w['length'] = random.randrange(2,10)

    for x in web.nodes(data=True):
        nx.set_node_attributes(web,[1, 2, 3],'buildings')

    return(web, capidx)
