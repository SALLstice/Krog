import random

class Item:
	def __init__(self,name,equipRegions,combatValue,cost,effect,effectValue,desc):
		self.name = name
		self.equipRegions = equipRegions
		self.combatValue = combatValue
		self.cost = cost
		self.effect = effect
		self.effectValue = effectValue
		self.desc = desc
		
shortSword = Item('Short Sword',(1, 2),2,30,0,0,0)

#print(shortSword.name, shortSword.combatValue)

class entity:
	def __init__(self,name,itemType,desc):
		self.name = name
		self.itemType = item
		self.desc = desc

entityList = []

for i in range(4):
	entityList.append(entity("Stabber",shortSword,random.randrange(20)))

for i in range(4):
	print(entityList[i].name,entityList[i].itemType.name, entityList[i].desc)