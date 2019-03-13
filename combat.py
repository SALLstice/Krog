from random import randrange
import items as it
import people as pe

def combat(enemyPID):

    itemsGained = []

    if pe.persons[enemyPID].currentHP <= 0:
        print("...that you killed earlier.")  # todo allow for picking up items left behind. Might be stolen by traveler
        return 1

    strength = pe.me.strength
    weapon = it.items[pe.me.inv[0][0]].combatValue
    PDR = weapon + strength  # player damage range
    defense = it.items[pe.me.inv[0][1]].combatValue + it.items[pe.me.inv[0][2]].combatValue

    damage = 0
    enemyDamage = 0

    enemyName = pe.persons[enemyPID].name
    enemyHP = pe.persons[enemyPID].currentHP
    enemyWeapon = pe.persons[enemyPID].stats[1]
    enDef = pe.persons[enemyPID].stats[2]
    enInv = pe.persons[enemyPID].inv

    print("\nFIGHT!\n")

    while pe.me.currentHP > 0 and enemyHP > 0:  # todo make comabt display better
        print(pe.me.name, ":", pe.me.currentHP, " ( -", enemyDamage,
              ")")  # todo remove damage display if damage not dealt
        print(enemyName, enemyHP, " ( -", damage, ")")

        print("\n1: Attack\n2: Defend\n")  # todo make defend do anything
        action = int(input("Action:"))  # todo give option to requip in combat
        if action == 1:
            damage = max(0, (randrange(PDR) - enDef))
            enemyDamage = max(0, randrange(enemyWeapon) - defense)

            enemyHP = enemyHP - damage
            pe.me.currentHP = pe.me.currentHP - enemyDamage

            print("------------------")
    if pe.me.currentHP <= 0:
        print("\n\nYOU DIED")
        quit()
    elif enemyHP <= 0:  # enemy killed
        for k in range(len(enInv)):
            itemsGained.append(it.createItem(enInv[k]))  # todo choose which items to gain
        xpGained = randrange(10)  # todo leave remaining items in monster. could start to rot? Gain bones?
        result = 1

        print(pe.me.name, "defeated the", enemyName)
        for i in range(len(itemsGained)):
            print("Gained", it.items[itemsGained[i]].itemType)
        print("Gained", xpGained, "XP")
        for k in range(len(itemsGained)):
            pe.me.inv[1].append(itemsGained[k])

        pe.persons[enemyPID].currentHP = enemyHP  # update person stats after combat
        pe.persons[enemyPID].stats[1] = enemyWeapon
        pe.persons[enemyPID].stats[2] = enDef
        pe.persons[enemyPID].inv = enInv  # todo delete items from enemy inv after being collected

        return result
