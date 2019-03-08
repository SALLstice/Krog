from random import randrange
from items import *
from places import *

enemies = ["Baby Krog", 4, 2, 0]

def travel(web, loc):
    print("\nYou are in " + str(loc) + ".")
    print("There are " + str(len(web.edges(loc))) + " roads out of " + str(loc) + ": ")
    print("0 : Don't travel")
    for j in range(len(list(web.neighbors(loc)))):
        print(j+1, ": " + web[loc][list(web.neighbors(loc))[j]]['description'])
    trav = int(input("Which road will you travel?\n"))
    if trav == 0: return -5
    return(list(web.neighbors(loc))[trav-1])

def worldInfo(web, cap):
  print("The capital city is " + str(cap))

def locInfo(web, loc):
  roads = []
  print("\nYou are in " + str(loc) + ".")
  for i in range(len(web.node[1]['buildings'])):
    print(str(i) + ": " + checkBuilding(web.node[1]['buildings'][i]))
  return input()

def store(web, loc, inv):
    option = int(input("1: Club [20]\n2: Padded Shirt [30]"))
    if option == 1 and inv[2] >= 20:
      inv[2] -= 20 
    elif option == 1 and inv[2] < 20:
      print ("can't afford")
    elif option == 2 and inv[2] >= 30:
      inv[2] -= 30
      inv[3] = 1
    elif option == 2 and inv[2] < 30:
      print ("can't afford")
    return(inv)

def inventory(inv):
  print("\nItems you have equipped:")
  for i in range(len(inv[0])):
    print(checkItem(inv[0][i]))
  print("\nItems in your bag:")
  for j in range(1, len(inv[1])):
    print(checkItem(inv[1][j]))
    
def combat(name,currentHP,location,inv):

  weapon = inv[0]
  defense = inv[1]
  
  damage = 0 
  enemyDamage = 0
  
  if location == 0:
    enemyType = 1
    enemyName = "Baby Krog"
    enemyHP = randrange(4)+1
    enemyWeapon = 2
    enDef = 0
  
  print("\nFIGHT!\n")
  
  while currentHP > 0 and enemyHP > 0:
    print(name,":",currentHP," ( -",enemyDamage,")")
    print(enemyName, enemyHP, " ( -",damage,")")

    print("\n1: Attack\n2: Defend\n")
    action = int(input("Action:"))
    if action == 1:
      damage = (randrange(weapon) - enDef)
      enemyDamage = randrange(enemyWeapon) - defense
      
      enemyHP = enemyHP - damage
      currentHP = currentHP - enemyDamage
      
      print("------------------")
  
  if enemyHP <= 0:
    goldGained = randrange(5)
    xpGained = randrange(10)
    result = 1
    
    print (name, "defeated the", enemyName)
    print("Gained ",goldGained,"gold.")
    print("Gained",xpGained,"XP")
    return result, currentHP, goldGained, xpGained
