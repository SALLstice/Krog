# import networkx as nx
# import matplotlib.pyplot as plt
# import random
# from WorldBuilder import *
# from items import *
# from places import *
# import csv
# import ast

#web, cap = buildWorld(12)
# loc = 1
# equip = [1,3]
# bag = [5,5]
# inv = [equip, bag, 0]
flist = []
##------------------------------------

money = 15015

print("gold " + str(int((money / 10000))))
print("silver " + str(int(money / 100)%100))
print("copper " + str(money % 100))










##------------------------------------



#
# with open('testfile.txt') as f:
#     for line in f:
#         flist.append(eval(line))
#     print(flist)

##--------------------------------------
##    option = int(input(web(loc)
##    if option == 1 and inv[2] >= 20:
##      inv[2] -= 20 
##    elif option == 1 and inv[2] < 20:
##      print ("can't afford")
##    elif option == 2 and inv[2] >= 30:
##      inv[2] -= 30
##      inv[3] = 1
##    elif option == 2 and inv[2] < 30:
##      print ("can't afford")
##    return(inv)



##boop =[]
##with open(r"C:\Users\Matt\Documents\GitHub\Krog\buildings.txt") as f:
##    reader = csv.reader(f, delimiter=';')
##    for row in reader:
##        boop.append((int(row[0]), row[1], list(row[2])))
##
##
##for i in range(len(boop)):
##    if ' ' in boop[i][2]:
##        boop[i][2].remove(' ')
##        for j in range(len(boop[i][2])):
##            boop[i][2][j] = int(boop[i][2][j])
##
##for k in range(len(boop[2][2])):
##    print(checkItem(boop[2][2][k]))



##trav = 0
##while trav < 100:
##    print("\nYou are in " + str(loc) + ".")
##    print("There are " + str(len(web.edges(loc))) + " roads out of " + str(loc) + ": ")
##    for j in range(len(list(web.neighbors(loc)))):
##        print(j+1, ": " + web[loc][list(web.neighbors(loc))[j]]['description'])
##    trav = int(input("Which road will you travel?\n"))
##
##    loc = list(web.neighbors(loc))[trav-1]


##
##
##with open(r"C:\Users\Matt\Documents\GitHub\Krog\roaddesc.txt") as f:
##    lines = f.read().splitlines()
##
##temp = random.sample(range(len(lines)),2)
##print("a " + str(lines[temp[0]]) + ", " + str(lines[temp[1]]) + " road")
##

##from WorldBuilder import *
##import networkx as nx
##
##web, cap = buildWorld(20)
##loc = 1
##roadmap = list(web.edges)
##
##print(web.edges)
##
##roads = []
##print("\nYou are in " + str(loc) + ".")
##print("There are " + str(len(list(web.neighbors(loc)))) + " roads out of " + str(loc) + ": ")
##
##for j in range(len(list(web.neighbors(loc)))):
##    print(web[loc][list(web.neighbors(loc))[j]]['description'])


##
##capidx = 0
##
###randomly generated node web
##web = nx.barabasi_albert_graph(12,1)
##
###finds city with most roads out, names it the capital
##for i in web.nodes:
###        print("Roads out of " + str(i) + ": " + str(web.edges(i)) + str(len(web.edges(i))))
##    if len(web.edges(i)) > len(web.edges(capidx)):
##        capidx = i
##
###    nx.draw(web)
###    plt.show()
##
##for (u,v,w) in web.edges(data=True):
##    w['discription'] = random.randint(0,10)
##
##print(web.edges)
##a = 0
##while a < 100:
##    a = int(input("a"))
##    b = int(input("b"))
##    print(web[a][b])


