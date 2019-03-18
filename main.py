import combat as c
import debug as d
import items as it
import people as pe
import places as pl
import times as t
import worlds as w

debugMode = 0
# todo save character to use. move character file when they die. or just delete?
name = input("Enter your name: ")

it.items.append(it.item(int(len(it.items)),                                             #create null itemType
                        it.itemTypeList[0].itemType,
                        it.itemTypeList[0].equip,
                        it.itemTypeList[0].combatValue,
                        it.itemTypeList[0].cost,
                        it.itemTypeList[0].effect,
                        it.itemTypeList[0].effectValue,
                        '',
                        ''))

try:
    f = open('world/world.kr')  # check if world file already exists
    w.resetWorld()  # then load it if it does
    print("World Loaded Successfully")

except FileNotFoundError:  # if it doesn't exist...
    worldSize = int(input("Number of Cities? "))
    infestation = int(input("How infested with monsters is the world? (1-10) "))

    if worldSize >= 2:
        w.world = w.buildWorld(worldSize, infestation)  # build the world if its big enough
        w.saveWorldState()

    else:
        exit()

startlocation = 0  #todo randomize start location
maxHP = 10
stren = 2
tough = 2
overland = 3
equip = [3, 0, 2, 0, 0]
bag = [5]
money = 0


# todo different race options?
pe.createPlayer(name, startlocation, maxHP, maxHP, stren, tough, overland, [equip, bag, money])

# todo adult monsters can birth babies
while 1:
    w.saveWorld()
    print("\nYou are in " + str(pe.me.location) + ".")
    print("it is %i:00. The date is %i/%i/%i" % (
    w.world.graph['hour'], w.world.graph['month'], w.world.graph['day'], w.world.graph['year']))
    print("\n1 : Explore\n2 : Status\n3 : Region\n4 : World") #todo reorder?
    if debugMode == 1:
        print("99: Debug Help")
    a = int(input())

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
    elif a == 99 and debugMode == 1:
        print("100: Find All Hidden Sites")
        print("101: Spawn Item")
        print("102: Print All History to File")
        print("103: Auto-Druid")
        print("104: Set HP")
        print("105: Show Node Map")
