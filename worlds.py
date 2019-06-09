import os
import pickle as p
import random as r

import networkx as nx

import items as it
import people as pe
import places as pl
import times as t

world = 5
activeJobs = []

class road:
    def __init__(self, desc, length, type, known, roughness, travellers):
        self.desc = desc
        self.length = length
        self.type = type
        self.known = known
        self.roughness = roughness
        self.travellers = travellers


class stock:
    def __init__(self, item, inStock, reqStock, buy, sell, craft, need='unknown', job=None):
        self.item = item
        # self.inStock = inStock
        self.reqStock = reqStock
        self.entities = []
        # self.needState = need
        self.source = None
        self.buy = buy
        self.sell = sell
        self.craft = craft
        self.job = job


class shoppingBag:
    def __init__(self, item, holding, wants):
        self.item = item
        self.holding = holding
        self.wants = wants
        self.entities = []


class craft:
    def __init__(self, jobID, worker, homeShop, quantity, item, status):
        self.jobID = jobID
        self.worker = worker
        self.homeShop = homeShop
        self.status = status
        # self.timeRemaining = -1
        self.quantity = quantity
        self.item = item
        self.craftMatProgress = []
        self.craftMatsApplied = []


class shoppingTrip:
    def __init__(self, jobID, worker, homeShop, shop, distance, wallet, wagon, item, returning, status='inactive'):
        self.jobID = jobID
        self.worker = worker
        self.homeShop = homeShop
        self.shop = shop
        self.distance = distance
        self.wallet = wallet
        self.wagon = wagon
        self.item = item
        # self.timeRemaining = 0
        self.returning = returning
        self.status = status


class harvest:
    def __init__(self, jobID, worker, homeShop, item):
        self.jobID = jobID
        self.worker = worker
        self.homeShop = homeShop
        self.item = item
        self.status = 'inactive'


