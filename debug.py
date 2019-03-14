import items as it
import people as pe
import places as pl
import worlds as w

def findAllHiddenSites():
    for i in range(len(pl.sites)):
        pl.sites[i].known = 1

def spawnItem(iID):
    pe.me.inv.append(it.createItem(iID))

