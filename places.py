import csv

#itemID; Name; itemType; itemStrength, itemWorth

with open(r"C:\Users\Matt\Documents\GitHub\Krog\buildings.txt") as f:
  reader = csv.reader(f)
  buildingList = tuple(reader)

def checkBuilding(buildingID):
    return(buildingList[buildingID][1])
