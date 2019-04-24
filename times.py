import random as r

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
    web.graph['hour'] = 8
    web.graph['startingDay'] = r.randrange(1, 28)
    web.graph['startingMonth'] = r.randrange(1, 12)
    web.graph['startingYear'] = r.randrange(100, 200)  # todo monster never heals. Keep throwing characters at it.

    web.graph['day'] = web.graph['startingDay']
    web.graph['month'] = web.graph['startingMonth']
    web.graph['year'] = web.graph['startingYear']
    return web


def now():
    return f"{w.world.graph['year']:04d}{w.world.graph['month']:02d}{w.world.graph['day']:02d}{w.world.graph[
        'hour']:02d}"
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

        w.world.graph['hour'] += 1  # time advances 1 hour at a time

        for events in history:  # check history for any events which have to occur now
            if events.datetime == now():
                doEvent(events)

        for j in pe.persons:  # check each person entity if they have a time-based event
            if j.eventTimer == 0:
                j.eventTimer -= 1  # todo prevent timer from growing infinately
                # personEvent(j) todo re-add
            j.eventTimer = int(j.eventTimer) - 1

        if w.world.graph['hour'] >= 24:
            w.world.graph['hour'] -= 24
            w.world.graph['day'] += 1
        if w.world.graph['day'] >= 29:
            w.world.graph['day'] -= 28
            w.world.graph['month'] += 1
        if w.world.graph['month'] >= 13:
            w.world.graph['month'] -= 12
            w.world.graph['year'] += 1
    g.gwin.timeL['text'] = f"Time: {w.world.graph['hour']}:00"

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
    if target in [o.target for o in history] and event in [p.event for p in
                                                           history]:  # todo event for items moving, buying/selling/looting
        w.world.graph['instability'] += 1

    history.append(
        notableEvent(datetime, person, event, target, location,
                     extra))  # todo create event for deal damage to Ultra Krog


def printHistory():
    with open('world/historyLog.txt', 'w') as f:
        for i in history:
            f.write(
                f"at {i.datetime}, {i.person} {i.event} {i.target.name} {i.target.entityID} for {i.extra} in {i.location}\n")

    f.close()


def doEvent(e):
    if e.event == 'kills':
        e.currentHP = 0
    if e.event == 'died':
        pe.createBody(e)
    if e.event == 'wounds':
        e.currentHP -= e.extra
