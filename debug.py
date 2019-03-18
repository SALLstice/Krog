import matplotlib.pyplot as plt
import networkx as nx

import items as it
import people as pe
import places as pl
import worlds as w


def findAllHiddenSites():
    for i in range(len(pl.places)):
        pl.places[i].known = 1

def spawnItem(iID):
    pe.me.inv.append(it.createItem(iID))


def fastDruid():
    krogCount = 0

    regionToSearch = int(input("Which region? "))
    for j in range(len(w.world.node[regionToSearch]['monsters'])):  # count all living krogs in the node
        if pe.persons[w.world.node[regionToSearch]['monsters'][j]].currentHP >= 1:
            krogCount += 1
    print("There are", krogCount, "krogs still alive around", regionToSearch)


def showMap():
    nx.draw(w.world, with_labels=True)
    plt.show()
