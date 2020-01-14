import random as r

import networkx as nx

import people as pe
import worlds as w


def findBoss():
    global kingKrog

    for baddie in pe.persons:
        if baddie.name == 'King krog':
            kingKrog = baddie


def createBoss():
    global kingKrog
    kingKrog = pe.createPerson(4, 1, 'King krog')
    setattr(kingKrog, 'attackTarget', None)
    setattr(kingKrog, 'sleepTimer', 5)
    setattr(kingKrog, 'location', 83)
    setattr(kingKrog, 'travelling', False)
    setattr(kingKrog, 'travelRemaining', 0)
    setattr(kingKrog, 'awake', False)


def awaken():
    global kingKrog

    kingKrog.awake = True


def attackTown():
    global kingKrog

    loc = kingKrog.location
    targ = getattr(kingKrog, 'attackTarget')

    if targ is None:
        if len([o for o in w.world.nodes[loc]['sites'] if not o.destroyed]) == 0:
            w.world.nodes[loc]['ruined'] = True
            print(f'{loc} left in ruins')
            bossTravel()
        else:
            setattr(kingKrog, 'attackTarget', r.choice([o for o in w.world.nodes[loc]['sites'] if not o.destroyed]))
            targ = getattr(kingKrog, 'attackTarget')
    else:
        targ.open = False
        targ.currentHP -= 1
        print(f'attacks {targ.type}')
        # todo wear all items in stock

        if targ.currentHP <= 0:
            setattr(targ, 'destroyed', True)
            print(f'destroys {targ.name}')
            setattr(kingKrog, 'attackTarget', None)

            if len([o for o in w.world.nodes[loc]['sites'] if not o.destroyed]) <= 0:
                w.world.nodes[loc]['ruined'] = True
                print(f'{loc} left in ruins')
                bossTravel()
            # else:
            #    setattr(kingKrog, 'attackTarget', r.choice([o for o in w.world.nodes[loc]['sites'] if not o.destroyed]))


def bossTravel():
    global kingKrog

    loc = kingKrog.location
    try:
        dest = r.choice([o for o in list(w.world.neighbors(loc)) if not w.world.nodes[o]['ruined']])
    except IndexError:
        dest = findClosestNotRuin(loc)
    print(f'leaving {loc} for {dest}')
    setattr(kingKrog, 'travelling', True)
    setattr(kingKrog, 'location', [loc, dest])
    try:
        setattr(kingKrog, 'travelRemaining', findDistance(loc, dest))
    except:
        print("dd")


def bossArrive():
    global kingKrog
    setattr(kingKrog, 'travelling', False)
    kingKrog.location = kingKrog.location[1]
    print(f'arrives in {kingKrog.location}')


def findClosestNotRuin(loc):
    distance = 10000
    closestNode = -1

    for n in w.world.nodes:
        if not w.world.nodes[n]['ruined']:
            if len(nx.shortest_path(w.world, loc, n)) < distance and loc != n:
                closestNode = n
                distance = len(nx.shortest_path(w.world, loc, n))

    if closestNode != -1:
        return closestNode
    else:
        print("The world has ended...")
        quit()


def findDistance(start, end):
    route = nx.shortest_path(w.world, start, end)
    distance = 0

    for idx, val in enumerate(route):
        if idx > 0:
            distance += w.world.edges[val, route[idx - 1]]['route'].length

    return distance
