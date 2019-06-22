import csv
import random as r
import time

import networkx as nx

import events as ev
import gui as g
import items as it
import newCombat as c
import people as pe
import times as t
import worlds as w

placeTypeList = []
places = []
PLACE_HEADERS = []

class place:
    def __init__(self, name, currentHP):
        self.name = name
        self.currentHP = currentHP

class placeType:
    def __init__(self, *args, **kwargs):
        for each in PLACE_HEADERS:
            setattr(self, each, args)

    '''
    def __init__(self, type, use, area, known, inv, maxHP, extraSiteOption, recipes):
        self.type = type
        self.use = use
        self.area = area
        self.known = known
        self.inv = inv
        self.maxHP = maxHP
        self.extraSiteOption = extraSiteOption
        self.recipes = recipes
    '''


def initPlaceTypeList():
    global PLACE_HEADERS

    with open('placeList.csv') as f:
        reader = csv.reader(f)
        headers = next(reader)
        for row in reader:
            placeTypeList.append(placeType(*headers))
            for idx, attr in enumerate(headers):
                if type(attr) == str:
                    attr = attr.strip()


                try:
                    tempval = int(row[idx])
                except:
                    if attr in ["inv", "recipes"]:
                        tempval = row[idx].split()
                        tempval = [int(x) for x in tempval]
                    else:
                        tempval = row[idx].strip()

                setattr(placeTypeList[len(placeTypeList) - 1], attr, tempval)

def createPlace(sTID, name="", currentHP=-500):  # todo gain ability to reverse lookup sites by location
    inv = []

    if type(sTID) == str:
        sTID = findTID(sTID)

    if currentHP == -500:
        currentHP = placeTypeList[sTID].maxHP

    places.append(place(name, currentHP))
    for val, attr in enumerate(list(placeTypeList[0].__dict__.keys())):
        if attr in ["inv", "recipes"]:
            templist= []
            invList = getattr(placeTypeList[sTID], attr)
            if invList != 0:
                for i in invList:
                    templist.append(it.createItem(i))
            setattr(places[len(places) - 1], attr, templist)
        else:
            setattr(places[len(places) - 1], attr, getattr(placeTypeList[sTID], attr))

    return places[int(len(places) - 1)]

def findTID(inplT):
    if type(inplT) == str:
        for idx, plT in enumerate(placeTypeList):
            if inplT == plT.type:
                plTID = idx
                break

    return plTID

def arrive():
    g.updateStatus()

    if pe.kingKrog.location == pe.me.location:
        g.clearText()
        g.setText(label4="The King Krog Is Here!")
        c.initCombat(pe.kingKrog)
    else:
        g.dispTown()

def visitRegionPlace():  #todo split lodInfo into branch options, shop, rumors, revisit known wild location, town inventory, food stores, etc

    # print("Shop\nRumors\nInfo") #todo make rumors and info
    # g.setText(label4 = f"You are in {loc}.")
    regSites = []
    #todo once you learn Krog count, make that available in loc info. note "As of datetime, there were X Krogs in loc"
                                                                                #prints out every known site in location
    for site in w.world.nodes[pe.me.location]['sites']:
        if site.area == 'town':
            regSites.append(site)
        if site.area == 'wild' and site.known == 1:
            regSites.append(site)

    # dispList = [o.itemType for o in pe.me.inv]
    display = f"You are in {pe.me.location}."
    g.initSelect(display, regSites, "", 'type', 'site', 'town')

    # siteActivity(places[w.world.nodes[loc]['sites'][whereGo]])

