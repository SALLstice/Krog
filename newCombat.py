import random as r

import time as t

import gui as g
import people as pe
import times as ti

baddie = -1


class statusEffect:
    def __init__(self, name, effect, effectValue, secondaryValue):
        self.name = name
        self.effect = effect
        self.effectValue = effectValue
        self.secondaryValue = secondaryValue


class activeStatus:
    def __init__(self, statusEffect, duration):
        self.statusEffect = statusEffect
        self.duration = duration


def initCombat(tempBaddie):
    global baddie

    g.gwin.button1.grid()
    g.gwin.button2.grid()
    g.gwin.button3.grid()

    baddie = tempBaddie
    setattr(baddie, 'TIBS', 50)

    g.setText(label4=baddie.name, label5=baddie.currentHP)
    tickUntilTurn()


def readyMonsterAttack():
    global gwin

    monsterActions = [5, 35, 100], [10, 5, 2], [75, 60, 30], [-10, 0, 5], ["powerful attack!", "strong attack.",
                                                                           "fast attack."]
    actionSelect = r.randrange(100)
    idx, monsterAttack = 0, 0

    for idx, actionRate in enumerate(monsterActions[0]):
        if actionSelect <= actionRate:
            monsterAttack = idx
            break

    g.setText(label3=f"The monster is making a {monsterActions[4][idx]}")
    g.gwin.button0["text"] = "Continue"
    g.gwin.update()
    baddie.TIBS = monsterActions[2][idx]
    g.gwin.button0["command"] = lambda: tickUntilTurn()

    return baddie.TIBS, monsterAttack


def tickUntilTurn():
    global gwin

    if pe.me.retreating:
        g.setText(text2="You are running away.")

    while pe.me.TIBS > 0 and baddie.TIBS > 0:
        tempSpeedMod = 0
        poisonDamage = 0

        Ticker = "..................................................................................................."

        # todo re-add
        # for each in monsterStatus:
        #    if each.statusEffect.effect == "speed":
        #        tempSpeedMod += each.statusEffect.effectValue

        pe.me.TIBS -= pe.me.speed
        baddie.TIBS -= int(baddie.speed) + tempSpeedMod

        pe.me.TIBS = max(0, pe.me.TIBS)
        baddie.TIBS = max(0, baddie.TIBS)

        Ticker = Ticker[0:pe.me.TIBS] + "P" + Ticker[pe.me.TIBS:len(Ticker)]
        Ticker = Ticker[0:baddie.TIBS] + "K" + Ticker[baddie.TIBS:len(Ticker)]

        g.gwin.TIBSlabel["text"] = Ticker

        g.setText(label4=baddie.name, label5=baddie.currentHP)
        g.gwin.update()

        # todo re-add
        # for idx, val in enumerate(monsterStatus):
        #    print(val.statusEffect.name, val.duration)

        # for each in monsterStatus:
        #    if each.statusEffect.effect == "poison":
        #        if r.randrange(100) < each.statusEffect.secondaryValue:
        #            poisonDamage += r.randrange(int(each.statusEffect.effectValue / 2), each.statusEffect.effectValue)

        #   each.duration -= 1
        #    if each.duration < 0:
        #        monsterStatus.remove(each)

        # if poisonDamage >= 1:
        #    print(f"The monster takes {poisonDamage} damage from poison")

        t.sleep(.1)

    if baddie.TIBS <= 0:
        baddie.TIBS, monsterAction = monsterTurn()
    elif pe.me.TIBS <= 0:
        playerTurn()


def tickUntilStatusClear(monsterStatus):
    poisonDamage = 0

    while len(monsterStatus) > 0:
        for each in monsterStatus:
            if each.statusEffect.effect == "poison":
                if r.randrange(100) < each.statusEffect.secondaryValue:
                    poisonDamage += r.randrange(int(each.statusEffect.effectValue / 2), each.statusEffect.effectValue)
            # todo apply damage here
            # todo check if monster dies
            # todo option to return to loot

            each.duration -= 1
            if each.duration < 0:
                monsterStatus.remove(each)

    if poisonDamage >= 1:
        print(f"The monster takes {poisonDamage} damage from poison")


def monsterTurn():
    monsterDodge = 10
    monsterAttackSkill = 15
    monsterStrength = 5
    monsterSpeed = 3
    monsterActions = [5, 35, 100], [10, 5, 2], [75, 60, 30], [-10, 0, 5], ["powerful attack!", "strong attack.",
                                                                           "fast attack."]
    monsterAttack = 0

    dodgeChance = pe.me.skills.Dodge - int(pe.me.equippedArmor.secondaryEffectValue) + monsterAttackSkill

    hit = r.randrange(100) + monsterActions[3][monsterAttack]
    damage = r.randrange(monsterActions[1][monsterAttack]) + monsterStrength - \
             int(pe.me.equippedArmor.baseEffectValue)

    if hit >= dodgeChance:
        if damage <= 0:
            g.setText(label1="You block all damage")
        elif damage > 0:
            g.setText(label1=f"you take {damage}")
            if r.randrange(100) >= pe.me.skills.Dodge:
                pe.me.skills.Dodge += 1
                g.setText(label2=f"Your Dodge skill increases to {pe.me.skills.Dodge}!")

    else:
        g.setText(label1="You dodge")

    g.gwin.button0["text"] = "Continue"
    g.gwin.button0["command"] = lambda: readyMonsterAttack()
    g.gwin.update()

    return monsterActions[2][monsterAttack], monsterAttack


