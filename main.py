from sequences import *
from WorldBuilder import *

name = input("Enter your name:")
WorldSize = int(input("Number of Cities? "))

worldWeb, capital = buildWorld(WorldSize)

location = 0
currentHP = 10
overlandSpeed = 3
equip = [1,3]
bag = [5,5]
inv = [equip, bag, 0]

while 1:
  activity = int(input("\n1:Fight\n2:Inventory\n3:Location\n4:World\n"))
  
  if activity == 1:
    result, currentHP, goldGain, xpGain = combat(name, currentHP, location, inv)
    if result == 1:
      inv[2] =+ goldGain
      inv[3] =+ xpGain
      result = 0 
  elif activity == 2:
    inventory(inv)
  elif activity == 3:
    print("Shop\nRumors\nInfo")
    locInfo(worldWeb, location)
  elif activity == 4:
    x = int(input("1: World Info\n2: Travel"))
    if x == 1: worldInfo(worldWeb, capital)
    elif x == 2:
      travres = travel(worldWeb, location)
      if travres != -5:
        print("\nYou walk for " + str(worldWeb[location][travres]['length']) + " miles.")
        print("It took you " + '%.1f'%(int(worldWeb[location][travres]['length']) / overlandSpeed) + " hours.")
        location = travres
        print("You are now in " + str(location))
        

