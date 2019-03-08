import csv

#itemID; Name; itemType; itemStrength, itemWorth

with open(r"C:\Users\Matt\Documents\GitHub\Krog\itemList.txt") as f:
  reader = csv.reader(f)
  itemList = tuple(reader)

def checkItem(itemID):
    return(itemList[itemID][1])