def playerTurn():
    global baddie

    attackType = "Q"

    if pe.me.retreating:
        g.setText(label1="You escape.", label2="ccc")
        tickUntilStatusClear(baddie)
        # break
        # todo

    g.setText(label1="Choose your action")

    if attackType == "Q":
        g.gwin.button0["text"] = "Quick Attack"
    elif attackType == "S":
        g.gwin.button0["text"] = "Strong Attack"
    elif attackType == "D":
        g.gwin.button0["text"] = "Double Strike"
    g.gwin.button0["command"] = lambda: attack()

    # todo
    g.gwin.button1["text"] = "Defend"
    # gwin.button1["command"] = lambda : defend()
    g.gwin.button2["text"] = "Items"
    # gwin.button2["command"] = lambda : items()
    g.gwin.button3["text"] = "Tactics"
    g.gwin.button3["command"] = lambda: tactics()


def combat(gui, ePID):
    global monsterStatus
    global monsterSpeed
    global gwin

    gwin = gui
    # turn = "x"

    attackType = 'Q'
    runFlag = 0

    monsterStatus = []
    #    eTIBS = 0

    #    equippedWeapon = spear
    #    equippedArmor = leatherArmor
    #    inv = [healingElixer, icyLongSword, fireyLongSword, poisonPotion]
    #    setattr(skills, "Dodging", 0)
    # ---------------------------------------------------------------------------------------------------------

    readyMonsterAttack(monsterAttacks)
    #    TIBS = 30

    # try:
    #    attackSkill = getattr(skills, equippedWeapon.name)
    # except:
    #    setattr(skills, equippedWeapon.name, 0)
    #    attackSkill = getattr(skills, equippedWeapon.name)

    # hitChance = attackSkill + equippedWeapon.secondaryEffectValue - monsterDodge

    defense = equippedArmor.baseEffectValue

    # while 1:
    # while turn == "x":
    #    turn, TIBS, eTIBS = tick(monsterStatus,monsterSpeed,TIBSSpeed,TIBS,eTIBS)
    #    updateTicker(gwin,TIBS,eTIBS)

    turn = tickUntilTurn()
    # gwin.update()
    # print("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n")

    if turn == "P" or turn == "B":

        try:
            attackSkill = getattr(skills, equippedWeapon.name)
        except:
            setattr(skills, equippedWeapon.name, 0)
            attackSkill = getattr(skills, equippedWeapon.name)

        hitChance = attackSkill + equippedWeapon.secondaryEffectValue - monsterDodge
        dodgeChance = skills.Dodging - equippedArmor.secondaryEffectValue + monsterAttackSkill
        defense = equippedArmor.baseEffectValue

        if attackType == "Q":
            gwin.button0["text"] = "Quick Attack"
        elif attackType == "S":
            gwin.button0["text"] = "Strong Attack"
        elif attackType == "D":
            gwin.button0["text"] = "Double Strike"
        TIBS = gwin.button0["command"] = lambda: attack(attackType, equippedWeapon)

        # print("2:Dodge\n3:Block\n4:Retreat\n5:Choose Attack Type\n6: Items")
        gwin.button1["text"] = "Defend"
        gwin.button2["text"] = "Items"
        gwin.button3["text"] = "Tactics"

        gwin.mainloop()
        # do = int(input("\n"))
        """
        if do == 4:
            TIBS = 100
            runFlag = 1
        if do == 5:
            attackType = input("QSD")
        if do == 2:
            dodgeChance += 50
            TIBS = 30
        if do == 3:
            defense *= 2
            TIBS = 30
        if do == 1:
            damage = 0
            if r.randrange(100) <= hitChance:
                damage += r.randrange(int(equippedWeapon.baseEffectValue / 2), equippedWeapon.baseEffectValue)
                if attackType == "S":
                    damage += strength
                print(f"You deal {damage}")

                if equippedWeapon.specialEffect == "Icy":
                    if r.randrange(100) <= 33:
                        monsterStatus.append(activeStatus(slow, 10))
                if equippedWeapon.specialEffect == "Firey":
                    if r.randrange(100) <= 40:
                        temp = equippedWeapon.specialEffectValue
                        print(f"You deal an additional {r.randrange(int(temp / 2), temp)} fire damage.")

                if attackType == "Q":
                    TIBS = 30
                if attackType == "S":
                    TIBS = 70
            else:
                print("You miss")
                TIBS = 25
                if attackType == "S":
                    TIBS += 10
                if r.randrange(100) >= getattr(skills, equippedWeapon.name):
                    setattr(skills, equippedWeapon.name, getattr(skills, equippedWeapon.name) + 1)
                    print(f"Your {equippedWeapon.name} skill increases to {getattr(skills, equippedWeapon.name)}!")

                    # todo cap at 100
            input()
        if do == 6:
            for idx, val in enumerate(inv):
                print(idx, val.specialEffect, val.name)
            select = int(input())

            if inv[select].type == "consumable":
                if inv[select].specialEffect == "Healing":
                    effect = r.randrange(int(inv[select].specialEffectValue / 2), inv[select].specialEffectValue)
                    print("you heal", effect)
                    input()
            if inv[select].specialEffect == "Poison":
                monsterStatus.append(activeStatus(lightPoison, 10))

            elif inv[select].type == "weapon":
                equippedWeapon = inv[select]
            elif inv[select].type == "armor":
                equippedArmor = inv[select]
        """


