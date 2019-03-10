from random import randrange
import item as it
import people as pe

def combat():

  enemies = ["Baby Krog", 1, 1, 0, it.createItem(6)]

  weapon = it.items[pe.me.inv[0][0]].combatValue
  defense = it.items[pe.me.inv[0][2]].combatValue + it.items[pe.me.inv[0][1]].combatValue

  damage = 0
  enemyDamage = 0

  enemyName = enemies[0]
  enemyHP = enemies[1]+1
  enemyWeapon = enemies[2]
  enDef = enemies[3]
  
  print("\nFIGHT!\n")
  
  while pe.me.currentHP > 0 and enemyHP > 0:
    print(pe.me.name,":",pe.me.currentHP," ( -",enemyDamage,")")
    print(enemyName, enemyHP, " ( -",damage,")")

    print("\n1: Attack\n2: Defend\n")
    action = int(input("Action:"))
    if action == 1:
      damage = (randrange(weapon) - enDef)
      enemyDamage = randrange(enemyWeapon) - defense
      
      enemyHP = enemyHP - damage
      pe.me.currentHP = pe.me.currentHP - enemyDamage
      
      print("------------------")
  
  if enemyHP <= 0:
    itemsGained = enemies[4]
    xpGained = randrange(10)
    result = 1
    
    print (pe.me.name, "defeated the", enemyName)
    print("Gained",it.items[itemsGained].itemType)
    print("Gained",xpGained,"XP")

    pe.me.inv[1].append(itemsGained)
    return result
