import networkx as nx
import pandas as pd
import csv
import matplotlib.pyplot as plt

G=nx.Graph()

with open('nodelist.csv', newline='') as csvfile:
    for row in csvfile:
        rowlist = row.split(",")
        nodeName = rowlist[0]
        x=int(rowlist[1])
        y=rowlist[2]
        y = int(y[0:-2])
        nodeLocation = [x,y]
        G.add_node(nodeName, pos=(x,y),label=nodeName)

with open('edgelist.csv', newline='') as csvfile:
    for row in csvfile:
        rowlist = row.split(",")
        G.add_edge(rowlist[0], rowlist[1])

pos=nx.get_node_attributes(G,'pos')
nx.draw(G,pos,with_labels=True)

plt.show()