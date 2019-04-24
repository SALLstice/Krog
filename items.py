import csv
import random as r

import gui as g
import people as pe

itemTypeList = []
items = []

class item:
    def __init__(self, type, name, condition):
        self.type = type
        self.name = name
        self.condition = condition


class itemType:
    def __init__(self, itemType, use, cost, baseEffectValue, secondaryEffectValue, special, specialValue):
        self.itemType = itemType
        self.use = use
        self.cost = cost
        self.baseEffectValue = baseEffectValue
        self.secondaryEffectValue = secondaryEffectValue
        self.special = special
        self.specialValue = specialValue


# create itemTypeList from file
with open('itemList.csv') as f:
    reader = csv.reader(f)
    headers = next(reader)

    for row in reader:
        itemTypeList.append(itemType(*headers))
        for val, attr in enumerate(headers):
            try:
                tempval = int(row[val])
            except:
                tempval = row[val]

            setattr(itemTypeList[len(itemTypeList) - 1], attr, tempval)
    f.close()


#generates items List
def createItem(iTID, name="", condition=""):
    if iTID != 0:
        items.append(item(iTID, name, condition))
        for val, attr in enumerate(list(itemTypeList[0].__dict__.keys())):
            setattr(items[len(items) - 1], attr, getattr(itemTypeList[iTID], attr))
        return items[int(len(items) - 1)]

    if iTID == 0:
        return itemTypeList[0]

def createItemAt(itemTypeID, loc, name='0', desc='0'):
    return()
    #creates an item in a specific inventory todo


def inventory(holder, do, returnTo, sellTo=""):
    g.clearText()
    display = "Items in your bag:"  # todo inventory limit

    money = pe.me.money
    g.setText(label8=f"You have {str(int((money / 10000)))}  gold, {str(int(money / 100) % 100)} silver, {str(
        money % 100)} copper.")

    g.initSelect(display, pe.me, 'inv', 'itemType', do, returnTo, sellTo=sellTo)


def displayItems(store):
    # Display items to buy
    g.clearText()
    # g.setText(label0=f"--- {store.type} Stock ---")
    # g.setText(label3=f"  -Item-\t\t\t\t-Strength-\t\t-Cost-")
    display = f"--- {store.type} Stock ---"
    g.initSelect(display, store, 'inv', 'itemType', 'buy', 'dispTown')


def buyItem(store, selection):
    if pe.me.money >= store.inv[selection - 1].cost:
        pe.me.money -= store.inv[selection - 1].cost
        pe.me.inv.append(store.inv.pop(selection - 1))

    g.dispTown()


def sellItem(store, selection):
    pe.me.money += pe.me.inv[selection - 1].cost
    store.inv.append(pe.me.inv.pop(selection - 1))


def equipItem(iEID):
    equipRegion = items[iEID].equip  # find which region the new item equips to
    if pe.me.inv[0][equipRegion] != 0:              #tests to make sure it's not "nothing"
        pe.me.inv[1].append(pe.me.inv[0][equipRegion])  # add unequiped item to invnentory
    pe.me.inv[0][equipRegion] = iEID  # set new item to equip slot
    pe.me.inv[1].remove(iEID)  # remove equipped item from inventory


def useItem(itemToUse, returnTo):
    if itemToUse.use == "consume":
        if itemToUse.special == "Healing":
            effect = r.randrange(int(itemToUse.specialValue / 2), itemToUse.specialValue)
            g.setText(label3=f"you heal {effect}")
            pe.me.currentHP = max(pe.me.currentHP + effect, pe.me.maxHP)
        # if itemToUse.specialEffect == "Poison":
        #    monsterStatus.append(activeStatus(lightPoison, 10))

    elif itemToUse.use == "weapon":
        pe.me.inv.append(pe.me.equippedWeapon)
        pe.me.equippedWeapon = itemToUse
        g.gwin.eqWeapL["text"] = f"Wea: {pe.me.equippedWeapon.itemType}"
    elif itemToUse.type == "armor":
        pe.me.inv.append(pe.me.equippedArmor)
        pe.me.equippedArmor = itemToUse
        g.gwin.eqArmL["text"] = f"Arm: {pe.me.equippedArmor.itemType}"

    pe.me.inv.remove(itemToUse)

    if returnTo == 'town':
        g.dispTown()

def sortInventory():
    #todo
    ...


def loot(baddie, selection):
    pe.me.inv.append(baddie.inv.pop(selection - 1))
    return baddie
