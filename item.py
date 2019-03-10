import csv
import ast
import people as pe
import places as pl

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
    items.append(item(int(len(items)), itemTypeList[iTID].itemType,itemTypeList[iTID].equip,itemTypeList[iTID].combatValue,itemTypeList[iTID].cost,itemTypeList[iTID].effect,itemTypeList[iTID].effectValue, name, desc))
    return items[int(len(items) - 1)].entityID

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

    print("\nItems in your bag:")
    for i in range(len(pe.me.inv[1])):
        if items[pe.me.inv[1][i]].itemType != 'null':
            print(items[pe.me.inv[1][i]].itemType)

    print("\nCoin Pouch:")
    money = pe.me.inv[2]
    print("You have " + str(int((money / 10000))) + " gold, " +str(int(money / 100)%100) + " silver, " + str(money % 100) + " copper.")

    whatDo = int(input("\n1: Equip\n2: Unequip\n"))
    if whatDo == 1:
        print("Equip what?")
        for i in range(len(pe.me.inv[1])):
            if items[pe.me.inv[1][i]].itemType != 'null':
                print(i, items[pe.me.inv[1][i]].itemType)
        invidx = int(input())
        equipRegion = items[pe.me.inv[1][invidx]].equip       #find which region the new item equips to
        pe.me.inv[1].append(pe.me.inv[0][equipRegion])           #add unequiped item to invnentory
        pe.me.inv[0][equipRegion] = pe.me.inv[1][invidx]          #set new item to equip slot
        pe.me.inv[1].remove(pe.me.inv[1][invidx])                 #remove equipped item from inventory

    elif whatDo == 2:
        print("Unequip what?")
        for i in range(len(pe.me.inv[0])):
            if items[pe.me.inv[0][i]].itemType != 'null':
                print(i, items[pe.me.inv[0][i]].itemType)
        invidx = int(input())
        equipRegion = items[pe.me.inv[0][invidx]].equip  # find which region the new item equips to

        pe.me.inv[1].append(pe.me.inv[0][equipRegion])        #add unequiped item to invnentory



def sellItem(iE, soldTo):
    pe.me.inv[2] += int(items[iE].cost/2)             #add money for item cost
    soldTo.inv.append(items[iE].entityID)    #
    pe.me.inv[1].remove(items[iE].entityID)      #

def buyItem(iE, boughtFrom):
    pe.me.inv[2] -= items[iE].cost #subtract money for item cost
    boughtFrom.inv.remove(items[iE].entityID) #remove item from store inventory
    pe.me.inv[1].append(items[iE].entityID) #add item to player inventory