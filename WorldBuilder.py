import networkx as nx

def BuildWorld(numCities):
    capidx = 0

    #randomly generated node web
    web = nx.barabasi_albert_graph(numCities,2)

    #finds city with most roads out, names it the capital
    for i in web.nodes:
        if len(web.edges(i)) > len(web.edges(capidx)):
            capidx = i

    return(web)
