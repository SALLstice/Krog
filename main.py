from sequences import *
from WorldBuilder import *
from places import *

name = input("Enter your name:")
WorldSize = int(input("Number of Cities? "))

worldWeb, capital = buildWorld(WorldSize)

location = 0
currentHP = 10
overlandSpeed = 3
equip = [createItem(0,'Training Sword',0),createItem(0,'Clothing',0)]
bag = [createItem(0,'Curative Herbs',0)]
inv = [equip, bag, 0]

while 1:
  a = int(input("\n1:Fight\n2:Inventory\n3:Location\n4:World\n"))
  
  if a == 1:
    result, currentHP, goldGain, xpGain = combat(name, currentHP, location, inv)
    if result == 1:
      inv[2] =+ goldGain
      inv[3] =+ xpGain
      result = 0 
  elif a == 2:
    inventory(inv)
  elif a == 3:
    print("Shop\nRumors\nInfo")
    locInfo(worldWeb, location)
  elif a == 4:
    x = int(input("1: World Info\n2: Travel"))
    if x == 1: worldInfo(worldWeb, capital)
    elif x == 2:
      travres = travel(worldWeb, location)
      if travres != -5:
        print("\nYou walk for " + str(worldWeb[location][travres]['length']) + " miles.")
        print("It took you " + '%.1f'%(int(worldWeb[location][travres]['length']) / overlandSpeed) + " hours.")
        location = travres
        print("You are now in " + str(location))
  elif a == 696969:
    debug = int(input("\n Debug \n 1: every item type \n2: every item entity\n3: every site entity"))
    if debug == 1:
      for i in range(len(itemList)):
        print(itemList[i].name)
    elif debug == 2:
        for i in range(len(itemEntityList)):
          print(itemEntityList[i].entityID,itemEntityList[i].name, itemEntityList[i].itemType, itemEntityList[i].desc)
    elif debug == 3:
        for i in range(len(siteEntityList)):
          print(siteEntityList[i].entityID,siteEntityList[i].name, siteEntityList[i].siteType, siteEntityList[i].loc, siteEntityList[i].civil)
