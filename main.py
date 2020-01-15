import os
import boss as b
import events as ev
import gui as g
import items as it
import people as pe
import places as pl
import times as t
import worlds as w

g.init()

if os.path.exists("world/world.kr"):

    if os.path.exists("player.kr"):
        w.loadWorld()
        it.initMaterials()
        it.initItemTypeList()
        pe.initPersonTypeList()
        pl.initPlaceTypeList()
        ev.initEvents()
        pe.loadPlayer()
        pl.arrive()

    else:
        w.openInitialWorld()
        it.initItemTypeList()
        pe.initPersonTypeList()
        pl.initPlaceTypeList()
        ev.initEvents()
        pe.createPlayer('human', 0)

else:
    # worldSize = int(input("Number of Cities? "))
    worldSize = 25
    # infestation = int(input("How infested with monsters is the world? (1-10) "))
    infestation = 1

    if worldSize >= 2:
        w.world = w.buildWorld(worldSize, infestation)  # build the world if its big enough
        pe.createPlayer('human', 0)
        b.createBoss()
        t.timePasses(1000)
        w.saveWorldState()
        w.saveWorld()

# todo different race options?
# todo adult monsters can birth babies

g.dispTown()
g.gwin.mainloop()


def mainlayout(request):
    return render(request, 'krog/layout.html')
