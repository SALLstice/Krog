import combat as c

import gui as g
import people as pe
import worlds as w

debugMode = 0
# todo save character to use. move character file when they die. or just delete?
name = input("Enter your name: ")

try:
    f = open('world/world.kr')  # check if world file already exists
    w.resetWorld()  # then load it if it does
    # print("World Loaded Successfully")

except FileNotFoundError:  # if it doesn't exist...
    worldSize = int(input("Number of Cities? "))
    infestation = int(input("How infested with monsters is the world? (1-10) "))

    if worldSize >= 2:
        w.world = w.buildWorld(worldSize, infestation)  # build the world if its big enough
        w.saveWorldState()

    else:
        exit()

startLocation = 0  #todo randomize start location
maxHP = 10
stren = 2
tough = 2
overland = 3
equip = [3, 0, 2, 0, 0]
bag = [5]
money = 0
speed = 4


# todo different race options?
# pe.createPlayer(name, startLocation)

pe.createPlayer('human', name, startLocation)

# kingKrogLocation = r.randrange(r.randrange(len(w.world.nodes)))
pe.createBoss('King Krog', [3, 0], 1000, 1, 200)

# todo adult monsters can birth babies

g.init()

w.saveWorld()

if pe.kingKrog.location == pe.me.location:
    print("The King Krog Is Here!")
    c.bossCombat()
else:
    g.dispTown()

g.gwin.mainloop()

"""
    if a == 0:
        w.saveWorld()
        quit()
    if a == 1:  # explore
        duration = int(input("Explore up to how many hours? "))
        exploredEntity, exploredType = pl.exploreRegion(duration)
        if exploredType == 'm':
            print ("You encounter a", pe.persons[exploredEntity].personType)
            combatResult = c.combat(exploredEntity)
        if exploredType == 's':
            print("You encounter a", pl.places[exploredEntity].placeType)
            pl.places[exploredEntity].known = 1

    elif a == 2:  # status
        print("1:", pe.me.name,"\n2: Inventory")
        status = int(input())
        if status == 1:
            pe.playerInfo()
        if status == 2:
            it.inventory()
    elif a == 3:  # current location
        pl.visitRegionPlace()
    elif a == 4:  # world
        x = int(input("1: World Info\n2: Travel"))
        if x == 1:
            pl.worldInfo()
        elif x == 2:
            pl.travel()

    elif a == 696969:
        debugMode = 1
    elif a == 100 and debugMode == 1: d.findAllHiddenSites()
    elif a == 101 and debugMode == 1:
        spawn = int(input("Spawn ID?"))
        pe.me.inv[1].append(it.createItem(spawn))
    elif a == 102 and debugMode == 1:
        t.printHistory()
    elif a == 103 and debugMode == 1:
        d.fastDruid()
    elif a == 104 and debugMode == 1:
        pe.me.currentHP = int(input("hp? "))
    elif a == 105 and debugMode == 1:
        d.showMap()
    elif a == 106 and debugMode == 1:
        print(f"\nKing Krog\nSleep timer: {pe.kingKrog.sleepTimer}\nHealth: {pe.kingKrog.currentHP}\nLocation: {pe.kingKrog.location}\nAttack Target: {pe.kingKrog.attackTarget}")
    elif a == 99 and debugMode == 1:
        print("100: Find All Hidden Sites")
        print("101: Spawn Item")
        print("102: Print All History to File")
        print("103: Auto-Druid")
        print("104: Set HP")
        print("105: Show Node Map")
        print("106: King Krog Status")
"""
