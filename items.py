import csv
import random as r

import gui as g
import newCombat as c
import people as pe
import places as pl

itemTypeList = []
mat = 0
items = []
ITEM_HEADERS = []

class item:
    def __init__(self, type, name=None, magicEffect="", magicEffectValue=""):
        self.typeID = int(type)

        if name is None:
            self.name = itemTypeList[int(type)].itemType
        else:
            self.name = name

        self.magicEffect = magicEffect
        self.magicEffectValue = magicEffectValue

class itemType:
    def __init__(self, *args, **kwargs):
        for each in ITEM_HEADERS:
            setattr(self, each, args)
    """
    def __init__(self, itemType, use, cost, baseEffectValue, secondaryEffectValue, special, specialValue, recipe):
        self.itemType = itemType
        self.use = use
        self.cost = cost
        self.baseEffectValue = baseEffectValue
        self.secondaryEffectValue = secondaryEffectValue
        self.special = special
        self.specialValue = specialValue
        self.recipe = recipe
    """


def initMaterials():
    with open('materials.csv') as mib:
        reader = csv.reader(mib)
        matHeaders = next(reader)

        for row in reader:
            # materialList.append(itemType(row))
            for val, attr in enumerate(matHeaders):
                try:
                    tempval = int(row[val])
                except:
                    if attr in ["recipe", "craftMats"]:
                        tempval = eval(row[val])
                        # tempval = [int(x) for x in tempval]
                    else:
                        tempval = row[val]

                # setattr(materialList[len(materialList) - 1], attr, tempval)

def initItemTypeList():
    global ITEM_HEADERS

    with open('itemList.csv') as fib:
        reader = csv.reader(fib)
        ITEM_HEADERS = next(reader)

        for row in reader:
            # itemTypeList.append(itemType(*headers))
            itemTypeList.append(itemType(row))
            for val, attr in enumerate(ITEM_HEADERS):
                if type(attr) == str:
                    attr = attr.strip()

                try:
                    tempval = int(row[val])
                except:
                    if attr in ["recipe"]:
                        tempval2 = row[val].split()
                        tempval = [int(x) for x in tempval2]
                    elif attr in ['craftMats']:
                        tempval = eval(row[val])
                    else:
                        tempval = row[val].strip()

                setattr(itemTypeList[len(itemTypeList) - 1], attr, tempval)


def ref(iTID):
    if type(iTID) == str:
        for idx, iT in enumerate(itemTypeList):
            if iTID == iT.itemType:
                iTID = idx
                break

    return itemTypeList[iTID]


def createItem(iTID, **kwargs):

    if type(iTID) == str:
        for idx, iT in enumerate(itemTypeList):
            if iTID == iT.itemType:
                iTID = idx
                break

    if iTID != 0:
        items.append(item(iTID))
        setattr(items[len(items) - 1], 'itemEntityID', len(items) - 1)
        for val, attr in enumerate(list(itemTypeList[0].__dict__.keys())):
            temp1 = items[len(items) - 1]
            temp2 = itemTypeList[iTID]
            setattr(temp1, attr, getattr(temp2, attr))
        for inputAttr in kwargs.keys():
            setattr(items[len(items) - 1], inputAttr, kwargs.get(inputAttr))

        return items[int(len(items) - 1)]

    if iTID == 0:
        return itemTypeList[0]

def createItemAt(itemTypeID, loc, name='0', desc='0'):
    return()
    #todo creates an item in a specific inventory

def inventory(holder, do, returnTo, sellTo=""):
    g.clearText()
    display = "Items in your bag:"  # todo inventory limit

    money = pe.me.money
    g.setText(label9=f"You have {str(int((money / 10000)))}  gold, {str(int(money / 100) % 100)} silver, {str(money % 100)} copper.")

    g.initSelect(display, pe.me, 'inv', 'name', do, returnTo, sellTo=sellTo, doNotClear=[9])

def displayItems(store):
    # Display items to buy
    g.clearText()
    # g.setText(label0=f"--- {store.type} Stock ---")
    # g.setText(label3=f"  -Item-\t\t\t\t-Strength-\t\t-Cost-")
    display = f"--- {store.type} Stock ---"
    g.initSelect(display, store, 'inv', ['name', 'cost'], 'buy', 'town')

