import csv
from items import *

siteList = []
siteEntityList = []

class site:
    def __init__(self,name,siteType):
        self.name = name
        self.siteType = siteType

class siteEntity:
    def __init__(self,entityID,name,siteType,loc,civil,stock):
        self.entityID = entityID
        self.name = name
        self.siteType = siteType
        self.loc = loc
        self.civil = civil
        self.stock = stock

###create siteList from file
##with open(r'C:\Users\Matt\Documents\GitHub\Krog\sites.txt', 'r') as f:
##    reader = csv.reader(f)
##    for row in reader:
##        siteList.append(site(row[0], row[1]))

def createSite(name,siteType,loc,civil,stock):
    siteEntityList.append(siteEntity(int(len(siteEntityList)),name,siteType,loc,civil,stock))
    return siteEntityList[int(len(siteEntityList)-1)].entityID

def locInfo(web, loc):
    print("\nYou are in " + str(loc) + ".")
    for x in range(len(web.nodes[loc]['sites'])):
        print(siteEntityList[web.nodes[loc]['sites'][x]].name, siteEntityList[web.nodes[loc]['sites'][x]].siteType,siteEntityList[web.nodes[loc]['sites'][x]].entityID)
    shop(int(input()))

def shop(entityID):
    print(entityID)
    print("\nYou are at " + str(siteEntityList[entityID].name))
    print("Stock:")
    for i in range(len(siteEntityList[entityID].stock)):
        print(itemEntityList[siteEntityList[entityID].stock[i]].itemType, itemEntityList[siteEntityList[entityID].stock[i]].item.combatValue)
