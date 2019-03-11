from random import randrange
import item as it
import people as pe

def combat(monsterList):

  if type(monsterList) == list:
    enemyPID = monsterList[randrange(len(monsterList))]
  elif type(monsterList) == int:
      enemyPID = monsterList

  weapon = it.items[pe.me.inv[0][0]].combatValue #todo make it your STR + weapon. avoids error with no weapon equipped
  defense = it.items[pe.me.inv[0][1]].combatValue + it.items[pe.me.inv[0][2]].combatValue

  damage = 0
  enemyDamage = 0

  enemyName = pe.persons[enemyPID].name
  enemyHP = pe.persons[enemyPID].currentHP
  enemyWeapon = pe.persons[enemyPID].stats[1]
  enDef = pe.persons[enemyPID].stats[2]
  enInv = pe.persons[enemyPID].inv
  
  print("\nFIGHT!\n")
  
  while pe.me.currentHP > 0 and enemyHP > 0:
    print(pe.me.name,":",pe.me.currentHP," ( -",enemyDamage,")")
    print(enemyName, enemyHP, " ( -",damage,")")

    print("\n1: Attack\n2: Defend\n") #todo make defend do anything
    action = int(input("Action:")) #todo give option to requip in combat
    if action == 1:
      damage = max(0,(randrange(weapon) - enDef))
      enemyDamage = max(0,randrange(enemyWeapon) - defense)
      
      enemyHP = enemyHP - damage
      pe.me.currentHP = pe.me.currentHP - enemyDamage
      
      print("------------------")
  
  if enemyHP <= 0:
    itemsGained = enInv
    xpGained = randrange(10)
    result = 1
    
    print (pe.me.name, "defeated the", enemyName)
    for i in range(len(itemsGained)):
        print("Gained",it.items[i].itemType)
    print("Gained",xpGained,"XP")

    pe.me.inv[1].append(itemsGained)
    return result