def buyItem(store, selection):
    if pe.me.money >= store.stocks[selection - 1].item.cost and len(store.stocks[selection - 1].entities) >= 1:
        pe.me.money -= store.stocks[selection - 1].item.cost
        pe.me.inv.append(store.stocks[selection - 1].entities.pop(0))
    elif pe.me.money < store.stocks[selection - 1].item.cost:
        g.clearText()
        g.setText(label4="You can't afford it.")
        g.gwin.button0["text"] = "Return"
        g.gwin.button0["command"] = pl.arrive()
    elif len(store.stocks[selection - 1].entities) <= 0:
        g.clearText()
        g.setText(label4="Out of stock.")
        g.gwin.button0["text"] = "Return"
        g.gwin.button0["command"] = pl.arrive()

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

        if itemToUse.special == "poison" and returnTo == "combat":
            c.baddie.status.append(c.statusEffect("Poisoned", 'poison', itemToUse.baseEffectValue, itemToUse.secondaryEffectValue, itemToUse.specialValue, itemToUse.secondaryEffectValue))
        #todo change this to be more generic. attr for target, then use apply status effect of item special effect

    elif itemToUse.use == "weapon":
        pe.me.inv.append(pe.me.equippedWeapon)
        pe.me.equippedWeapon = itemToUse
        g.gwin.eqWeapL["text"] = f"Wea: {pe.me.equippedWeapon.name}"
    elif itemToUse.use == "armor":
        pe.me.inv.append(pe.me.equippedArmor)
        pe.me.equippedArmor = itemToUse
        g.gwin.eqArmL["text"] = f"Arm: {pe.me.equippedArmor.name}"

    pe.me.inv.remove(itemToUse)

    if returnTo == 'town':
        g.dispTown()
    elif returnTo == 'combat':
        pe.me.TIBS = 40
        c.tickUntilTurn()
        #todo check this is right

def sortInventory():
    #todo
    ...

def loot(baddie, selection):
    pe.me.inv.append(baddie.inv.pop(selection - 1))
    return baddie

def removeItemsFromInv(numToRemove, itemToRemove):
    count = 0
    itemsToRemove = []

    if type(itemToRemove) == int:
        itemToRemove = itemTypeList[itemToRemove].itemType

    for itms in pe.me.inv:
        if itms.itemType == itemToRemove:
            itemsToRemove.append(itms)
            count += 1
        if count == numToRemove:
            break
    for each in itemsToRemove:
        pe.me.inv.remove(each)

def brew(store, brewing):
    g.clearText()
    itemsNeeded = []

    for num in range(int(len(store.recipes[brewing].recipe)/2)):
        itemsNeeded.append([store.recipes[brewing].recipe[num], store.recipes[brewing].recipe[num+1]])
        num+=1
    g.setText(label4= "Give me some things and I can make it.") #todo make this good

    #todo have brewing cost money too
    #todo have option to brew yourself, with skill potential for failure

    #set button0 first, to craft as through there are enough items. CheckInvForItems will change it if needed
    g.gwin.button0["command"] = lambda:craftItem(store.recipes[brewing])
    checkInvForItems(itemsNeeded)

def craftItem(itemToCraft):
    #todo have crafting take time
    #todo have option to half-create an item, tracking how much crafting time is left
    for num in range(int(len(itemToCraft.recipe) / 2)):
        removeItemsFromInv(itemToCraft.recipe[num],itemToCraft.recipe[num+1])
        num+=1
    pe.me.inv.append(createItem(itemToCraft.itemType))
    g.clearText()
    g.setText(label4=f"You craft a {itemToCraft.itemType}")
    g.gwin.button0['text']="Continue"
    g.gwin.button0['command']=lambda:pl.arrive()

def countItem(it, lst):
    count = 0
    for item in lst:
        if it == item:
            count += 1
    return count

def condenseList(lst):
    return [[countItem(o, lst), o] for o in lst]
    #todo turn list into set

def checkInvForItems(itemToSearch, numItems=999):
    itemCount = 0
    haveEnough = True

    if type(itemToSearch) == list:
        #tl = condenseList(itemToSearch)
        for each in itemToSearch:
            numItems = each[0] #todo make it work for multiple ingrediates
            itemToSearch = itemTypeList[each[1]].itemType

            for itm in pe.me.inv:
                if itm.itemType == itemToSearch:
                    itemCount += 1  # list of itemID of every krog guts
            if itemCount < numItems:
                haveEnough = False

    g.setText(label6=f"You have {itemCount} {itemToSearch}")
    if haveEnough:
        g.setText(label7="Hand them over?")

        g.gwin.button0["text"]="Yes"

        g.gwin.button1["text"]="No"
        g.gwin.button1["command"] = lambda:pl.arrive()
        g.gwin.button1.grid()
    else:
        g.setText(label7 = f"You don't have enough {itemToSearch}.")
        g.gwin.button0.grid_remove()
        g.gwin.button1["text"]="Return"
        g.gwin.button1["command"]=pl.arrive()
