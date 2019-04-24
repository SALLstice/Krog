import csv
import math as m
import random as r

import networkx as nx
import time

import gui as g
import items as it
import newCombat as c
import people as pe
import times as t
import worlds as w

placeTypeList = []
places = []
exploring = 0

class place:
    def __init__(self, name, currentHP):
        self.name = name
        self.currentHP = currentHP


class placeType:
    def __init__(self, type, use, area, known, inv, maxHP):
        self.type = type
        self.use = use
        self.area = area
        self.known = known
        self.inv = inv
        self.maxHP = maxHP


with open('placeList.csv') as f:
    reader = csv.reader(f)
    headers = next(reader)
    for row in reader:
        placeTypeList.append(placeType(*headers))
        for val, attr in enumerate(headers):
            try:
                tempval = int(row[val])
            except:
                if attr == "inv":
                    tempval = row[val].split()
                    tempval = [int(x) for x in tempval]
                else:
                    tempval = row[val]

            setattr(placeTypeList[len(placeTypeList) - 1], attr, tempval)
    f.close()


def createPlace(sTID, name="", currentHP=-500):  # todo gain ability to reverse lookup sites by location
    inv = []

    if currentHP == -500:
        currentHP = placeTypeList[sTID].maxHP

    places.append(place(name, currentHP))
    for val, attr in enumerate(list(placeTypeList[0].__dict__.keys())):
        if attr == "inv":

            invList = getattr(placeTypeList[sTID], attr)
            if invList != 0:
                for i in invList:
                    inv.append(it.createItem(i))
            setattr(places[len(places) - 1], "inv", inv)
        else:
            setattr(places[len(places) - 1], attr, getattr(placeTypeList[sTID], attr))

    return places[int(len(places) - 1)]

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
    g.initSelect(display, regSites, "", 'type', 'site', 'dispTown')

    #siteActivity(places[w.world.nodes[loc]['sites'][whereGo]])  #todo can go to hidden sites before finding them

def siteActivity(store):
    #Shops
    if store.use in ('food', 'armor', 'weapon'):  # todo note how damaged the building if it is damaged
        g.clearText()
        g.setText(label4=f"You are at {store.name} {store.type}.")

        g.gwin.button0["text"] = "Buy"
        g.gwin.button0["command"] = lambda: it.displayItems(store)

        g.gwin.button1["text"] = "Sell"
        g.gwin.button1["command"] = lambda: it.inventory(pe.me, 'sell', 'town', sellTo=store)

        # g.setButtons("Buy","pl.buyItem(store)", "Sell", "sellItem", "Return", "dispTown")


# todo re-add
""" 
    elif store.placeType in ('druid'):
        druid()

    elif store.placeType in ('inn'):
        inn()
"""
def travel(): #todo ability to ask and learn where roads lead.
    loc = pe.me.location

    print("\nYou are in " + str(loc) + ".")
    print("There are " + str(len(w.world.edges(loc))) + " roads out of " + str(loc) + ": ")
    print("0 : Don't travel")
    for j in range(len(list(w.world.neighbors(loc)))):
        if w.world[loc][list(w.world.neighbors(loc))[j]]['known'] == 0:                         #check if the road is unknown
            print(j+1, ": " + w.world[loc][list(w.world.neighbors(loc))[j]]['description'])     #if road is unknown, print desc
        elif w.world[loc][list(w.world.neighbors(loc))[j]]['known'] == 1:                       #check if road is known
            print(j+1, ": The road leading to " + str(list(w.world.neighbors(loc))[j]))     #if known, print destination
    trav = int(input("Which road will you travel?\n"))

    if trav == -1:
        return  # escape to not travel
    dest = list(w.world.neighbors(loc))[trav - 1]  # -1 to fix index issues
    w.world[loc][dest]['known'] = 1  # roads traveled become known

    durationOfTrip = m.ceil(int(w.world[loc][dest]['length']) / pe.me.overlandSpeed)
    lengthOfTrip = str(w.world[loc][dest]['length'])

    t.createEvent(t.now(), pe.me.name, 'departs', dest, loc)  # create depart event

    for i in range(durationOfTrip):  #random encounters w/ time advancing
 #       encounteredEntity, encounterType = randomEncounter('t',0.3)
        t.timePasses()
        #todo needs road event encounters

    pe.me.location = dest
    print("\nYou walked for " + lengthOfTrip + " miles.")
    print("It took you %i hours." % durationOfTrip)

    t.createEvent(t.now(), pe.me.name, 'arrives', dest,dest)

    return


def setupExplore(gwin):
    gwin.label1["text"] = "Explore up to how many hours? "
    gwin.button0["text"] = "Accept"
    gwin.button0["command"] = lambda: exploreRegion(gwin, int(gwin.textInput.get()))
    return


def setupExploreRT():
    global exploring

    g.gwin.button1.grid_remove()
    g.gwin.button2.grid_remove()
    g.gwin.button3.grid_remove()
    exploring = 1
    g.setText(label4="Exploring")
    g.gwin.button0["text"] = "Stop Exploring"
    g.gwin.update()
    g.gwin.button0["command"] = lambda: stopExplore()

    encounteredEntity = -1
    timer = 0
    montage = ["", ".", ".  .", ".  .  ."]
    exploreRT(timer, encounteredEntity, montage)


