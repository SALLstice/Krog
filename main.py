from sequences import combat

name = input("Enter your name:")

location = 1
currentHP = 10
weapon = 2
defense = 0
gold = 0 
xp = 0 

while 1:
  activity = int(input("1:Fight\n2:Inventory\n3:Store"))
  
  if activity == 1:
    result, goldGain, xpGain = combat(name, currentHP, defense, location, weapon)
    if result == 1:
      gold = gold + goldGain
      xp = xp + xpGain
      result = 0 
  elif activity == 2:
    print("Weapon: ", weapon)
    print("Armor: ", defense)
    print("Gold: ", gold)
    print("XP: ", xp)
  elif activity == 3:
    option = int(input("1: Better Sword [20]\n2: Better Armor [30]"))
    if option == 1 and gold >= 20:
      gold = gold - 20
      weapon = 3 
    elif option == 1 and gold < 20:
      print ("can't afford")
    elif option == 2 and gold >= 30:
      gold -= 30
      defense = 1
    elif option == 2 and gold < 30:
      print ("can't afford")
    

