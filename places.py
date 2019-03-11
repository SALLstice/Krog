import item as it
import people as pe

siteTypeList = []
sites = []

class site:
    def __init__(self,entityID,name,siteType,civil,inv):
        self.entityID = entityID
        self.name = name
        self.siteType = siteType
        self.civil = civil
        self.inv = inv

#create itemList from file
with open('siteList.txt') as f:
    for line in f:
        row = eval(line)
        siteTypeList.append(site(int(len(siteTypeList)),row[0],row[1],row[2],[]))

#todo make some sites hidden
def createSite(name,siteType,loc,civil,inv):
    sites.append(site(int(len(sites)),name,siteType,civil,inv))
    return sites[int(len(sites)-1)].entityID

def locInfo(web, loc): #todo split lodInfo into branch options
    print("Shop\nRumors\nInfo") #todo make rumors
    print("\nYou are in " + str(loc) + ".")
    #prints out every site in location
    for x in range(len(web.nodes[loc]['sites'])):
        print(x, sites[web.nodes[loc]['sites'][x]].name, sites[web.nodes[loc]['sites'][x]].siteType)
    whereGo = int(input())
    shop(sites[web.nodes[loc]['sites'][whereGo]])

def shop(store):
    print("\nYou are at " + str(store.name))
    buySell = int(input("1: Buy\n2: Sell\n"))

    #Buy an item
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

    #Sell an Item
    elif buySell == 2:
        print("\nItems in your bag:")
        for i in range(len(pe.me.inv[1])):
            if it.items[pe.me.inv[1][i]].itemType != 'null':
                print(i, it.items[pe.me.inv[1][i]].itemType)
        sell = int(input())
        it.sellItem(pe.me.inv[1][sell], store)


def travel(web, loc): #todo ability to ask and learn where roads lead. Going to a location learns that road
    print("\nYou are in " + str(loc) + ".")
    print("There are " + str(len(web.edges(loc))) + " roads out of " + str(loc) + ": ")
    print("0 : Don't travel")
    for j in range(len(list(web.neighbors(loc)))):
        print(j+1, ": " + web[loc][list(web.neighbors(loc))[j]]['description'])
    trav = int(input("Which road will you travel?\n"))
    if trav == 0: return -5
    return(list(web.neighbors(loc))[trav-1])

def worldInfo(cap):
  print("The capital city is " + str(cap))