def setupAttackType():
    g.gwin.button0["text"] = "Quick Attack"
    g.gwin.button0["command"] = pe.me.attackType = "Q"

    g.gwin.button1["text"] = "Strong Attack"
    g.gwin.button1["command"] = pe.me.attackType = "S"
    # todo


def setAttackType(atkType):
    pe.me.attackType = atkType
    tickUntilTurn()


def attack():
    # todo put this at top of every func becuase double clicking can double attack
    g.gwin.button0["command"] = ""

    eqWep = pe.me.equippedWeapon

    try:
        attackSkill = getattr(pe.me.skills, eqWep.itemType)
    except:
        setattr(pe.me.skills, eqWep.itemType, 0)
        attackSkill = getattr(pe.me.skills, eqWep.itemType)

    hitChance = attackSkill + eqWep.secondaryEffectValue  # - monsterDodge #todo

    damage = 0
    if r.randrange(100) <= hitChance:
        damage += r.randrange(int(eqWep.baseEffectValue / 2), eqWep.baseEffectValue)
        if pe.me.attackType == "S":
            damage += pe.me.strength
        g.setText(label2=f"You deal {damage}")
        baddie.currentHP -= damage
        baddie.currentHP = max(0, baddie.currentHP)
        ti.createEvent(ti.now(), pe.me.name, 'wounds', baddie, pe.me.location, extra=damage)
        """
        if equippedWeapon.specialEffect == "Icy":
            if r.randrange(100) <= 33:
                monsterStatus.append(activeStatus(slow, 10))
        if equippedWeapon.specialEffect == "Firey":
            if r.randrange(100) <= 40:
                temp = equippedWeapon.specialEffectValue
                print(f"You deal an additional {r.randrange(int(temp / 2), temp)} fire damage.")
        """
        if pe.me.attackType == "Q":
            pe.me.TIBS = 30
        if pe.me.attackType == "S":
            pe.me.TIBS = 70
    else:
        g.setText(label1="You miss")
        pe.me.TIBS = 25
        if pe.me.attackType == "S":
            pe.me.TIBS += 10
        if r.randrange(100) >= getattr(pe.me.skills, eqWep.itemType):
            setattr(pe.me.skills, eqWep.itemType, getattr(pe.me.skills, eqWep.itemType) + 1)
            g.setText(label2=f"Your {eqWep.itemType} skill increases to {getattr(pe.me.skills, eqWep.itemType)}!")

            # todo cap at 100
    if int(baddie.currentHP) > 0:
        g.gwin.button0["text"] = "Continue"
        g.gwin.button0["command"] = lambda: tickUntilTurn()
    elif int(baddie.currentHP) == 0:
        killedTheMonster(baddie)


def tactics():
    g.gwin.button0["text"] = "Attack Type"
    g.gwin.button0["command"] = lambda: setupAttackType()

    g.gwin.button1["text"] = "Retreat"
    g.gwin.button1["command"] = lambda: retreat()

    g.gwin.button2["text"] = "-"
    g.gwin.button2["command"] = ""

    g.gwin.button3["text"] = "Return"
    g.gwin.button3["command"] = lambda: playerTurn()


def retreat():
    pe.me.retreating = True
    pe.me.TIBS = 99
    tickUntilTurn()


def killedTheMonster(badd):
    display = f"You killed the {badd.name}!"
    g.gwin.label0["text"] = display
    g.gwin.button0["text"] = "Loot"
    # tempinv = badd.inv
    # badd.inv=[]

    # for i in tempinv:
    #    badd.inv.append(it.createItem(i))

    g.gwin.button0["command"] = lambda: g.initSelect(display, baddie, "inv", "itemType", 'loot', 'dispTown')  # todo
