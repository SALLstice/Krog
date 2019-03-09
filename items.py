import csv

itemList = []
itemEntityList = []

class item:
    def __init__(self,itemTypeID,itemType,equipRegions,combatValue,cost,effect,effectValue):
        self.itemTypeID = itemTypeID
        self.itemType = itemType
        self.equipRegions = equipRegions
        self.combatValue = combatValue
        self.cost = cost
        self.effect = effect
        self.effectValue = effectValue
		
class itemEntity:
    def __init__(self,entityID,itemType,name,desc):
        self.entityID = entityID
        self.name = name
        self.itemType = itemType
        self.desc = desc

#create itemList from file
with open(r'C:\Users\Matt\Documents\GitHub\Krog\itemList.txt', 'r') as f:
    reader = csv.reader(f)
    for row in reader:
        itemList.append(item(itemEntity(int(len(itemEntityList)),row[0], row[1], row[2], row[3], row[4], row[5]))

def createItem(itemType,name=0,desc=0):
    itemEntityList.append(itemEntity(int(len(itemEntityList)),itemType,name,desc))
    return itemEntityList[int(len(itemEntityList)-1)].entityID



def inventory(inv):
  print("\nItems you have equipped:")
  for i in range(len(inv[0])):
    print(itemEntityList[inv[0][i]].itemType)
  print("\nItems in your bag:")
  for i in range(len(inv[1])):
    print(itemEntityList[inv[1][i]].itemType)
  print("\nYou have " + str(inv[2]) + " gold.")    
