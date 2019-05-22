import gui as g
import people as pe
import worlds as w
import pickle as p
import places as pl
import os
import items as it
import events as ev

g.init()

if os.path.exists("world/world.kr"):

    if os.path.exists("player.kr"):
        w.loadWorld()
        it.initItemTypeList()
        ev.initEvents()
        pe.loadPlayer()
        pl.arrive()

    else:
        w.openInitialWorld()
        it.initItemTypeList()
        ev.initEvents()
        pe.createPlayer('human', 0)

else:
    worldSize = int(input("Number of Cities? "))
    infestation = int(input("How infested with monsters is the world? (1-10) "))

    if worldSize >= 2:
        w.world = w.buildWorld(worldSize, infestation)  # build the world if its big enough
        w.saveWorldState()
        pe.createPlayer('human', 0)
        w.saveWorld()

# todo different race options?
# todo adult monsters can birth babies

g.gwin.mainloop()