def siteActivity(store):
    #Shops
    if store.use == "store":  # todo note how damaged the building if it is damaged
        g.clearText()
        g.setText(label4=f"You are at {store.name} {store.type}.")

        g.gwin.button0["text"] = "Buy"
        g.gwin.button0["command"] = lambda: it.displayItems(store)

        g.gwin.button1["text"] = "Sell"
        g.gwin.button1["command"] = lambda: it.inventory(pe.me, 'sell', 'town', sellTo=store)

        if store.extraSiteOption != "":
            g.gwin.button2["text"] = store.extraSiteOption
            g.gwin.button2["command"] = lambda:siteExtra(store)
        else:
            g.gwin.button3['text'] = "-"
            g.gwin.button3['command'] = ""

        # g.setButtons("Buy","pl.buyItem(store)", "Sell", "sellItem", "Return", "dispTown")
    elif store.use == "druid":
        druid(store)
    elif store.use == "inn":
        inn(store)
    elif store.use == 'witch':
        witch(store)

    def siteExtra(store):
        if store.extraSiteOption == "Brew":
            g.clearText()
            g.initSelect("Brew Potions", store, 'recipes',['itemType'],'brew','town')

def setupTravelRT(dest):
    g.clearText()
    g.gwin.button1.grid_remove()
    g.gwin.button2.grid_remove()
    g.gwin.button3.grid_remove()

    pe.me.exploring = 1
    loc = pe.me.location
    cityname = w.world.nodes[dest]['name']

    if  w.world[loc][dest]['route'].known == 1:
        g.setText(label4=f"Travelling to {cityname}.")
    else:
        g.setText(label4=f"Travelling along {w.world[loc][dest]['route'].desc}.")
    g.gwin.button0["text"] = "Turn Back"
    g.gwin.update()
    g.gwin.button0["command"] = 0 #todo turnback command

    lengthOfTrip = w.world[loc][dest]['route'].length

    #t.createEvent(t.now(), pe.me.name, 'departs', dest, loc)  # create depart event todo make

    encounteredEntity = -1
    timer = 0
    montage = ["", ".", ".  .", ".  .  ."]
    realTimeActivity(timer, 'r', montage, 'travel', dest, lengthOfTrip)

def setupExploreRT():
    pe.me.exploring = 1
    g.setText(label4="Exploring")
    g.gwin.button0["text"] = "Stop Exploring"
    #g.gwin.update()
    g.gwin.button0["command"] = lambda: stopExplore()

    if pe.me.foraging:
        g.gwin.button1['text'] = "Stop Sneaking"
    else:
        g.gwin.button1['text'] = "Sneak"
    g.gwin.button1['command'] = lambda: toggleSneak()

    if pe.me.foraging:
        g.gwin.button2['text'] = "Stop Foraging"
    else:
        g.gwin.button2['text'] = "Forage"
    g.gwin.button2['command'] = lambda:toggleForage()

    #encounteredEntity = -1
    timer = 0
    montage = ["", ".", ".  .", ".  .  ."]
    realTimeActivity(timer, 'rmws', montage, 'explore')

