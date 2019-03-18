import items as it
import people as pe
import times as t
import worlds as w
from random import randrange


def combat(enemyPID):
    enInv = pe.persons[enemyPID].inv

    if pe.persons[enemyPID].currentHP > 0:

        itemsGained = []

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
            print("\nYOU DIED")
            #            w.world.nodes[pe.me.location]['monsters'].append(pe.createBody())            #create dead body entity #todo make coin pouch item entity #todo make option for making dead body anywhere
            inven = pe.me.inv[0] + pe.me.inv[1]
            inven = list(filter((0).__ne__, inven))

            pe.futureDead.append(pe.dead(len(pe.futureDead),
                                         # todo make characters list to pull for all events, update when they die
                                         2,  # todo make optional for other races
                                         pe.me.name,
                                         inven,
                                         t.now(),
                                         pe.me.location))
            t.createEvent(t.now(), len(pe.futureDead) - 1, 'died', enemyPID, pe.me.location)

            w.saveWorld()
            t.printHistory()
            quit()
        elif enemyHP <= 0:  # enemy killed

            xpGained = randrange(10)
            # todo count krogs kills. ultimate krog wakes after X number have been killed. Include time-shifted kills

            t.createEvent(t.now(), pe.me.name, 'kills', enemyPID, pe.me.location)

            print(pe.me.name, "defeated the", enemyName)
            print("Gained", xpGained, "XP")  # todo XP doesn't do anything

            pe.persons[enemyPID].currentHP = enemyHP  # update person stats after combat
            pe.persons[enemyPID].stats[1] = enemyWeapon
            pe.persons[enemyPID].stats[2] = enDef

    elif pe.persons[enemyPID].currentHP <= 0:
        print("...that is dead.")  # todo unlooted items Might be stolen by traveler

    print("\n-Loot-")

    for k in range(len(enInv)):
        print(k, it.items[enInv[k]].itemType)

    loot = list(str.split(input("Loot what? \n")))  # todo no loot option if they are empty inv

    if loot[0] == "99":  # todo make this better
        return

    for i in range(len(loot)):
        loot.sort(key=int, reverse=True)
        pe.me.inv[1].append(enInv[int(loot[i])])  # add item to player inventory
        del enInv[int(loot[i])]  # remove item from loot

    pe.persons[enemyPID].inv = enInv

    return
