import random as r
import os
import time as t
import gui as g
import people as pe
import times as ti

baddie = -1

class statusEffect:
    def __init__(self, name, effect, effectValue, secondaryValue, duration, timedEffect=0):
        self.name = name
        self.effect = effect
        self.effectValue = effectValue
        self.secondaryValue = secondaryValue
        self.duration = duration
        self.timedEffect = timedEffect

"""
class activeStatus:
    def __init__(self, statusEffect, duration, effectStrength):
        self.statusEffect = statusEffect
        self.duration = duration
        self.effectStrength = effectStrength
"""
def initCombat(tempBaddie):
    global baddie

    g.gwin.button1.grid()
    g.gwin.button2.grid()
    g.gwin.button3.grid()

    baddie = tempBaddie
    setattr(baddie, 'TIBS', 0)
    setattr(baddie, 'status', [])
    setattr(baddie, 'atk', 0)

    g.setText(label5=baddie.name +": " + str(baddie.currentHP))
    baddie.TIBS, baddie.atk = readyMonsterAttack()
    playerTurn()

def readyPlayerAttack():
    if pe.me.attackType == "Q":
        pe.me.TIBS = 30
    if pe.me.attackType == "N":
        pe.me.TIBS = 50
    if pe.me.attackType == "S":
        pe.me.TIBS = 60
    if pe.me.attackType == "A":
        pe.me.TIBS = 75

    tickUntilTurn()

def readyMonsterAttack():
    global gwin

    #monsterActions = [5, 35, 100], [10, 5, 2], [75, 60, 30], [-10, 0, 5], ["powerful attack!", "strong attack.", "fast attack."]
    actionSelect = r.randrange(100)
    idx, monsterAttack = 0, 0

    for idx, actionRate in enumerate(baddie.atkRate):
        if actionSelect <= actionRate:
            baddie.atk = idx
            break

    g.setText(label4=f"The monster is making a {baddie.atkDesc[idx]} attack.")
    g.gwin.button0["text"] = "Continue"
    g.gwin.update()
    baddie.TIBS = baddie.atkTIBS[idx]
    g.gwin.button0["command"] = lambda: tickUntilTurn()

    return baddie.TIBS, monsterAttack

def tickUntilTurn():
    global gwin

    g.clearText([8])

    if pe.me.retreating:
        g.setText(label2="You are running away.")
    if pe.me.dodging:
        g.setText(label2="You are dodging.")
    if pe.me.blocking:
        g.setText(label2="You are blocking.")

    while pe.me.TIBS > 0 and baddie.TIBS > 0:
        g.updateStatus()
        tempSpeedMod = 0

        Ticker = "..................................................................................................."

        for each in baddie.status:
           if each.effect == "slow":
               tempSpeedMod += each.effectValue
        #TODO combine both status checks

        modSpeed = max(1, int(baddie.speed) + tempSpeedMod)

        pe.me.TIBS -= (pe.me.speed + pe.me.equippedWeapon.secondaryEffectValue)

        if 'stop' not in [o.effect for o in baddie.status]:
            baddie.TIBS -= modSpeed

        pe.me.TIBS = max(0, pe.me.TIBS)
        baddie.TIBS = max(0, baddie.TIBS)

        Ticker = Ticker[0:pe.me.TIBS] + "P" + Ticker[pe.me.TIBS:len(Ticker)]
        Ticker = Ticker[0:baddie.TIBS] + "K" + Ticker[baddie.TIBS:len(Ticker)]

        g.gwin.TIBSlabel["text"] = Ticker

        g.setText(label5=baddie.name + ": " + str(baddie.currentHP))
        g.gwin.update()

        if len(baddie.status) == 0:
            g.setText(label6="")
        else:
            for eff in baddie.status:
                # TODO re-add
                if eff.effect == "poison":
                    eff.timedEffect -= 1
                    if eff.timedEffect <= 0:
                        eff.timedEffect = eff.secondaryValue
                        damageBaddie(eff.effectValue)
                        g.setText(label8=f"The monster takes {eff.effectValue} damage from poison")

                eff.duration -= 1
                if eff.duration <= 0:
                    baddie.status.remove(eff)
            g.setAllText(6, [o.name + str(o.duration) for o in baddie.status])

        t.sleep(.1)

    if int(baddie.currentHP) <= 0:
        killedTheMonster(baddie)

    if baddie.TIBS <= 0:
        baddie.TIBS, monsterAction = monsterTurn()
    elif pe.me.TIBS <= 0:
        if pe.me.blocking or pe.me.dodging:
            playerTurn()
        else:
            attack()

