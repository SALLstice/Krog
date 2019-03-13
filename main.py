import items as it
import worlds as w
import combat as c
import people as pe
import places as pl
import times as t
import debug as d

debugMode = 0

name = input("Enter your name:")
worldSize = int(input("Number of Cities? "))
infestation = int(input("How infested with monsters is the world? (1-10) "))
it.items.append(it.item(int(len(it.items)),                                             #create null itemType
                        it.itemTypeList[0].itemType,
                        it.itemTypeList[0].equip,
                        it.itemTypeList[0].combatValue,
                        it.itemTypeList[0].cost,
                        it.itemTypeList[0].effect,
                        it.itemTypeList[0].effectValue,
                        '',
                        ''))

if worldSize >= 2:
    w.world, capital = w.buildWorld(worldSize, infestation)          #build the world if its big enough
else:
    exit()

startlocation = 0
maxHP = 10
stren = 2
tough = 2
overland = 3
equip = [3, 0, 2, 0, 0]
bag = [5]
money = 0


# todo different race options?
pe.createPlayer(name, startlocation, maxHP, maxHP, stren, tough, overland, [equip, bag, money])



# todo monsters grow as time passes
# todo adult monsters can birth babies
while 1:  # todo add in game clock and calendar
    print("\nYou are in " + str(pe.me.location) + ".")
    print("It is", str(w.world.graph['hour']) + ":00.")
    print("\n1 : Explore\n2 : Status\n3 : Region\n4 : World") #todo reorder?
    if debugMode == 1:
        print("100: Find All Hidden Sites")
        print("101: Spawn Item")
    a = int(input())

    if a == 1:  # explore
        duration = int(input("Explore up to how many hours? "))
        exploredEntity, exploredType = pl.exploreRegion(duration)
        if exploredType == 'm':
            print ("You encounter a", pe.persons[exploredEntity].personType)
            combatResult = c.combat(exploredEntity)
        if exploredType == 's':
            print("You encounter a", pl.sites[exploredEntity].siteType)
            pl.sites[exploredEntity].known = 1

    elif a == 2:  # status
        print("1:", pe.me.name,"\n2: Inventory")
        status = int(input())
        if status == 1:
            pe.playerInfo()
        if status == 2:
            it.inventory()
    elif a == 3:  # current location
        pl.locInfo()
    elif a == 4:  # world
        x = int(input("1: World Info\n2: Travel"))
        if x == 1:
            pl.worldInfo(capital)
        elif x == 2:
            pl.travel()

    elif a == 696969:
        debugMode = 1
    elif a == 100 and debugMode == 1: d.findAllHiddenSites()
    elif a == 101 and debugMode == 1:
        for i in range(len(it.itemTypeList)):
            print(i, it.itemTypeList[i].itemType)
        spawn = int(input("Spawn ID?"))
        pe.me.inv[1].append(it.createItem(spawn))