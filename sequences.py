from random import randrange

enemies = ["Baby Krog", 4, 2, 0]

def combat(name,HP,defense,location,weapon):
  
  damage = 0 
  enemyDamage = 0
  
  if location == 1:
    enemyType = 1
    enemyName = "Baby Krog"
    enemyHP = randrange(4)+1
    enemyWeapon = 2
    enDef = 0
  
  print("\nFIGHT!\n")
  
  while HP > 0 and enemyHP > 0:
    print(name,":",HP," ( -",enemyDamage,")")
    print(enemyName, enemyHP, " ( -",damage,")")

    print("\n1: Attack\n2: Defend\n")
    action = int(input("Action:"))
    if action == 1:
      damage = (randrange(weapon) - enDef)
      enemyDamage = randrange(enemyWeapon) - defense
      
      enemyHP = enemyHP - damage
      HP = HP - enemyDamage
      
      print("------------------")
  
  if enemyHP <= 0:
    goldGained = randrange(5)
    xpGained = randrange(10)
    result = 1
    
    print (name, "defeated the", enemyName)
    print("Gained ",goldGained,"gold.")
    print("Gained",xpGained,"XP")
    return result, goldGained, xpGained