import networkx as nx
import matplotlib.pyplot as plt

capidx = 0

#number of locations
n = int(input("n? "))

#randomly generated node web
web = nx.barabasi_albert_graph(n,2)

#finds city with most roads out, names it the capital
for i in web.nodes:
    print("Roads out of " + str(i) + ": " + str(web.edges(i)) + str(len(web.edges(i))))
    if len(web.edges(i)) > len(web.edges(capidx)):
        capidx = i

#printout
print("Number of cities: " + str(web.number_of_nodes()))
print("Number of roads: " + str(web.number_of_edges()))
print("Captial: " + str(capidx))

