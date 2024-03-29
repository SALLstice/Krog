import csv
import pickle as p
import random as r

from django.shortcuts import render

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

    def useSkill(self, skill, mod=0):
        if not hasattr(self, skill):
            setattr(self, skill, 0)

        skillLevel = getattr(self, skill)

        result = r.randrange(1,101) + skillLevel + mod

        if r.randrange(1,101) >= skillLevel and  skillLevel < 100:
                setattr(self, skill, skillLevel + 1)

        return result
    
    def strength(self):
        effStr = max(0, self.baseStrength - int(self.hunger / 10))
        return effStr

    def speed(self):
        effSpeed = max(0, self.baseSpeed - int(self.timeAwake / 10))
        return effSpeed

class personType:
    def __init__(self, *args, **kwargs):
        for each in PERSON_HEADERS:
            setattr(self, each, args)

class dead:
    def __init__(self, entityID, personType, name, inv, deathDate, deathLocation):
        self.entityID = entityID
        self.personType = personType
        self.name = name
        self.inv = inv
        self.deathDate = deathDate
        self.deathLocation = deathLocation

class player(person):
    def __init__(self, name, location, skills, magic):
        self.name = name
        self.location = location
        self.skills = skills
        self.magic = magic
        self.timeAwake = 0
        self.sleeping = False
        self.hunger = 0


def peopleLayout(request):
    varDict = {'content': 'this came from people'}
    return render(request, 'krog/personLayout.html', varDict)

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
    global me
    # for i in range(len(inv[0])):
    #    inv[0][i]=it.createItem(inv[0][i])
    print("enter name")
    g.setText(label4="Enter your name:")
    g.gwin.button0["text"] = "Confirm"
    g.gwin.button0["command"] = lambda: g.setName(g.gwin.textInput.get())

    if race == 'human':
        me = player("", loc, skills(), magic())

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
        setattr(me, "maxHPGain", 0)
        setattr(me, "baseStrength", 2)
        setattr(me, "strengthGain", 0)
        setattr(me, "tough", 2)
        setattr(me, "toughGain", 0)
        setattr(me, "overlandSpeed", 3)
        setattr(me, "overlandSpeedGain", 0)
        setattr(me, "TIBS", 50)
        setattr(me, "baseSpeed", 3)
        setattr(me, "speedGain", 0)

    def setMagic():
        pass

    def setSkills():
        setattr(me.skills, "Dodge", 10)
        setattr(me.skills, "Club", 50)
        setattr(me.skills, "Dissection", 10)

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
    

def createPerson(pTID, number=1, name='null', currentHP=-500, location=-1, homeLocation=-1):
    multiAdd = []
    for i in range(number):
        inv = []

        if currentHP == -500:
            currentHP = personTypeList[pTID].maxHP

        if name == 'null':  #if the person has no name (like a krog), make the name the person type
            setname = personTypeList[pTID].personType
        elif name == 'rand':
            setname = w.randomName('city')  # TODO update to peopel names
        else:
            setname = name

        persons.append(person(int(len(persons)), personTypeList[pTID], setname, currentHP, 0, location, homeLocation))
        for val, attr in enumerate(list(personTypeList[0].__dict__.keys())):
            inv = []
            #if attr == 'busy':
            #    setattr(persons[len(persons) - 1], attr, False)

            if attr == "inv" or attr == "addInv":
                invList = getattr(personTypeList[pTID], attr)
                if invList != 0:
                    for i in invList:
                        inv.append(it.createItem(i))
                setattr(persons[len(persons) - 1], attr, inv)

            elif attr == 'money':
                setattr(persons[len(persons) - 1], attr, 0)

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

def statBoostCheck(stat, mod=0):
    #TODO set maximum on stats
    #TODO add player announcement for stat increases
    if (stat == "str"):
        randnum = r.randint(1,101)
        if randnum <= me.strengthGain:
            me.baseStrength += 1
            me.strenghGain = 0
        else:
            me.strengthGain += 1
    elif (stat == "tough"):
        randnum = r.randint(1,101)
        if randnum <= me.toughGain:
            me.tough += 1
            me.toughGain = 0
        else:
            me.toughGain += 1
    elif (stat == "speed"):
        randnum = r.randint(1,201)
        if randnum <= me.speedGain:
            me.baseSpeed += 1
            me.speedGain = 0
        else:
            me.speedGain += 1
    elif (stat == "maxHP"):
        randnum = r.randint(1,51)
        if randnum <= me.maxHPGain:
            me.maxHP += 1
            me.maxHPGain = 0
        else:
            me.maxHPGain += 1
    elif (stat == "overlandSpeed"):
        randnum = r.randint(1,101)
        if randnum <= me.overlandSpeedGain:
            me.overlandSpeed += 1
            me.overlandSpeedGain = 0
        else:
            me.overlandSpeedGain += 1

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