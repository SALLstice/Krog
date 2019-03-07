import networkx as nx
import matplotlib.pyplot as plt

capidx = 0

n = int(input("n? "))

web = nx.barabasi_albert_graph(n,2)

print("Number of cities: " + str(web.number_of_nodes()))
print("Number of roads: " + str(web.number_of_edges()))

for i in web.nodes:
    print("Roads out of " + str(i) + ": " + str(web.edges(i)) + str(len(web.edges(i))))
    if len(web.edges(i)) > len(web.edges(capidx)):
        capidx = i

print("Captial: " + str(capidx))

#nx.draw(web)
#plt.show()

