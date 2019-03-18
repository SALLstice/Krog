import people as pe

itemTypeList = []
items = []

class item:
    def __init__(self, entityID, itemType, equip, combatValue, cost, effect, effectValue, name, desc):
        self.entityID = entityID
        self.itemType = itemType
        self.equip = equip
        self.combatValue = combatValue
        self.cost = cost
        self.effect = effect
        self.effectValue = effectValue
        self.name = name
        self.desc = desc


#create itemList from file
with open('itemList.txt') as f:
    for line in f:
        row = eval(line)
        itemTypeList.append(item(int(len(itemTypeList)),row[0],row[1],int(row[2]),int(row[3]),int(row[4]),int(row[5]),'',''))

#generates items List
def createItem(iTID, name='0', desc='0'):
    if iTID != 0:
        items.append(item(int(len(items)),
                          itemTypeList[iTID].itemType,
                          itemTypeList[iTID].equip,
                          itemTypeList[iTID].combatValue,
                          itemTypeList[iTID].cost,
                          itemTypeList[iTID].effect,
                          itemTypeList[iTID].effectValue,
                          name,
                          desc))
        return items[int(len(items) - 1)].entityID
    if iTID == 0:
        return itemTypeList[0].entityID

def createItemAt(itemTypeID, loc, name='0', desc='0'):
    return()
    #creates an item in a specific inventory todo

def inventory():
    print("\nItems you have equipped:")
    print("Weapon: " + items[pe.me.inv[0][0]].itemType)
    print("Shield: " + items[pe.me.inv[0][1]].itemType)
    print("Armor: " + items[pe.me.inv[0][2]].itemType)
    print("Accessory 1: " + items[pe.me.inv[0][3]].itemType)
    print("Accessory 2: " + items[pe.me.inv[0][4]].itemType)

    print("\nItems in your bag:")       #todo inventory limit
    if len(pe.me.inv[1]) >= 1:
        for i in range(len(pe.me.inv[1])):
            print(items[pe.me.inv[1][i]].itemType)

    print("\nCoin Pouch:")
    money = pe.me.inv[2]
    print("You have " + str(int((money / 10000))) + " gold, " +str(int(money / 100)%100) + " silver, " + str(money % 100) + " copper.")

    whatDo = int(input("\n1: Equip/Use\n2: Unequip\n"))
    if whatDo == 1:
        print("Equip/Use what?")
        for i in range(len(pe.me.inv[1])):
            print(i, items[pe.me.inv[1][i]].itemType)
        invidx = int(input())
        if items[pe.me.inv[1][invidx]].equip >= 0:
            equipItem(pe.me.inv[1][invidx])
        elif items[pe.me.inv[1][invidx]].equip == -1:
            useItem(pe.me.inv[1][invidx])
    elif whatDo == 2:
        print("Unequip what?")
        for i in range(len(pe.me.inv[0])):
            if items[pe.me.inv[0][i]].itemType != '':
                print(i, items[pe.me.inv[0][i]].itemType)
        invidx = int(input())
        equipRegion = items[pe.me.inv[0][invidx]].equip  # find which region the new item equips to
        pe.me.inv[1].append(pe.me.inv[0][equipRegion])        #add unequiped item to invnentory
        pe.me.inv[0][equipRegion] = 0


def sellItem(iE, soldTo):
    pe.me.inv[2] += int(items[iE].cost/2)             #add money for item cost
    soldTo.inv.append(items[iE].entityID)  # add item to store inventory
    pe.me.inv[1].remove(items[iE].entityID)  # remove item from player inventory
    # #todo print what was sold for how much

def buyItem(iE, boughtFrom):
    pe.me.inv[2] -= items[iE].cost #subtract money for item cost
    boughtFrom.inv.remove(items[iE].entityID) #remove item from store inventory
    pe.me.inv[1].append(items[iE].entityID) #add item to player inventory

def equipItem(iEID):
    equipRegion = items[iEID].equip  # find which region the new item equips to
    if pe.me.inv[0][equipRegion] != 0:              #tests to make sure it's not "nothing"
        pe.me.inv[1].append(pe.me.inv[0][equipRegion])  # add unequiped item to invnentory
    pe.me.inv[0][equipRegion] = iEID  # set new item to equip slot
    pe.me.inv[1].remove(iEID)  # remove equipped item from inventory

def useItem(iEID):
    ef = items[iEID].effect

    if ef == 1: #todo check if at full first
        efv = items[iEID].effectValue   #todo add random range to effeect value
        print("The", items[iEID].itemType, "heals you for", efv, ".")
        if pe.me.currentHP + efv >= pe.me.maxHP:
            pe.me.currentHP = pe.me.maxHP
        elif pe.me.currentHP + efv < pe.me.maxHP:
            pe.me.currentHP += efv
    elif ef == 2: #todo ask if wanting to eat poison first. still give option if they want. Lose 1 (max?) hp every hour. check against tough to throw it off. Sleep to regain
        ...
    else:
        ...

    return

def sortInventory():
    #todo
    ...