def buildWorld(numCities, infestation):     #todo alternate worlds?
    global world

    capidx = 0
    #it.createItem(0)
    it.initItemTypeList()
    pe.initPersonTypeList()
    pl.initPlaceTypeList()

    #randomly generated node web
    # web = nx.barabasi_albert_graph(numCities,1)
    # web = nx.newman_watts_strogatz_graph(numCities,2, 1)
    # web = nx.connected_watts_strogatz_graph(numCities,4,1)
    web = nx.balanced_tree(4, 3)
    web.add_edge(1, 2)
    web.add_edge(2, 3)
    web.add_edge(3, 4)
    web.add_edge(4, 1)

    color_map = ['red'] * 85
    terraincolors = ['green', 'purple', 'lime', 'brown', 'grey', 'blue']
    terraintypes = ['grassland', 'swamp', 'forest', 'farm', 'mountains', 'coast']
    for node in range(0, 21):
        if node == 0:
            color_map[node] = 'green'
            web.nodes[node]['terrain'] = 'grassland'
        elif node in range(1, 5):
            color_map[node] = 'tan'
            web.nodes[node]['terrain'] = 'grassland'
        elif node in range(5, 21):
            # TODO create country
            randterrain = r.randrange(0, len(terraincolors))
            color_map[node] = terraincolors[randterrain]
            web.nodes[node]['terrain'] = terraintypes[randterrain]
            for i in range(1, 5):
                color_map[(node * 4) + i] = terraincolors[randterrain]
                web.nodes[(node * 4) + i]['terrain'] = terraintypes[randterrain]
        else:
            web.nodes[node]['terrain'] = 'grassland'



    web = t.createCalendar(web)
    # setattr(web,'mine',[])
    # setattr(web,'lumbermill',[])
    # setattr(web,'blacksmith', [])
    capidx = 0

    #construct and describe roads
    with open('roaddesc.txt') as f:
        lines = f.read().splitlines()
    
    for (u,v,w) in web.edges(data=True):
        temp = r.sample(range(len(lines)), 2)

        type = 'road'
        desc = f"a {str(lines[temp[0]])}, {str(lines[temp[1]])} {type}"
        lent = r.randrange(8, 25)
        kn = 0

        w['route'] = road(desc,lent,type,kn,1,[])

    #give each node sites and people
    for x in range(len(web.nodes)):
        web.nodes[x]['name'] = randomName('city')
        web.nodes[x]['sites'] = []
        web.nodes[x]['label'] = int(x)
        web.nodes[x]['population'] = pe.createPerson(2, 20, 'rand', location=x, homeLocation=x)

        if x == 0:
            tempsite = pl.createPlace('Weapon Shop')
            setattr(tempsite, 'stocks', [stock(it.ref('Short Sword'), 0, 5, True, False, False),
                                         stock(it.ref('Dagger'), 0, 10, True, False, False),
                                         stock(it.ref('Club'), 0, 3, True, False, False)])
            setattr(tempsite, 'money', r.randrange(1000, 3000))
            setattr(tempsite, 'employees', employRandom(web, x, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', True)
            setattr(tempsite, 'location', x)
            web.nodes[x]['sites'].append(tempsite)

            tempsite = pl.createPlace('Armor Shop')
            setattr(tempsite, 'stocks', [stock(it.ref('Plate Mail'), 0, 2, True, False, False)])
            setattr(tempsite, 'money', r.randrange(1000, 3000))
            setattr(tempsite, 'employees', employRandom(web, x, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', True)
            setattr(tempsite, 'location', x)
            web.nodes[x]['sites'].append(tempsite)

            tempsite = pl.createPlace('Inn')
            setattr(tempsite, 'stocks', [])
            setattr(tempsite, 'money', r.randrange(100, 300))
            setattr(tempsite, 'employees', employRandom(web, x, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', False)
            setattr(tempsite, 'location', x)
            web.nodes[x]['sites'].append(tempsite)

        if x in range(1, 5):  # todo dynamically create stocks based on what items the shop could create, based on the items craftMats
            # todo increase prices at tradeposts
            # todo rename blacksmith to arms/armor shop, etc, since they dont craft anything
            tempsite = pl.createPlace('Blacksmith')
            setattr(tempsite, 'stocks', [stock(it.ref('Short Sword'), 0, 4, False, True, True),
                                         stock(it.ref('Plate Mail'), 0, 1, False, True, True),
                                         stock(it.ref('Dagger'), 0, 10, False, True, True),
                                         stock(it.ref('Iron Ore'), 10, 20, True, False, False),
                                         stock(it.ref('Wood'), 10, 20, True, False, False)])
            setattr(tempsite, 'money', r.randrange(100, 300))
            setattr(tempsite, 'employees', employRandom(web, x, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', True)
            setattr(tempsite, 'location', x)
            web.nodes[x]['sites'].append(tempsite)


                                #todo food sites generate food supply

        numKrog = r.randrange(infestation + 3, 2 * (infestation + 4))
        web.nodes[x]['monsters'] = pe.createPerson(pTID=1, number=numKrog, location=x)
        # todo add savagery attr to nodes which affect number and strength of krogs
        # todo randomize krog growth times

    for x in range(5, 21):
        randnode = (x * 4) + r.randrange(1, 5)  # create mining camp in random node
        color_map[randnode] = 'blue'
        if web.nodes[randnode]['terrain'] == "mountains":
            tempsite = pl.createPlace('Mining Camp')
            setattr(tempsite, 'stocks', [stock(it.ref('Iron Ore'), 0, 500, False, True, True)])
            setattr(tempsite, 'money', r.randrange(100, 300))
            setattr(tempsite, 'employees', employRandom(web, randnode, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', True)
            setattr(tempsite, 'location', x)
            web.nodes[randnode]['sites'].append(tempsite)

        elif web.nodes[randnode]['terrain'] == 'forest':
            tempsite = pl.createPlace('Lumbermill')
            setattr(tempsite, 'stocks', [stock(it.ref('Wood'), 0, 500, False, True, True)])
            setattr(tempsite, 'money', r.randrange(100, 300))
            setattr(tempsite, 'employees', employRandom(web, randnode, 3))  # todo set num employees based on size
            setattr(tempsite, 'economic', True)
            setattr(tempsite, 'location', x)
            web.nodes[randnode]['sites'].append(tempsite)

    nx.draw(web, node_color=color_map, with_labels=True)
    # plt.show()

    fillEmptySources(web)

    # create Druid Circle in a random node
    #web.nodes[r.randrange(len(web.nodes))]['sites'].append(pl.createPlace(4, "Druid Circle"))
    createSiteAtRandomLoc(web, 4, 'Druid Circle')
    createSiteAtRandomLoc(web, 8, 'Hunter Camp')

    # create Krog Hill in a random node
    web.nodes[r.randrange(len(web.nodes))]['monsters'].append(pl.createPlace(4))

    web.graph['instability'] = 0
    web.graph['capital'] = capidx

    kkloc = 3 #todo dont do it this way lol
    #kkloc = r.randrange(r.randrange(len(web.nodes)))
    pe.createBoss()  # todo re-add but change to normal person of kingkrog object
    pe.kingKrog.location = kkloc

    return web


def fillEmptySources(web):
    for y in web.nodes:
        for s in web.nodes[y]['sites']:
            if s.stocks != []:
                for tocks in s.stocks:
                    if tocks.source is None:
                        tocks.source = findClosestSource(web, y, tocks.item.itemType)


def findClosestSource(web, homeNode, itType):
    tempItem = it.ref(itType)
    shortestPath = 99999
    genlist = []
    closest = None

    for node in web.nodes:
        for site in web.nodes[node]['sites']:
            if site.economic:
                for stk in site.stocks:
                    if stk.item.itemType == itType and stk.craft and site.location == homeNode:
                        return 'self'
                    elif stk.item.itemType == itType and stk.sell and len(stk.entities) > 0:  # if the store's stock matches the item being searched for and that site sells that item, and they have stock...
                        genlist.append(site)  # add it to the list of possible sources

    if tempItem.harvestable or tempItem.craftable:
        # craftList = [o.location for o in getattr(web,tempItem.craftSite)]
        for harvSite in genlist:
            if nx.shortest_path_length(web, homeNode, harvSite.location) < shortestPath:
                closest = harvSite
                shortestPath = nx.shortest_path_length(web, homeNode, harvSite.location)
        return closest

    return None


def runWorld(hours):
    global world
    global activeJobs
    TRAVEL_TIME = 4  # todo make based on roads
    progBar = '|..................................................|'
    prog = 0
    craftFlag = []

    for i in range(hours):
        progBar = '|................................................|'
        progBar = progBar[0:int(i / (hours / 50))] + "|" + progBar[int(i / (hours / 50)):]
        # print(progBar, len(activeJobs))

        # todo add paying employees. if payment not received, employee quits.
        # todo paying taxes, if taxes not paid, shop closes

        for node in world.nodes:
            for shop in world.nodes[node]['sites']:
                for i in shop.stocks:
                    if len(i.entities) < i.reqStock:
                        if not i.job:  # if the stock doesn't currently have a job
                            if i.source == 'self' and i.item.craftable and i.craft:
                                tempCraft = craft(len(activeJobs), None, shop, i.item.craftQuantity, it.createItem(i.item.itemType), 'inactive')
                                for craftMatsOfItem in i.item.craftMats:
                                    tempCraft.craftMatProgress.append([craftMatsOfItem[0], 0, craftMatsOfItem[1]])
                                setWorkerToJob(tempCraft)  # add job to create item
                                i.job = tempCraft

                            elif i.source == 'self' and i.item.harvestable:
                                tempHarvest = (harvest(0, None, shop, i.item))
                                setWorkerToJob(tempHarvest)
                                i.job = tempHarvest
                            elif i.source == None:
                                i.source = findClosestSource(world, shop.location, i.item.itemType)
                            elif i.source is not None and i.buy:  # if the item has a source and the homeshop buys this item, set up shopping trip

                                quantityToBuy = max(1, int(i.reqStock * 0.3))  # buy 30% of required stock
                                fullCost = quantityToBuy * i.item.cost
                                if fullCost > shop.money:  # figure out how much the home shop can buy
                                    allowedCost = int(shop.money / i.item.cost) * i.item.cost
                                else:
                                    allowedCost = fullCost

                                if allowedCost > 0:
                                    shop.money -= allowedCost
                                    # e.busy = True
                                    i.source = findClosestSource(world, shop.location, i.item.itemType)
                                    distance = 4  # todo make this actual distance between nodes
                                    tempJob = shoppingTrip(len(activeJobs), None, shop, i.source, distance, allowedCost, shoppingBag(i.item, 0, quantityToBuy), i.item, False)  # fixme replace none with worker
                                    setWorkerToJob(tempJob)
                                    # e.job = tempJob
                                    # e.location = [shop.location, i.source.location]
                                    # i.needState = 'being worked'
                                    i.job = tempJob
                                else:
                                    break

                            else:
                                pass

                    # if len(i.entities) >= i.reqStock:
                    # i.needState = 'fully stocked'

        for node in world.nodes:
            for shop in world.nodes[node]['sites']:
                for j in shop.stocks:
                    if j.job:
                        if j.job.status == 'need stock':
                            for craftProgress in j.job.craftMatProgress:
                                if craftProgress[1] < craftProgress[2]:  # if the number of materials applies is less than is needed
                                    for homeStocks in j.job.homeShop.stocks:
                                        if homeStocks.item.typeID == craftProgress[0]:
                                            if len(homeStocks.entities) > 0:
                                                continue
                                            else:
                                                break
                                    else:
                                        j.job.status = 'inactive'

                        if j.job.status == 'inactive':
                            if j.job.worker == None:
                                for e in j.job.homeShop.employees:
                                    if not e.job:
                                        j.job.worker = e  # todo give workers skills and prioratize workers based on skill
                                        j.job.status = 'active'
                                        e.job = j
                                        # j.worker.busy = True
                                        if type(j) == shoppingTrip:
                                            e.location = [j.job.homeShop.location, j.job.shop.location]
                                        break

                        if j.job.status == 'active':  # status 1 means job is still active
                            if type(j.job) == harvest:  # todo have ites go into worker inv then move to store after inv limite reached
                                stidx = findStockIndex(j.job.homeShop, j.job.item)
                                for i in range(j.job.item.craftQuantity):
                                    j.job.homeShop.stocks[stidx].entities.append(it.createItem(j.job.item.itemType))
                                if checkStockFull(j.job.homeShop, j.job.item):
                                    j.job.status = 'complete'

                            elif type(j.job) == craft:
                                for craftProgress in j.job.craftMatProgress:
                                    if craftProgress[1] < craftProgress[2]:  # if the number of materials applies is less than is needed
                                        for homeStocks in j.job.homeShop.stocks:
                                            if homeStocks.item.typeID == craftProgress[0]:
                                                if len(homeStocks.entities) > 0:
                                                    j.job.craftMatsApplied.append(homeStocks.entities.pop(0))
                                                    craftProgress[1] += 1
                                                    break
                                                else:
                                                    j.job.status = 'need stock'
                                                    try:
                                                        j.job.worker.job = None
                                                        j.job.worker = None
                                                    except:
                                                        pass
                                                    break
                                    else:
                                        continue
                                    break
                                for each in j.job.craftMatProgress:
                                    if each[1] < each[2]:
                                        break
                                else:
                                    sidx = findStockIndex(j.job.homeShop, j.job.item)
                                    j.job.homeShop.stocks[sidx].entities.append(j.job.item)
                                    j.job.item.craftMatsSource = []
                                    j.job.item.crafter = j.job.worker
                                    for each in j.job.craftMatsApplied:
                                        pass  # todo track soure of materials in item
                                    j.job.status = 'complete'


                            elif type(j.job) == shoppingTrip:
                                if j.job.distance > 0:
                                    j.job.distance -= 1
                                elif j.job.distance <= 0:
                                    if not j.job.returning:  # buying from the shop
                                        stidx = findStockIndex(j.job.shop, j.job.item)
                                        for count in range(j.job.wagon.wants):  # for the quantity the buyer wants...
                                            if j.job.wallet >= j.job.item.cost and len(j.job.shop.stocks[stidx].entities) >= 1:  # check if the buyer can afford it and the store has actual stock of the item
                                                j.job.wagon.entities.append(j.job.shop.stocks[stidx].entities.pop(0))  # move the item to the buyers wagon
                                                j.job.wagon.holding += 1  # mark the increase of wagon stock
                                                j.job.wallet -= j.job.item.cost  # exchange money
                                                j.job.shop.money += j.job.item.cost
                                            else:
                                                break
                                        j.job.returning = True
                                        j.job.distance = 4
                                        j.job.worker.location = [j.job.shop.location, j.job.homeShop.location]

                                    else:  # returning with purchased goods
                                        stidx = findStockIndex(j.job.homeShop, j.job.item)
                                        for wagonItem in j.job.wagon.entities:
                                            j.job.homeShop.stocks[stidx].entities.append(wagonItem)

                                        # if len(j.homeShop.stocks[stidx].entities) >= j.homeShop.stocks[stidx].reqStock:
                                        # j.homeShop.stocks[stidx].needState = 'fully stocked'
                                        # else:
                                        # j.homeShop.stocks[stidx].needState = 'under stocked'
                                        j.job.homeShop.money += j.job.wallet
                                        j.job.status = 'complete'
                                        j.job.worker.location = j.job.homeShop.location

                        if j.job.status == 'complete':
                            j.job.worker.job = None
                            # activeJobs.remove(j)
                            stidx = findStockIndex(j.job.homeShop, j.job.item)
                            j.job.homeShop.stocks[stidx].job = None


def setWorkerToJob(job):
    if not job.worker:
        for e in job.homeShop.employees:
            if not e.job:
                job.worker = e  # todo give workers skills and prioratize workers based on skill
                job.status = 'active'
                e.job = job
                # j.worker.busy = True
                if type(job) == shoppingTrip:
                    e.location = [job.homeShop.location, job.shop.location]
                break


def checkStockFull(store, item):
    for storeStocks in store.stocks:
        if storeStocks.item.typeID == item.typeID:
            if len(storeStocks.entities) >= storeStocks.reqStock:
                return True
            else:
                return False


def findStockIndex(store, item):
    for stockidx, storeStocks in enumerate(store.stocks):
        if storeStocks.item.typeID == item.typeID:
            return stockidx


def createSiteAtRandomLoc(web, sTID, name):
    web.nodes[r.randrange(len(web.nodes))]['sites'].append(pl.createPlace(sTID, name))


def worldInfo():
    print("The capital city is " + str(world.graph['capital']))


def saveWorld():
    global world

    nx.write_gpickle(world, r'world/world.kr')
    # nx.write_gml(world, r'world/world.kr')

    with open(r"world/items.kr", "wb") as pit:
        p.dump(it.items, pit)
    pit.close()
    with open(r"world/places.kr", "wb") as ppl:
        p.dump(pl.places, ppl)
    ppl.close()
    with open(r"world/persons.kr", "wb") as ppe:
        p.dump(pe.persons, ppe)
    ppe.close()
    with open(r"world/history.kr", "wb") as phi:
        p.dump(t.history, phi)
    phi.close()
    with open(r"world/obituary.kr", "wb") as obi:
        p.dump(pe.futureDead, obi)
    obi.close()
    t.printHistory()

def loadWorld():
    global world
    #todo time is not loaded
    world = nx.read_gpickle(r'world/world.kr')
    #world = nx.read_gml(r'world/world.kr')
    world = nx.convert_node_labels_to_integers(world)

    with open(r"world/items.kr", "rb") as pit:
        it.items = p.load(pit)

    with open(r"world/places.kr", "rb") as ppl:
        pl.places = p.load(ppl)

    with open(r"world/persons.kr", "rb") as ppe:
        pe.persons = p.load(ppe)

    with open(r"world/history.kr", "rb") as phi:
        t.history = p.load(phi)

    with open(r"world/obituary.kr", "rb") as obi:
        pe.futureDead = p.load(obi)

    pe.findBoss()

def openInitialWorld():  # todo I think dead monster inventory loot resets
    global world

    world = nx.read_gpickle(r'world/worldStart.kr')
    #world = nx.read_gml('world/worldStart.kr')
    world = nx.convert_node_labels_to_integers(world)

    with open(r"world/itemsStart.kr", "rb") as pit:
        it.items = p.load(pit)
    pit.close()
    with open(r"world/placesStart.kr", "rb") as ppl:
        pl.places = p.load(ppl)
    ppl.close()
    with open(r"world/personsStart.kr", "rb") as ppe:
        pe.persons = p.load(ppe)
    ppe.close()
    with open(r"world/history.kr", "rb") as phi:
        t.history = p.load(phi)
    phi.close()
    with open(r"world/obituary.kr", "rb") as obi:
        pe.futureDead = p.load(obi)
    obi.close()

    pe.findBoss()

    # todo new characters don't keep known info about locs and roads?
    # todo can find journal of dead characters to learn all stuff they knew

def saveWorldState():
    if not os.path.exists('world'):
        os.makedirs('world')

    nx.write_gpickle(world, r'world/worldStart.kr')
    #nx.write_gml(world, 'world/worldStart.kr')  # saves the world state for future characters

    with open(r"world/itemsStart.kr", "wb") as pit:
        p.dump(it.items, pit)
    pit.close()
    with open(r"world/placesStart.kr", "wb") as ppl:
        p.dump(pl.places, ppl)
    ppl.close()
    with open(r"world/personsStart.kr", "wb") as ppe:
        p.dump(pe.persons, ppe)
    ppe.close()

def randomName(type):
    name = ""
    cnsnnts = ['B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'QU', 'R', 'S', 'T', 'V', 'W', 'X', 'Z']
    oe = ['A', 'E', 'U', 'I', 'O', 'Y']
    if type == 'city':
        for i in range(r.randrange(2,4)):
            name += cnsnnts[r.randrange(len(cnsnnts) - 1)] + oe[r.randrange(len(oe) - 1)]
    return name


def employRandom(web, loc, num):
    templist = []

    for pers in web.nodes[loc]['population']:
        if pers.employed is None:
            templist.append(pers)
            pers.employed = True
            if len(templist) >= num:
                break

    return templist
