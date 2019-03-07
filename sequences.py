from random import randrange

enemies = ["Baby Krog", 4, 2, 0]

def locInfo(web, loc):
  print("\nYou are in " + str(loc) + ".")
  print("There are " + str(len(web.edges(loc))) + " roads out of " + str(loc) + ": " + str(web.edges(loc)))

def store(inv):
    option = int(input("1: Better Sword [20]\n2: Better Armor [30]"))
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
    print("Weapon: ", inv[0])
    print("Armor: ", inv[1])
    print("Gold: ", inv[2])
    print("XP: ", inv[3])
    
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
