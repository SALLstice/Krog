import items as it
import people as pe
import worlds as w
import times as t
import random as r
import math as m

siteTypeList = []
sites = []

class site:
    def __init__(self,entityID,name,siteType,area,known,inv):
        self.entityID = entityID
        self.name = name
        self.siteType = siteType
        self.area = area
        self.known = known
        self.inv = inv

#create itemList from file              #todo I think this was important
# with open('siteList.txt') as f:
#     for line in f:
#         row = eval(line)
#         siteTypeList.append(site(int(len(siteTypeList)),row[0],row[1],[]))

def createSite(name,siteType,area,known,inv):        #todo gain ability to reverse lookup sites by location
    sites.append(site(int(len(sites)),
                      name,
                      siteType,
                      area,
                      known,
                      inv))
    return sites[int(len(sites)-1)].entityID

def locInfo(): #todo split lodInfo into branch options, shop, rumors, revisit known wild location, town inventory, food stores, etc
    loc = pe.me.location
    #print("Shop\nRumors\nInfo") #todo make rumors
    print("\nYou are in " + str(loc) + ".") #todo inn to sleep and recover
                                                                                #prints out every known site in location
    for x in range(len(w.world.nodes[loc]['sites'])):
        if sites[w.world.nodes[loc]['sites'][x]].area == 'town':
            print(x, sites[w.world.nodes[loc]['sites'][x]].name, sites[w.world.nodes[loc]['sites'][x]].siteType)
        if sites[w.world.nodes[loc]['sites'][x]].area == 'wild' and sites[w.world.nodes[loc]['sites'][x]].known == 1:
            print(x, sites[w.world.nodes[loc]['sites'][x]].name, sites[w.world.nodes[loc]['sites'][x]].siteType)
    whereGo = int(input())
    siteActivity(sites[w.world.nodes[loc]['sites'][whereGo]])

def siteActivity(store):
    #Shops
    if store.siteType in ('food', 'armor', 'weapons'):
        print("\nYou are at " + str(store.name))
        buySell = int(input("1: Buy\n2: Sell\n"))

        #Display items to buy
        if buySell == 1:
            print("Stock:")
            for i in range(len(store.inv)):
                if it.items[store.inv[i]].itemType != 'null':
                    print(i, it.items[store.inv[i]].itemType, it.items[store.inv[i]].combatValue, it.items[store.inv[i]].cost)
            whatBuy = int(input())
            if whatBuy == 0:
                return
            if pe.me.inv[2] >= it.items[store.inv[whatBuy]].cost:
                it.buyItem(store.inv[whatBuy], store)

        #Display items to sell
        elif buySell == 2:
            print("\nItems in your bag:")
            for i in range(len(pe.me.inv[1])):
                if it.items[pe.me.inv[1][i]].itemType != 'null':
                    print(i, it.items[pe.me.inv[1][i]].itemType)
            sell = int(input())
            it.sellItem(pe.me.inv[1][sell], store)          #todo make it so inputing 1 4 18 5 sells item 1, 4, 18 and 5

    #Druid
    elif store.siteType in ('druid'):
        print("Give me 2 Krog Guts and I can tell you how many Krogs are left in the region.")
        count = 0
        guts = []
        for i in range(len(pe.me.inv[1])):
            if it.items[pe.me.inv[1][i]].itemType == "Krog Guts":
                guts.append(pe.me.inv[1][i])                                                #list of itemID of every krog guts
                count += 1
        print("You have", count)
        giveGuts = input("Give the guts? (y/n)")
        if giveGuts == 'y' and count >= 2:                                                  #if the player has enough guts and gives them up
            for j in range(len(w.world.node[0]['monsters'])):                               #count all living krogs in the node
                if pe.persons[w.world.node[0]['monsters'][j]].currentHP > 0:
                    count += 1
            print ("There are", count, "krogs still alive around", pe.me.location)

            pe.me.inv[1].remove(guts[0])                                                    #using the list of guts, remove those from player inventory
            pe.me.inv[1].remove(guts[1])

def travel(): #todo ability to ask and learn where roads lead.
    loc = pe.me.location
    roadMap = []

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
        return                                                                #escape to not travel
    dest = list(w.world.neighbors(loc))[trav-1]                          #-1 to fix index issues
    w.world[loc][dest]['known'] = 1                                  #roads traveled become known

    durationOfTrip = int(w.world[loc][dest]['length']) / pe.me.overlandSpeed
    lengthOfTrip = str(w.world[loc][dest]['length'])

    for i in range(m.ceil(durationOfTrip)):
 #       encounteredEntity, encounterType = randomEncounter('t',0.3)
        t.timePasses()
        #todo needs road event encounters

    pe.me.location = trav
    print("\nYou walked for " + lengthOfTrip + " miles.")
    print("It took you " + '%.1f' % durationOfTrip + " hours.")     #todo either make durationgs hourly increments or allow time to increment quarter hour

    return

def worldInfo(cap):
  print("The capital city is " + str(cap))

def randomEncounter(encounterCode, encounterRate):
    encounterList = []

    if encounterCode == 'rmws':                                                              #rmws: region monsters and wild sites
        for i in range(len(w.world.nodes[pe.me.location]['monsters'])):                     # generate encounter list from monsters and wild sites
            encounterList.append([w.world.nodes[pe.me.location]['monsters'][i], 'm'])
        for i in range(len(w.world.nodes[pe.me.location]['sites'])):
            if sites[w.world.nodes[pe.me.location]['sites'][i]].area == 'wild':
                encounterList.append([w.world.nodes[pe.me.location]['sites'][i], 's'])
                                                                                             # todo check if anyone is travelling to encoutner them
    if encounterCode == 't':                                                                            #t: travel
        encounterList = []                                                                      #todo random events on the road

    if r.random() <= encounterRate:                                                             #determine random enounter, if any
        found = r.randrange(len(encounterList))
        return encounterList[found][0], encounterList[found][1]
    return -1                                                                                   #if no encounter, return -1 for nothing to happen

def exploreRegion(duration):
    then = w.world.graph['hour']
    for j in range(duration):
        t.timePasses()
        encounteredEntity, encounterType = randomEncounter('rmws', 0.7)  # todo option to re-loot dead bodies
        if encounteredEntity != -1:
            now = w.world.graph['hour']                                  # todo while continueing to explore, skip encounters already found this trip
            print ("\nYou explore for" ,str(now - then), "hours, and...")   #todo check if found dead body and give option to keep exploring
            return encounteredEntity, encounterType
    print("\nYou explore for", duration, "hours and find nothing.")     #todo option to keep exploreing
    return -1