def tickUntilStatusClear(monsterStatus):
    poisonDamage = 0

    while len(monsterStatus) > 0:
        for each in monsterStatus:
            if each.statusEffect.effect == "poison":
                if r.randrange(100) < each.statusEffect.secondaryValue:
                    poisonDamage += r.randrange(int(each.statusEffect.effectValue / 2), each.statusEffect.effectValue)
            # TODO apply damage here
            # TODO check if monster dies
            # TODO option to return to loot

            each.duration -= 1
            if each.duration < 0:
                monsterStatus.remove(each)

    if poisonDamage >= 1:
        print(f"The monster takes {poisonDamage} damage from poison")

def monsterTurn():
    atk = baddie.atk

    if pe.me.dodging:
        dodging = int(pe.me.skills.Dodge/2)
    else:
        dodging = 0

    if pe.me.blocking:
        blocking = 2 + 2*pe.me.equippedShield.baseEffectValue
    else:
        blocking = 0

    #TODO blocking skill

    dodgeChance = pe.me.skills.Dodge - int(pe.me.equippedArmor.secondaryEffectValue) + dodging
    hit = r.randrange(100) + baddie.atkMod[atk]

    blockedDamage = int(pe.me.equippedArmor.baseEffectValue) - pe.me.equippedShield.baseEffectValue - blocking
    damage = r.randrange(baddie.atkStr[atk]) + baddie.strength - blockedDamage

    g.setText(label1="The monster attacks!")

    if hit >= dodgeChance:
        if damage <= 0:
            g.setText(label2="You block all damage")
        elif damage > 0:
            g.setText(label2=f"you take {damage}")
            g.updateStatus()
            pe.me.currentHP -= damage

            #g.gwin.HPLabel["text"]=f"HP: {pe.me.currentHP}/{pe.me.maxHP}"
            if pe.me.currentHP <= 0:
                playerDied()
            elif r.randrange(100) >= pe.me.skills.Dodge:
                pe.me.skills.Dodge += 1
                g.setText(label3=f"Your Dodge skill increases to {pe.me.skills.Dodge}!")

    else:
        g.setText(label2="You dodge")

    g.gwin.button0["text"] = "Continue"
    g.gwin.button0["command"] = lambda: readyMonsterAttack()
    g.gwin.update()

    return baddie.atkTIBS[atk], atk

def playerTurn():
    global baddie

    pe.me.blocking = False
    pe.me.dodging = False

    if pe.me.retreating:
        pe.me.retreating = False
        g.setText(label1="You escape.")
        g.gwin.button0["text"] = "Return to Town"
        g.gwin.button0["command"] = lambda:g.dispTown()
        #TODO re-add tickUntilStatusClear(baddie)
    else:
        g.setText(label7="Choose your action")

        if pe.me.attackType == "Q":
            g.gwin.button0["text"] = "Quick Attack"
        elif pe.me.attackType == "S":
            g.gwin.button0["text"] = "Strong Attack"
        elif pe.me.attackType == "N":
            g.gwin.button0["text"] = "Normal Attack"
        elif pe.me.attackType == "A":
            g.gwin.button0["text"] = "Accurate Attack"
        g.gwin.button0["command"] = lambda: readyPlayerAttack()

        g.gwin.button1["text"] = "Defend"
        g.gwin.button1["command"] = lambda : defend()
        g.gwin.button2["text"] = "Items"
        g.gwin.button2["command"] = lambda : g.initSelect('combat use:', pe.me, 'inv', 'name', 'use', 'combat')
        #FIXME cancelling using an item sends you back to town
        g.gwin.button3["text"] = "Tactics"
        g.gwin.button3["command"] = lambda: tactics()

def setupAttackType():
    g.gwin.button0["text"] = "Quick Attack"
    g.gwin.button0["command"] = lambda:setAttackType("Q")

    g.gwin.button1["text"] = "Normal Attack"
    g.gwin.button1["command"] = lambda:setAttackType("N")

    g.gwin.button2["text"] = "Strong Attack"
    g.gwin.button2["command"] = lambda:setAttackType("S")

    g.gwin.button3["text"] = "Accurate Attack"
    g.gwin.button3["command"] = lambda:setAttackType("A")

def setAttackType(aT):
    pe.me.attackType = aT
    playerTurn()

