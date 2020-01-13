import random as r

# import weakref as wr
import boss as b
import gui as g
import people as pe
import worlds as w

history = []


class notableEvent:
    def __init__(self, datetime, person, event, target, location, extra):
        self.datetime = datetime
        self.person = person
        self.event = event
        self.target = target
        self.location = location
        self.extra = extra


def createCalendar(web):  # todo make seasons, randomize months and seasons, etc

    setattr(web, 'startingDay', r.randrange(1, 28))
    setattr(web, 'startingMonth', r.randrange(1, 12))
    setattr(web, 'startingYear', r.randrange(100, 200))

    setattr(web, 'hour', 8)
    setattr(web, 'day', web.startingDay)
    setattr(web, 'month', web.startingMonth)
    setattr(web, 'year', web.startingYear)

    return web

def now():
    return f"{w.world.graph['year']:04d}{w.world.graph['month']:02d}{w.world.graph['day']:02d}{w.world.graph['hour']:02d}"
    # str(w.world.graph['year']) + str(w.world.graph['month']) + str(w.world.graph['day']) + str(w.world.graph['hour'])


def timePasses(timePassed=1, byThe='hour'):  # todo player gets sleepy and hungry
    global world

    if byThe == 'day':
        timePassed *= 24
    elif byThe == 'month':
        timePassed *= (24 * 28)
    elif byThe == 'year':
        timePassed *= (24 * 28 * 12)

    for i in range(timePassed):
        w.runWorld(1)
        w.world.hour += 1  # time advances 1 hour at a time
        #pe.me.timeAwake += 1  #todo make this do something
        #pe.me.hunger += 1


        if b.kingKrog.sleepTimer >= 0:
            b.kingKrog.sleepTimer -= 1
        elif b.kingKrog.sleepTimer == 0:
            b.awaken()

        if b.kingKrog.travelling:
            b.kingKrog.travelRemaining -= 1
            if b.kingKrog.travelRemaining <= 0:
                b.bossArrive()
        else:
            b.attackTown()


        for events in history:  # check history for any events which have to occur now
            if events.datetime == now():
                doEvent(events)         #todo all of economics has no events

        for j in pe.persons:  # check each person entity if they have a time-based event
            if j.eventTimer == 0:
                j.eventTimer -= 1  # todo prevent timer from growing infinately
                # personEvent(j) todo re-add
            j.eventTimer = int(j.eventTimer) - 1

        if w.world.hour >= 24:
            w.world.hour -= 24
            w.world.day += 1
            newDay()
        if w.world.day >= 29:
            w.world.day -= 28
            w.world.month += 1
            newMonth()
        if w.world.month >= 13:
            w.world.month -= 12
            w.world.year += 1
            newYear()

    g.gwin.timeL['text'] = f"Time: {w.world.hour}:00"
    g.gwin.dateL["text"] = f"Date: {w.world.month}/{w.world.day}/{w.world.year}"

def newDay():
    PAY_RATE = 2
    wearRate = [0,1,2,3]

    for n in w.world.nodes:
        for s in w.world.nodes[n]['sites']:
            pop = len(w.world.nodes[n]['population'])
            if s.economic:
                for e in s.employees:
                    if s.money >= PAY_RATE:
                        s.money -= PAY_RATE
                        e.money += PAY_RATE
                    else:
                        #print("employee quits")
                        s.employees.remove(e)
                for i in s.stocks:
                    if i.storeStock and len(i.entities) > 0:
                        buyChance = max(1, int(100000 / i.item.cost * pop * 5) / 1000)
                        if r.randrange(1,101) <= buyChance:
                            buyer = w.world.nodes[n]['population'][0]
                            if buyer.money >= i.item.cost:
                                buyer.inv.append(i.entities.pop(0))
                                buyer.money -= i.item.cost
                                s.money += i.item.cost
    '''
    for i in it.items:
        if i.wears:
            i.wear += r.choice(wearRate)
            if i.wear >= 40:
                print("d")
                #del i

                #todo weak refs will fix this
    '''
def newMonth():
    TAXES = 20

    for n in w.world.nodes:
        for s in w.world.nodes[n]['sites']:
            if s.economic:
                if s.area == 'krog':
                    if s.money >= TAXES:
                        s.money -= TAXES
                    else:
                        #print("store closes")
                        s.open = False

def newYear():
    pass

def personEvent(pers):
    if pers.eventType == 'grow' and pers.currentHP >= 1:
        if pe.personTypeList[pers.eventTarget].name == 'null':
            pe.persons[pers.entityID].name = pe.personTypeList[pers.eventTarget].personType

        pe.persons[pers.entityID].currentHP += pe.personTypeList[pers.eventTarget].stats[0] - \
                                               pe.persons[pers.entityID].stats[0]

        pe.persons[pers.entityID].stats = pe.personTypeList[pers.eventTarget].stats
        pe.persons[pers.entityID].personType = pe.personTypeList[pers.eventTarget].personType
        pe.persons[pers.entityID].eventTimer = pe.personTypeList[pers.eventTarget].eventTimer
        pe.persons[pers.entityID].eventType = pe.personTypeList[pers.eventTarget].eventType
        pe.persons[pers.entityID].eventTarget = pe.personTypeList[pers.eventTarget].eventTarget

    if pers.eventType == 'awaken':
        print("The entire earth trembles beneath your feet.")
        pe.persons[pers.entityID].eventTarget = 1  # todo check time passes for ultra krog awake
        pe.persons[pers.entityID].eventType = 'destroy'

    if pers.eventType == 'destroy':
        print('destory')


def createEvent(datetime, person, event, target, location, extra=0):
    pe.savePlayer()

    if target in [o.target for o in history] and event in [p.event for p in
                                                           history]:  # todo event for items moving, buying/selling/looting
        w.world.graph['instability'] += 1

    history.append(
        notableEvent(datetime, person, event, target, location,
                     extra))  # todo create event for deal damage to Ultra krog


def printHistory():
    with open('world/historyLog.txt', 'w') as f:
        for i in history:
            f.write(
                f"at {i.datetime}, {i.person} {i.event} {i.target.name} {i.target.entityID} for {i.extra} in {i.location}\n")

    f.close()


def doEvent(e):
    if e.event == 'kills':
        e.target.currentHP = 0
    if e.event == 'died':
        pe.createBody(e)
    if e.event == 'wounds':
        e.target.currentHP -= e.extra