def exploreRT(timer, encounteredEntity, montage):
    global exploring

    time.sleep(0.5)
    g.setText(label5=montage[timer])
    timer += 1
    timer %= 4
    if timer == 0:
        t.timePasses()
        encounteredEntity, encounterType = randomEncounter('rmws', 0.35)
        # todo increase encounter chance based on number of krog

    if encounteredEntity != -1:
        if encounterType == 'm':
            exploring = 2
            g.setText(label4=f"You encounter a {encounteredEntity.name}")
            g.gwin.button0["text"] = "Enter Combat"
            g.gwin.button0["command"] = lambda: c.initCombat(encounteredEntity)
            # combatResult = c.combat(encounteredEntity)
        if encounterType == 's':
            exploring = 2
            g.setText(label4=f"You encounter a {encounteredEntity.placeType}")
            places[encounteredEntity].known = 1

    if exploring == 1:
        g.gwin.after(0, exploreRT(timer, encounteredEntity, montage))
    elif exploring == 0:
        g.gwin.button1.grid()
        g.gwin.button2.grid()
        g.gwin.button3.grid()
        g.dispTown()


def stopExplore():
    global exploring

    exploring = 0


def exploreRegion(gwin, duration):

    then = w.world.graph['hour']  # todo create explore event?
    for j in range(duration):
        t.timePasses()
        encounteredEntity, encounterType = randomEncounter('rmws', 0.35)
        if encounteredEntity != -1:
            now = w.world.graph['hour']  # todo while continueing to explore, skip encounters already found this trip
            gwin.label1["text"] = "You explore for " + str(now - then) + " hours, and..."

            if encounterType == 'm':
                gwin.label2["text"] = "You encounter a " + pe.persons[encounteredEntity].personType
                gwin.button0["text"] = "Enter Combat"
                gwin.button0["command"] = lambda: c.initCombat(gwin, encounteredEntity)
                # combatResult = c.combat(encounteredEntity)
            if encounterType == 's':
                gwin.label2["text"] = "You encounter a " + places[encounteredEntity].placeType
                places[encounteredEntity].known = 1
            return
            # return encounteredEntity, encounterType  # todo hours listed as negative if day rolls over

    gwin.label1["text"] = "\nYou explore for", duration, "hours and find nothing."  # todo option to keep exploreing
    return -1, -1

def randomEncounter(encounterCode, encounterRate):
    encounterList = []

    if encounterCode == 'rmws':                                                              #rmws: region monsters and wild sites
        for i in w.world.nodes[pe.me.location]['monsters']:  # generate encounter list from monsters and wild sites
            encounterList.append([i, 'm'])
        for i in w.world.nodes[pe.me.location]['sites']:
            if i.area == 'wild':
                encounterList.append([i, 's'])
                                                                                             # todo check if anyone is travelling to encoutner them
    if encounterCode == 't':                                                                            #t: travel
        encounterList = []                                                                      #todo random events on the road

    if r.random() <= encounterRate:                                                             #determine random enounter, if any
        found = r.randrange(len(encounterList))
        return encounterList[found][0], encounterList[found][1]
    return -1, -1                                                                                   #if no encounter, return -1 for nothing to happen

def druid():
    #todo add option to trade krog teeth for curative herbs
    print("Give me 2 Krog Guts and I can tell you how many Krogs are left in a region.")
    gutCount = 0
    krogCount = 0
    guts = []

    for i in range(len(pe.me.inv[1])):
        if it.items[pe.me.inv[1][i]].itemType == "Krog Guts":
            guts.append(pe.me.inv[1][i])  # list of itemID of every krog guts
            gutCount += 1
    print("You have", gutCount)
    giveGuts = input("Give the guts? (y/n)")

    if giveGuts == 'y' and gutCount >= 2:  # if the player has enough guts and gives them up
        regionToSearch = int(input("Which region? "))
        for j in range(len(w.world.node[regionToSearch]['monsters'])):  # count all living krogs in the node
            if pe.persons[w.world.node[regionToSearch]['monsters'][j]].currentHP >= 1:
                krogCount += 1
        print("There are", krogCount, "krogs still alive around", regionToSearch)

        pe.me.inv[1].remove(guts[0])  # using the list of guts, remove those from player inventory
        pe.me.inv[1].remove(guts[1])

def inn():
    print(
        "1: Relax at the bar\n2: Gather rumors\n3: Get directions\n4: Rent a room")  # todo option to keep things in your room and rent for X days, etc
    doWhat = int(input())

    if doWhat == 1 or doWhat == 4:
        sleepTime = int(input("Sleep for how many hours? "))
        t.timePasses(sleepTime)

    elif doWhat == 3:
        dest = int(input("To where? "))
        route = nx.shortest_path(w.world, pe.me.location, dest)
        t.timePasses()

        if len(route) >= 6:
            print("Never heard of it.")
        elif len(route) < 6 and len(route) >= 4:
            print(f"Not sure exactly, but I know you have to travel {w.world[pe.me.location][route[1]][
                'description']} to {route[1]} to get there.")
        elif len(route) < 4:
            print(route)  #todo make this speech
