from sequences import *
from WorldBuilder import *

name = input("Enter your name:")
WorldSize = int(input("Number of Cities? "))

worldWeb, capital = BuildWorld(WorldSize)

location = 0
currentHP = 10
inv = [2, 0, 0, 0]

while 1:
  activity = int(input("1:Fight\n2:Inventory\n3:Store\n4:Location\n"))
  
  if activity == 1:
    result, currentHP, goldGain, xpGain = combat(name, currentHP, location, inv)
    if result == 1:
      inv[2] =+ goldGain
      inv[3] =+ xpGain
      result = 0 
  elif activity == 2:
    inventory(inv)
  elif activity == 3:
    inv = store(inv)
  elif activity == 4:
    locInfo(worldWeb, location, capital)
    