def realTimeActivity(timer, encounterCode, montage, activity, dest=-1, journeyLength=-1, progress=0):
    ENCOUNTER_RATE = 0.25
    encounterType = 0
    encounteredEntity = -1
    FORAGE_ENC_RATE = 0.20
    timerRollover = 4

    #durationOfTrip = max(1,m.ceil(int((w.world[loc][dest]['route'].length) / pe.me.overlandSpeed)*w.world[loc][dest]['route'].roughness))

    time.sleep(0.5)
    g.setText(label5=montage[timer])
    timer += 1
    timer %= timerRollover

    if pe.me.sneaking and timer == timerRollover/2:
        if not pe.skillCheck('Sneaking'):
            t.timePasses()

    if timer == 0:

        t.timePasses()
        modEncRate = ENCOUNTER_RATE

        if pe.me.sneaking:
            modEncRate -= int(pe.getSkill('Sneaking') / 5)

        if pe.me.foraging:
            modEncRate =+ FORAGE_ENC_RATE
            if r.randrange(3) == 0: #easy 33% chance of maybe finding something
                if pe.skillCheck('Foraging'):
                    pe.me.inv.append(it.createItem('Healing Herbs'))
                    g.setText(label8='You find Healing Herbs')

        encounteredEntity, encounterType = randomEncounter(encounterCode, modEncRate)
        # todo increase encounter chance based on number of krog
        #todo have button to flag for forage. disp on label6 item found until timer resets label7 skill increase, increase enc rate
        if activity == 'travel':
            progress += pe.me.overlandSpeed * w.world[pe.me.location][dest]['route'].roughness
            if progress >= journeyLength:
                pe.me.exploring = 2

                w.world[pe.me.location][dest]['route'].known = 1
                pe.me.location = dest
                g.clearText()
                g.setText(label4=f"You walked for {journeyLength} miles and arrive in {w.world.nodes[dest]['name']}.")
                # todo make travel work like ExploreRT
                # todo time passes

                #t.createEvent(t.now(), pe.me.name, 'arrives', dest, dest)  # todo re-add

                g.gwin.button0["text"] = 'Continue'
                g.gwin.button0['command'] = lambda: arrive()

    if encounteredEntity != -1:
        if encounterType == 'm':
            pe.me.exploring = 2
            status = ""
            if encounteredEntity.currentHP <= 0:
                status = "dead "

            elif encounteredEntity.currentHP < encounteredEntity.maxHP:
                status = "wounded "

            g.setText(label4=f"You encounter a {status}{encounteredEntity.name}")
            if status != "dead ":
                g.gwin.button0["text"] = "Enter Combat"
                g.gwin.button0["command"] = lambda: c.initCombat(encounteredEntity)
            else:
                display = "Loot:"
                g.gwin.button0["text"] = "Loot"
                g.gwin.button0["command"] = lambda: g.initSelect(display, encounteredEntity, "inv", "itemType", 'loot', 'dispTown')  # todo

                g.gwin.button1["text"] = "Keep Exploring"
                g.gwin.button1["command"] = lambda: setupExploreRT()
                g.gwin.button1.grid()

                g.gwin.button2["text"] = "Stop Exploring"
                g.gwin.button2["command"] = lambda: g.dispTown()
                g.gwin.button2.grid()

            # combatResult = c.combat(encounteredEntity)
        if encounterType == 's':
            pe.me.exploring = 2
            g.setText(label4=f"You encounter a {encounteredEntity.type}")
            encounteredEntity.known = 1
            g.gwin.button0["text"] = "Continue"
            g.gwin.button0["command"] = lambda:siteActivity(encounteredEntity)
        if encounterType == 'e':
            pe.me.exploring = 2
            ev.runEvent(encounteredEntity)

    if pe.me.exploring == 1:
        realTimeActivity(timer, encounterCode, montage,activity,dest,journeyLength,progress)

    if pe.me.exploring == 0:
        g.gwin.button1.grid()
        g.gwin.button2.grid()
        g.gwin.button3.grid()
        arrive()

def stopExplore():
    pe.me.exploring=0

def toggleSneak():
    if not pe.me.sneaking:
        pe.me.sneaking = True
        g.gwin.button1['text'] = 'Stop Sneaking'
    else:
        pe.me.sneaking = False
        g.gwin.button1['text'] = "Sneak"

def toggleForage():
    if not pe.me.foraging:
        pe.me.foraging = True
        g.gwin.button2['text'] = 'Stop Foraging'
    else:
        pe.me.foraging = False
        g.gwin.button2['text'] = "Forage"