def attack():
    # TODO put this at top of every func becuase double clicking can double attack
    g.gwin.button0["command"] = ""

    eqWep = pe.me.equippedWeapon

    try:
        attackSkill = getattr(pe.me.skills, eqWep.itemType)
    except:
        setattr(pe.me.skills, eqWep.itemType, 0)
        attackSkill = getattr(pe.me.skills, eqWep.itemType)

    if pe.me.attackType == "A":
        attackBonus = 20
    else:
        attackBonus = 0

    hitChance = attackSkill + eqWep.secondaryEffectValue + attackBonus # - monsterDodge #TODO

    damage = 0

    g.setText(label1="You attack!")

    #TODO making strong attacks has chance of increasing str.
    #TODO making quick attacks has chance in incresing TIBS Spd

    if r.randrange(100) <= hitChance:
        damage = r.randrange(int(eqWep.baseEffectValue / 2), eqWep.baseEffectValue) + pe.me.strength
        if pe.me.attackType == "S":
            damage += pe.me.strength
            pe.statBoostCheck("str")
        if pe.me.attackType == "Q":
            damage -= pe.me.strength
            pe.statBoostCheck("speed")
        g.setText(label2=f"You deal {damage}")
        damageBaddie(damage)

        if eqWep.magicEffect == "Freezing":
            if r.randrange(100) <= eqWep.magicEffectValue:
                baddie.status.append(statusEffect("Held", 'stop', 0, 0, 30))

        if eqWep.magicEffect == "Icy":
            if r.randrange(100) <= 100:
                baddie.status.append(statusEffect("Slow", 'speed', -3, 20, 10))
        """
        if equippedWeapon.specialEffect == "Firey":
            if r.randrange(100) <= 40:
                temp = equippedWeapon.specialEffectValue
                print(f"You deal an additional {r.randrange(int(temp / 2), temp)} fire damage.")
        """
        #if pe.me.attackType == "Q":
        #    pe.me.TIBS = 30
        #if pe.me.attackType == "S":
        #    pe.me.TIBS = 70
    else:
        g.setText(label1="You miss")
        #pe.me.TIBS = 25
        #if pe.me.attackType == "S":
        #    pe.me.TIBS += 10
        #TODO update to UseSkill
        if r.randrange(100) >= getattr(pe.me.skills, eqWep.itemType):
            setattr(pe.me.skills, eqWep.itemType, getattr(pe.me.skills, eqWep.itemType) + 1)
            g.setText(label2=f"Your {eqWep.itemType} skill increases to {getattr(pe.me.skills, eqWep.itemType)}!")

            # TODO cap at 100

    if int(baddie.currentHP) > 0:
        playerTurn()
    elif int(baddie.currentHP) == 0:
        killedTheMonster(baddie)

def damageBaddie(damage):
    baddie.currentHP -= damage
    baddie.currentHP = max(0, baddie.currentHP)
    ti.createEvent(ti.now(), pe.me.name, 'wounds', baddie, pe.me.location, extra=damage)

def defend():
    g.gwin.button0["text"] = "Dodge"
    g.gwin.button0["command"] = lambda:dodge()

    g.gwin.button1["text"] = "Block"
    g.gwin.button1["command"] = lambda:block()

    g.gwin.button2["text"] = "Return"
    g.gwin.button2["command"] = lambda:playerTurn()

def dodge():
    pe.me.dodging = True
    pe.me.TIBS = 30
    tickUntilTurn()

def block():
    pe.me.blocking = True
    pe.me.TIBS = 30
    tickUntilTurn()

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
    #TODO clear Ticker
    display = f"You killed the {badd.name}!"
    ti.createEvent(ti.now(),pe.me.name, 'kills',badd, pe.me.location)
    g.setText(label3=display)

    g.gwin.button0["text"] = "Loot"
    g.gwin.button0["command"] = lambda: g.initSelect(display, baddie, "inv", "itemType", 'loot', 'dispTown')

    g.gwin.button1['text'] = 'Dissect'
    g.gwin.button1['command'] = lambda:dissect(badd)

def playerDied():
        g.clearText()
        g.setText(label4="You died!")

        if os.path.exists("player.kr"):
            os.remove("player.kr")
        
        g.gwin.button0["text"] = "Start New"
        g.gwin.button0["command"] = "" #TODO
        g.gwin.button1["text"] = "Quit"
        g.gwin.button1["command"] = lambda:g.quitGame()
        #TODO delete player.kr

def dissect(baddy):
    import items as it
    #TODO combine dissect and loot into single menu
    #TODO clicking Dissect multiple times kinda works sometimes??
    for each in baddy.addInv:
        testrand = r.randrange(100)
        if pe.skillCheck('Dissection', each.harvestDifficulty):
            baddy.inv.append(each)
            baddy.addInv.remove(each)
            print(f'add {each.name}')
            ti.timePasses(each.harvestTime)
        else:
            print(f'destroyed {each.name}')
            baddy.addInv.remove(each)
            temptime = int(each.harvestTime/2)
            if temptime > 0:
                ti.timePasses(temptime)