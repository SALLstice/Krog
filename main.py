import item as it
import WorldBuilder as wb
import combat as c
import people as pe
import places as pl

name = input("Enter your name:")
WorldSize = int(input("Number of Cities? "))
it.items.append(it.item(int(len(it.items)),
                          it.itemTypeList[0].itemType,
                          it.itemTypeList[0].equip,
                          it.itemTypeList[0].combatValue,
                          it.itemTypeList[0].cost,
                          it.itemTypeList[0].effect,
                          it.itemTypeList[0].effectValue,
                          '',
                          ''))

if WorldSize >= 2:
  worldWeb, capital = wb.buildWorld(WorldSize)
else:
  exit()

location = 0

pe.createPlayer(name,10,[2,2],[[3,0,0,0,0], [2,5], 0])

currentHP = 10
overlandSpeed = 3
#todo monsters grow as time passes
#todo adult monsters can birth babies
while 1: #todo add in game clock and calendar
  a = int(input("\n1:Fight\n2:Inventory\n3:Location\n4:World\n"))
  
  if a == 1: #fight #todo make fight explore with options of finding hidden locations
    result = c.combat(worldWeb.nodes[location]['monsters'])
    if result == 1:
      result = 0 
  elif a == 2: #inventory
    it.inventory()
  elif a == 3: #current location
    pl.locInfo(worldWeb, location)
  elif a == 4: #world
    x = int(input("1: World Info\n2: Travel"))
    if x == 1: pl.worldInfo(capital)
    elif x == 2:
      travres = pl.travel(worldWeb, location)
      if travres != -5:
        print("\nYou walk for " + str(worldWeb[location][travres]['length']) + " miles.")
        print("It took you " + '%.1f'%(int(worldWeb[location][travres]['length']) / overlandSpeed) + " hours.")
        location = travres
        print("You are now in " + str(location))