import networkx as nx
import matplotlib.pyplot as plt

def BuildWorld(numCities):
    capidx = 0

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,1)

    #finds city with most roads out, names it the capital
    for i in web.nodes:
#        print("Roads out of " + str(i) + ": " + str(web.edges(i)) + str(len(web.edges(i))))
        if len(web.edges(i)) > len(web.edges(capidx)):
            capidx = i

    nx.draw(web)
    plt.show()

    return(web, capidx)
