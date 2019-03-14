import csv
import items as it

persons = []
personTypeList = []

class person:
    def __init__(self, entityID, personType, name, inv, stats, currentHP):
        self.entityID = entityID
        self.personType = personType
        self.name = name
        self.inv = inv
        self.stats = stats
        self.currentHP = currentHP

class player:
    def __init__(self,name,location,currentHP,maxHP, strength, tough, overlandSpeed, inv):
        self.name = name
        self.location = location
        self.currentHP = currentHP
        self.maxHP = maxHP
        self.strength = strength
        self.tough = tough
        self.overlandSpeed = overlandSpeed
        self.inv = inv

def createPlayer(name,location, currentHP,maxHP, strength, tough, overlandSpeed,inv):
    global me
    for i in range(len(inv[0])):
        inv[0][i]=it.createItem(inv[0][i])
    for j in range(len(inv[1])):
        inv[1][j]=it.createItem(inv[1][j])
    me = player(name,location, currentHP,maxHP, strength, tough, overlandSpeed,inv)

#create personList from file
with open('personList.txt') as f:
    for line in f:
        row = eval(line)
        personTypeList.append(person(int(len(personTypeList)),row[0],row[1],row[2],row[3],row[4]))

#generates personEntityList
def createPerson(pTID, number = 1, inv = -500, name = 'null', currentHP = -500):
    multiAdd = []
    inven = []
    for i in range(number):
        if currentHP == -500:
            currentHP = personTypeList[pTID].stats[0]
        if inv == -500:
            inven = personTypeList[pTID].inv        #todo items are never created. Thats probably wrong?
        if inv != -500:
            for j in range(len(row[2])):
                inven.append(it.createItem(row[2][j]))
        if personTypeList[pTID].name == 'null':                     #if the person has no name (like a krog), make the name the person type
            name = personTypeList[pTID].personType

        persons.append(person(int(len(persons)),
                              personTypeList[pTID].personType,
                              name,
                              inven,
                              personTypeList[pTID].stats,
                              currentHP))
        multiAdd.append(persons[len(persons)-1].entityID)

    if number == 1:
        return [persons[int(len(persons) - 1)].entityID]
    if number > 1:
        return multiAdd

def playerInfo():
    print("Name:",me.name)
    print("HP:",me.currentHP, "/", me.maxHP)
    print("Strength:", me.strength)
    print("Toughness:", me.tough)
    print("Overland Speed:", me.overlandSpeed)

    return