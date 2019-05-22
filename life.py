import time

class store:
    def __init__(self, stocks, employees, money):
        self.stocks = stocks
        self.money = money
        self.employees = employees

class item:
    def __init__(self, type, cost, craftable, harvestable, craftMats, craftTime, craftQuantity):
        self.type = type
        self.cost = cost
        self.craftable = craftable
        self.harvestable = harvestable
        self.craftMats = craftMats
        self.craftTime = craftTime
        self.craftQuantity = craftQuantity

class stock:
    def __init__(self, item, inStock, reqStock, need=0):
        self.item = item
        self.inStock = inStock
        self.reqStock = reqStock
        self.need = need

class worker:
    def __init__(self, name):
        self.name = name
        self.busy = False
        self.job = None
        self.wallet = 0

class job:
    def __init__(self, worker, store, craft, quantity):
        self.worker = worker
        self.store = store
        self.craft = craft
        self.status = 0
        self.timeRemaining = -1
        self.quantity = quantity

class shoppingTrip:
    def __init__(self, worker, home, shop, wallet, wagon, item, returning):
        self.worker = worker
        self.home = home
        self.shop = shop
        self.wallet = wallet
        self.wagon = wagon
        self.item = item
        self.timeRemaining = 0
        self.returning = returning
        self.status = 0

activeJobs = []
TRAVEL_TIME = 4

tom = worker('tom')
alex = worker('alex')
emily = worker('emily')
jonas = worker('jonas')

ironOre = item('ore', 2, False, True, None, 8, 5)
wood = item('wood', 1, False, False, None, 6, 4)
sword = item('sword', 10, True, False, [[ironOre, 3], [wood, 2]], 6, 1)
mace = item('mace', 15, True, False, [[ironOre, 5]], 12, 1)
blacksmith = store([stock(sword,5,6), stock(mace,2,6),stock(ironOre,61,60), stock(wood,61,60)], [tom, alex, emily],2000)
lumbermill = store([stock(wood,101,100)], [jonas], 1000)

while 1:

    for v in blacksmith.stocks:
        print(f"{v.item.type} {v.inStock} {v.reqStock} {v.need}")
    for p in activeJobs:
        if type(p) == job:
            print(f"{p.worker.name} {p. craft.type} {p.timeRemaining}")
        if type(p) == shoppingTrip:
            print(f"{p.worker.name} {p.wallet} {p.wagon} {p.returning} {p.timeRemaining}")
    print(f"{blacksmith.money} {lumbermill.money}")

    for i in blacksmith.stocks:
        if i.inStock < i.reqStock and i.need != 2:
            i.need = 1                                                                                                  #need 1 means stock of item is needed
            for e in blacksmith.employees:
                if not e.busy:
                    e.busy = True
                    if i.item.craftable or i.item.harvestable:
                        activeJobs.append(job(e, blacksmith, i.item, i.item.craftQuantity))
                        i.need = 2                                                                                          #need 2 means stock of item is being created
                        break
                    elif not i.item.craftable and not i.item.harvestable:
                        quantityToBuy = int((i.reqStock + i.reqStock*0.10) - i.inStock)                                      #buy enough to get 10% over req amount, I think
                        totalCost = quantityToBuy * i.item.cost

                        blacksmith.money -= totalCost
                        activeJobs.append(shoppingTrip(e,blacksmith,lumbermill,totalCost,[],i.item, False))

                        i.need = 2
                        break

        if i.inStock >= i.reqStock:
            i.need = 0

    for j in activeJobs:
        if j.status == 1:
            j.timeRemaining -= 1                                                                                        #decrement the time remaining for each job

            if j.timeRemaining <= 0:                                                                                    #when the item is done being crafted
                if type(j) == job:
                    for stockidx, stockitem in enumerate([h.item for h in [o for o in j.store.stocks]]):                    #go through every item in the store's stock
                        if j.craft.type == stockitem.type:                                                                  #find the inventory of item that was just crafted
                            j.store.stocks[stockidx].inStock += j.quantity                                                  #and increase it by craft quantity
                            j.status = 2                                                                                   #set job status to complete

                            if j.store.stocks[stockidx].inStock < j.store.stocks[stockidx].reqStock:                        #if the store's current stock doesnt matches their required stock
                                activeJobs.append(job(j.worker,j.store,j.craft, j.quantity))                                #continue the job
                if type(j) == shoppingTrip:
                    if not j.returning:
                        for stockidx, stockitem in enumerate([h.item for h in [o for o in j.shop.stocks]]):  # go through every item in the store's stock
                            if j.item.type == stockitem.type:
                                j.shop.stocks[stockidx].inStock -= int(j.wallet / stockitem.cost)

                        j.shop.money += j.wallet

                        activeJobs.append(shoppingTrip(j.worker, blacksmith, lumbermill, 0, stock(j.item,int(j.wallet / j.item.cost), 0), j.item, True))

                        j.wallet = 0
                        j.status = 2

                    else:
                        for stockidx, stockitem in enumerate([h.item for h in [o for o in j.home.stocks]]):  # go through every item in the store's stock
                            if j.wagon.item.type == stockitem.type:
                                j.home.stocks[stockidx].inStock += j.wagon.inStock

                        j.status = 2

        elif j.status == 0:
            j.status = 1
            if type(j) == job:
                j.timeRemaining = j.craft.craftTime
            if type(j) == shoppingTrip:
                j.timeRemaining = TRAVEL_TIME

            if type(j) == job:
                if j.craft.craftable:
                    for craftidx, craftitem in enumerate(j.craft.craftMats):                                                    #as above
                        for stockidx, stockitem in enumerate([h.item for h in [o for o in j.store.stocks]]):
                            if craftitem[0].type == stockitem.type:
                                j.store.stocks[stockidx].inStock -= j.craft.craftMats[craftidx][1]                              #remove the crafting materials from the stocks

    for j in activeJobs:
        if j.status == 2:
            activeJobs.remove(j)
            j.worker.busy = False

    input()