def randomEncounter(encounterCode, encounterRate):
    encounterList = []

    if pe.me.name == "Druid": #for debugging
        pe.me.inv.append(it.createItem('Krog Guts'))
        pe.me.inv.append(it.createItem('Krog Guts'))
        for i in w.world.nodes[pe.me.location]['sites']:
            if i.area == 'wild':
                encounterList.append([i, 's'])

    elif encounterCode == 'rmws':                                                              #rmws: region monsters and wild sites
        for i in w.world.nodes[pe.me.location]['monsters']:  # generate encounter list from monsters and wild sites
            encounterList.append([i, 'm'])
        for i in w.world.nodes[pe.me.location]['sites']:
            if i.area == 'wild':
                encounterList.append([i, 's'])
                                                                                             # todo check if anyone is travelling to encoutner them
    if encounterCode == 'r':                                                                            #t: travel
        for e in ev.eventList:
            if e.location == "road":                                                                               #todo random events on the road
                encounterList.append([e, 'e'])

        #todo make random enoucnters more common the more populated the region is 2 * (infestation + 4)
    if len(encounterList) >= 1:
        if r.random() <= encounterRate:                                                             #determine random enounter, if any
            found = r.randrange(len(encounterList))
            return encounterList[found][0], encounterList[found][1]
    return -1, -1                                                                                   #if no encounter, return -1 for nothing to happen

def druid(drood):
    #todo add option to trade krog teeth for curative herbs
    #todo druid doesn't become known until payedextra. make even for becoming known
    g.clearText()
    g.setText(label4= "Give me 2 Krog Guts and I can tell you how many Krogs are left in a region.")
    gutCount = 0
    guts = []

    gutList = it.checkInvForItems('Krog Guts', 2)
    g.gwin.button0["command"] = lambda:checkGuts(gutList)

    def checkGuts(gutCount):
        if len(gutCount) >= 2:  # if the player has enough guts and gives them up
            g.setText(label4="Which region? ", label6="", label7="")
            g.gwin.button0["text"] = "Confirm"
            g.gwin.button0["command"] = lambda: giveGuts(int(g.gwin.textInput.get()))
        else:
            arrive()    #todo

        def giveGuts(regionToSearch):
            krogCount = 0
            for mon in w.world.node[regionToSearch]['monsters']:  # count all living krogs in the node
                if mon.currentHP >= 1:
                    krogCount += 1
            g.setText(label4=f"There are {krogCount} krogs still alive around {regionToSearch}.")

            it.removeItemsFromInv(2, 'Krog Guts')

            g.gwin.button0['command'] = lambda:arrive() #todo?

def inn(store):
    g.gwin.button0['text'] = "Gather Rumors"
    g.gwin.button0['command'] = lambda:rumors()

    g.gwin.button1['text'] = "Get directions"
    g.gwin.button1['command'] = lambda:directions()

    g.gwin.button2['text'] = 'Rent a Room'
    g.gwin.button2['command'] = lambda:rentRoom()

    g.gwin.button3['text'] = 'Return'
    g.gwin.button3['command'] = lambda:pl.arrive()

    def rumors():
        pass
        #todo learn neighboring cities
        #todo learn locations of druid, dungeons, etc
        #todo if a PC does well, create rumors of them
        #todo location of king krog
        #todo side quests

    def directions():
        dest = int(input("To where? "))
        route = nx.shortest_path(w.world, pe.me.location, dest)
        t.timePasses()

        if len(route) >= 6:
            print("Never heard of it.")
        elif len(route) < 6 and len(route) >= 4:
            print(f"Not sure exactly, but I know you have to travel {w.world[pe.me.location][route[1]]['description']} to {route[1]} to get there.")
        elif len(route) < 4:
            print(route)  # todo make this speech

    def rentRoom():
    # todo option to keep things in your room and rent for X days, etc
        g.clearText()
        g.setText(label4="Sleep for how many hours?")
        g.gwin.button0['text'] = 'Continue'
        g.gwin.button0['command'] = lambda:sleep()

        g.gwin.button3['text'] = 'Return'
        g.gwin.button3['command'] = lambda:arrive()

        def sleep():
            sleepTime = int(g.gwin.textInput.get())

            t.timePasses(sleepTime)

            healed = sleepTime * int(pe.me.tough/2)
            pe.me.currentHP = min(pe.me.maxHP, pe.me.currentHP + healed)

            g.setText(label4=f"You sleep for {sleepTime} hours.")
            g.setText(label5=f"You heal {healed}.")
            g.updateStatus()

            g.gwin.button0['command'] = lambda:arrive()


def witch(store):
    pass
