import csv

import gui as g
import items as it
import worlds as w

persons = []
personTypeList = []
futureDead = []


class skill:
    def __init__(self, name, level, grow):
        self.name = name
        self.level = level
        self.grow = grow

class person:
    def __init__(self, entityID, perT, name, currentHP, status):
        self.entityID = entityID
        self.personType = perT
        self.name = name
        self.currentHP = currentHP
        self.status = status


class personType:
    def __init__(self, personType, inv, strength, maxHP, defense, dodge, speed, event, eventTimer):
        self.personType = personType
        self.inv = inv
        self.strength = strength
        self.maxHP = maxHP
        self.defense = defense
        self.dodge = dodge
        self.speed = speed
        self.event = event
        self.eventTimer = eventTimer

class dead:
    def __init__(self, entityID, personType, name, inv, deathDate, deathLocation):
        self.entityID = entityID
        self.personType = personType
        self.name = name
        self.inv = inv
        self.deathDate = deathDate
        self.deathLocation = deathLocation


class boss:
    def __init__(self, name, stats, currentHP, location, attackTarget, sleepTimer):
        self.name = name
        self.stats = stats
        self.currentHP = currentHP
        self.location = location
        self.attackTarget = attackTarget
        self.sleepTimer = sleepTimer

class player:
    def __init__(self, name, location, skills):
        self.name = name
        self.location = location
        self.skills = skills


with open('personList.csv') as f:
    reader = csv.reader(f)
    headers = next(reader)
    for row in reader:
        personTypeList.append(personType(*headers))
        for val, attr in enumerate(headers):
            try:
                tempval = int(row[val])
            except:
                if attr == "inv":
                    tempval = row[val].split()
                    tempval = [int(x) for x in tempval]
                else:
                    tempval = row[val]

            setattr(personTypeList[len(personTypeList) - 1], attr, tempval)
    f.close()


def createPlayer(race, name, loc):
    global me  # todo give player skills which grow as they use the skill more often
    # for i in range(len(inv[0])):
    #    inv[0][i]=it.createItem(inv[0][i])

    if race == 'human':
        me = player(name, loc, skill)

    def setFlags():
        setattr(me, "retreating", False)
        setattr(me, "attackType", "Q")

    def setStats():
        setattr(me, "currentHP", 10)
        setattr(me, "maxHP", 10)
        setattr(me, "strength", 2)
        setattr(me, "tough", 2)
        setattr(me, "overlandSpeed", 3)
        setattr(me, "TIBS", 50)
        setattr(me, "speed", 3)

    def setSkills():
        setattr(me.skills, "Dodge", 10)
        setattr(me.skills, "Short Sword", 40)

    def setInventory(ininv):
        inv = []

        for j in range(len(ininv)):
            inv.append(it.createItem(ininv[j]))

        setattr(me, "inv", inv)
        setattr(me, "money", 1000000)

    def setEquipment():
        setattr(me, "equippedWeapon", it.createItem(14))
        setattr(me, "equippedShield", it.createItem(0))
        setattr(me, "equippedArmor", it.createItem(2))
        setattr(me, "equippedAcc1", it.createItem(0))
        setattr(me, "equippedAcc2", it.createItem(0))

    setFlags()
    setStats()
    setSkills()
    setInventory([5, 5, 5, 1])
    setEquipment()


def createBoss(name, stats, currentHP, location, status):
    global kingKrog
    kingKrog = boss(name, stats, currentHP, location, -1, status)


def createPerson(pTID, number=1, name='null', currentHP=-500):
    multiAdd = []
    for i in range(number):
        inv = []

        if currentHP == -500:
            currentHP = personTypeList[pTID].maxHP

        if name == 'null':  #if the person has no name (like a krog), make the name the person type
            name = personTypeList[pTID].personType

        persons.append(person(int(len(persons)), personTypeList[pTID], name, currentHP, 0))
        for val, attr in enumerate(list(personTypeList[0].__dict__.keys())):
            if attr == "inv":

                invList = getattr(personTypeList[pTID], attr)
                if invList != 0:
                    for i in invList:
                        inv.append(it.createItem(i))
                setattr(persons[len(persons) - 1], "inv", inv)
            else:
                setattr(persons[len(persons) - 1], attr, getattr(personTypeList[pTID], attr))

        multiAdd.append(persons[len(persons) - 1])

    if number == 1:
        return persons[int(len(persons) - 1)]
    if number > 1:
        return multiAdd

def createBody(e):
    persons.append(person(int(len(persons)),
                          personTypeList[2].personType,
                          futureDead[e.person].name,
                          futureDead[e.person].inv,
                          0,
                          0,
                          0,
                          0,
                          0))

    w.world.nodes[e.location]['monsters'].append(persons[int(len(persons) - 1)].entityID)


def dispSkills():
    skillList = []
    skil = ""
    for skil in me.skills.__dict__.keys():
        if skil[0:2] != "__":
            skillList.append(f"{skil}: {getattr(me.skills, skil)}")
    g.setAllText(1, skillList)
