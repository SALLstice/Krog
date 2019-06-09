import csv
import pickle as p
import random as r

import gui as g
import items as it
import worlds as w

persons = []
personTypeList = []
futureDead = []
PERSON_HEADERS = []

class skills:
    def __init__(self):
        pass

class magic:
    def __init__(self):
        pass

class person:
    def __init__(self, entityID, perT, name, currentHP, status, location, homeLocation):
        self.entityID = entityID
        self.personType = perT
        self.name = name
        self.currentHP = currentHP
        self.status = status
        self.location = location
        # self.busy = False
        self.job = None
        self.wallet = 0
        self.employed = None
        self.homeLocation = homeLocation

class personType:
    def __init__(self, *args, **kwargs):
        for each in PERSON_HEADERS:
            setattr(self, each, args)

    """
    def __init__(self, personType, inv, strength, maxHP, defense, dodge, speed, event, eventTimer, atkRate, atkStr, atkTIBS, atkMod, atkDesc, addInv):
        self.personType = personType
        self.inv = inv
        self.strength = strength
        self.maxHP = maxHP
        self.defense = defense
        self.dodge = dodge
        self.speed = speed
        self.event = event
        self.eventTimer = eventTimer
        self.atkRate = atkRate
        self.atkStr = atkStr
        self.atkTIBS = atkTIBS
        self.atkMod = atkMod
        self.atkDesc = atkDesc
        self.addInv = addInv
    """
class dead:
    def __init__(self, entityID, personType, name, inv, deathDate, deathLocation):
        self.entityID = entityID
        self.personType = personType
        self.name = name
        self.inv = inv
        self.deathDate = deathDate
        self.deathLocation = deathLocation

"""
class boss:
    def __init__(self, name, stats, currentHP, location, attackTarget, sleepTimer):
        self.name = name
        self.stats = stats
        self.currentHP = currentHP
        self.location = location
        self.attackTarget = attackTarget
        self.sleepTimer = sleepTimer
"""
class player:
    def __init__(self, name, location, skills, magic):
        self.name = name
        self.location = location
        self.skills = skills
        self.magic = magic


def initPersonTypeList():
    global PERSON_HEADERS

    with open('personList.csv') as f:
        reader = csv.reader(f)
        PERSON_HEADERS = next(reader)
        for row in reader:
            personTypeList.append(personType(*PERSON_HEADERS))
            for val, attr in enumerate(PERSON_HEADERS):
                if type(attr) == str:
                    attr = attr.strip()

                try:
                    tempval = int(row[val])
                except:
                    if attr in ["inv", "atkRate", "atkStr", "atkTIBS", "atkMod", "addInv"]:
                        tempval2 = row[val].split()
                        tempval = [int(x) for x in tempval2]
                    elif attr == "atkDesc":
                        tempval = row[val].split()
                    else:
                        tempval = row[val].strip()

                setattr(personTypeList[len(personTypeList) - 1], attr, tempval)

def createPlayer(race, loc):
    global me  # todo give player skills which grow as they use the skill more often
    # for i in range(len(inv[0])):
    #    inv[0][i]=it.createItem(inv[0][i])

    g.setText(label4="Enter your name:")
    g.gwin.button0["text"] = "Confirm"
    g.gwin.button0["command"] = lambda: g.setName(g.gwin.textInput.get())

    skill=skills()
    mag=magic()

    if race == 'human':
        me = player("", loc, skill, mag)

    def setFlags():
        setattr(me, "retreating", False)
        setattr(me, "dodging", False)
        setattr(me, "blocking", False)
        setattr(me, "attackType", "N")
        setattr(me, "exploring", 0)
        setattr(me, "travelling", False)
        setattr(me, 'foraging', False)
        setattr(me, 'sneaking', False)

    def setStats():
        setattr(me, "currentHP", 10)
        setattr(me, "maxHP", 10)
        setattr(me, "strength", 2)
        setattr(me, "tough", 2)
        setattr(me, "overlandSpeed", 3)
        setattr(me, "TIBS", 50)
        setattr(me, "speed", 3)

    def setMagic():
        pass

    def setSkills():
        setattr(me.skills, "Dodge", 10)
        setattr(me.skills, "Club", 50)

    def setInventory(ininv):
        inv = []

        for j in range(len(ininv)):
            inv.append(it.createItem(ininv[j]))

        setattr(me, "inv", inv)
        setattr(me, "money", 0)

    def setEquipment():
        setattr(me, "equippedWeapon", it.createItem('Club'))
        setattr(me, "equippedShield", it.createItem(0))
        setattr(me, "equippedArmor", it.createItem('Clothing'))
        setattr(me, "equippedAcc1", it.createItem(0))
        setattr(me, "equippedAcc2", it.createItem(0))

    setFlags()
    setStats()
    setSkills()
    setInventory([5, 5])
    setEquipment()
    setMagic()

def createBoss():
    global kingKrog
    kingKrog = createPerson(4, 1, 'King Krog')

def findBoss():
    global kingKrog

    for baddie in persons:
        if baddie.name == 'King Krog':
            kingKrog = baddie


def createPerson(pTID, number=1, name='null', currentHP=-500, location=-1, homeLocation=-1):
    multiAdd = []
    for i in range(number):
        inv = []

        if currentHP == -500:
            currentHP = personTypeList[pTID].maxHP

        if name == 'null':  #if the person has no name (like a krog), make the name the person type
            setname = personTypeList[pTID].personType
        elif name == 'rand':
            setname = w.randomName('city')  # todo update to peopel names
        else:
            setname = name

        persons.append(person(int(len(persons)), personTypeList[pTID], setname, currentHP, 0, location, homeLocation))
        for val, attr in enumerate(list(personTypeList[0].__dict__.keys())):
            inv = []
            if attr == 'busy':
                setattr(persons[len(persons) - 1], attr, False)

            elif attr == "inv" or attr == "addInv":

                invList = getattr(personTypeList[pTID], attr)
                if invList != 0:
                    for i in invList:
                        inv.append(it.createItem(i))
                setattr(persons[len(persons) - 1], attr, inv)
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

def nameDebugCheck(name):
    if name == "Superman":
        setattr(me.skills, "Dodge", 500)
        setattr(me.skills, "Club", 500)

    if name == "Debug":
        me.skills.Club = 100
        me.skills.Dodge = 100
        me.strength = 20
        me.currentHP = 500

    if name == "Poison":
        me.inv.append(it.createItem('Noxious Elixir'))

    if name == "Hunter":
        setattr(me.skills,"Dissection", 50)

    if name == 'Richboi':
        me.money = 10000000

def savePlayer():
    with open(r"player.kr", "wb") as play:
        p.dump(me, play)
        p.dump(me.skills, play)

def loadPlayer():
    global me

    with open(r"player.kr", "rb") as play:
        me = p.load(play)
        me.skills = p.load(play)

def skillCheck(skill, mod=0):

    skillLevel = getSkill(skill)

    randRoll = r.randrange(100)

    if randRoll <= skillLevel + mod:
        return True
    else:
        skillGainRoll = r.randrange(100)
        if skillGainRoll >= skillLevel:
            setattr(me.skills, skill, skillLevel+1)
            print(skill, skillLevel)
        return False

def getSkill(skill):
    try:
        return getattr(me.skills,skill)
    except:
        setattr(me.skills, skill,0)
        return getattr(me.skills,skill)