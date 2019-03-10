import WorldBuilder as wb
import combat as c
import item as it
import people as pe
import places as pl

name = input("Enter your name:")
WorldSize = int(input("Number of Cities? "))

worldWeb, capital = wb.buildWorld(WorldSize)

location = 0

pe.createPlayer(name,location,10,[2, 2],[[it.createItem(2),it.createItem(1)], [it.createItem(4)], 0])

currentHP = 10
overlandSpeed = 3

while 1:
  a = int(input("\n1:Fight\n2:Inventory\n3:Location\n4:World\n"))
  
  if a == 1: #fight
    result = c.combat()
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