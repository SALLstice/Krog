import gui as g
import csv

EVENT_HEADERS = []
eventList = []

class event:
    def __init__(self, *args, **kwargs):
        for each in EVENT_HEADERS:
            setattr(self, each, args)

def initEvents():
    global EVENT_HEADERS
    global eventList

    with open('events.csv') as evf:
        reader = csv.reader(evf)
        EVENT_HEADERS = next(reader)

        for row in reader:
            eventList.append(event(row))
            for idx, attr in enumerate(EVENT_HEADERS):
                try:
                    tempval = eval(row[idx])
                except:
                    tempval = row[idx]
                setattr(eventList[len(eventList) - 1], attr, tempval)

def runEvent(e):
    g.initSelect(e.introText,e,"options",'','event','town')

def eventResults(result):
    if result == "greed":
        print("You feel greedy")

    if result == 'beggardivine':
        if pe.me.money >= 1:
            pe.me.money -= 1
