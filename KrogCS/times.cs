//using r = random;
//using b = boss;
//using g = gui;
using pe = people;
using w = worlds;
using System.Collections.Generic;
using System;
using System.Linq;

public static class times {
    
    public static List<object> history = new List<object>();
    
    public class notableEvent {
        
        public object datetime;
        
        public object @event;
        
        public object extra;
        
        public object location;
        
        public object person;
        
        public object target;
        
        public notableEvent(
            object datetime,
            object person,
            object @event,
            object target,
            object location,
            object extra) {
            this.datetime = datetime;
            this.person = person;
            this.@event = @event;
            this.target = target;
            this.location = location;
            this.extra = extra;
        }
    }
    /*
    public static void createCalendar(object web) {
        // todo make seasons, randomize months and seasons, etc
        setattr(web, "startingDay", r.randrange(1, 28));
        setattr(web, "startingMonth", r.randrange(1, 12));
        setattr(web, "startingYear", r.randrange(100, 200));
        setattr(web, "hour", 8);
        setattr(web, "day", web.startingDay);
        setattr(web, "month", web.startingMonth);
        setattr(web, "year", web.startingYear);
        return web;
    }
    */
    /*
    public static string now() {
        return "{w.world.year:04d}{w.world.month:02d}{w.world.day:02d}{w.world.hour:02d}";
        // str(w.world.graph['year']) + str(w.world.graph['month']) + str(w.world.graph['day']) + str(w.world.graph['hour'])
    }
    */
    /*
    public static void timePasses(int timePassed = 1, string byThe = "hour") {
        // todo player gets sleepy and hungry
        if (byThe == "day") {
            timePassed *= 24;
        } else if (byThe == "month") {
            timePassed *= 24 * 28;
        } else if (byThe == "year") {
            timePassed *= 24 * 28 * 12;
        }
        foreach (var i in Enumerable.Range(0, timePassed)) {
            w.runWorld(1);
            w.world.hour += 1;
            //pe.me.timeAwake += 1  #todo make this do something
            //pe.me.hunger += 1
            if (b.kingKrog.sleepTimer >= 0) {
                b.kingKrog.sleepTimer -= 1;
            } else if (b.kingKrog.sleepTimer == 0) {
                b.awaken();
            }
            if (b.kingKrog.travelling) {
                b.kingKrog.travelRemaining -= 1;
                if (b.kingKrog.travelRemaining <= 0) {
                    b.bossArrive();
                }
            } else {
                b.attackTown();
            }
            foreach (var events in history) {
                // check history for any events which have to occur now
                if (events.datetime == now()) {
                    doEvent(events);
                }
            }
            foreach (var j in pe.persons) {
                // check each person entity if they have a time-based event
                if (j.eventTimer == 0) {
                    j.eventTimer -= 1;
                    // personEvent(j) todo re-add
                }
                j.eventTimer = Convert.ToInt32(j.eventTimer) - 1;
            }
            if (w.world.hour >= 24) {
                w.world.hour -= 24;
                w.world.day += 1;
                newDay();
            }
            if (w.world.day >= 29) {
                w.world.day -= 28;
                w.world.month += 1;
                newMonth();
            }
            if (w.world.month >= 13) {
                w.world.month -= 12;
                w.world.year += 1;
                newYear();
            }
        }
        g.gwin.timeL["text"] = "Time: {w.world.hour}:00";
        g.gwin.dateL["text"] = "Date: {w.world.month}/{w.world.day}/{w.world.year}";
    }
    
    public static void newDay() {
        var PAY_RATE = 2;
        var wearRate = new List<int> {
            0,
            1,
            2,
            3
        };
        foreach (var n in w.world.nodes) {
            foreach (var s in w.world.nodes[n]["sites"]) {
                var pop = w.world.nodes[n]["population"].Count;
                if (s.economic) {
                    foreach (var e in s.employees) {
                        if (s.money >= PAY_RATE) {
                            s.money -= PAY_RATE;
                            e.money += PAY_RATE;
                        } else {
                            //print("employee quits")
                            s.employees.remove(e);
                        }
                    }
                    foreach (var i in s.stocks) {
                        if (i.storeStock && i.entities.Count > 0) {
                            var buyChance = max(1, Convert.ToInt32(100000 / i.item.cost * pop * 5) / 1000);
                            if (r.randrange(1, 101) <= buyChance) {
                                var buyer = w.world.nodes[n]["population"][0];
                                if (buyer.money >= i.item.cost) {
                                    buyer.inv.append(i.entities.pop(0));
                                    buyer.money -= i.item.cost;
                                    s.money += i.item.cost;
                                }
                            }
                        }
                    }
                }
            }
        }
        @"
    for i in it.items:
        if i.wears:
            i.wear += r.choice(wearRate)
            if i.wear >= 40:
                print(""d"")
                #del i

                #todo weak refs will fix this
    ";
    }
    
    public static void newMonth() {
        var TAXES = 20;
        foreach (var n in w.world.nodes) {
            foreach (var s in w.world.nodes[n]["sites"]) {
                if (s.economic) {
                    if (s.area == "krog") {
                        if (s.money >= TAXES) {
                            s.money -= TAXES;
                        } else {
                            //print("store closes")
                            s.open = false;
                        }
                    }
                }
            }
        }
    }
    
    public static void newYear() {
    }
    
    public static void personEvent(object pers) {
        if (pers.eventType == "grow" && pers.currentHP >= 1) {
            if (pe.personTypeList[pers.eventTarget].name == "null") {
                pe.persons[pers.entityID].name = pe.personTypeList[pers.eventTarget].personType;
            }
            pe.persons[pers.entityID].currentHP += pe.personTypeList[pers.eventTarget].stats[0] - pe.persons[pers.entityID].stats[0];
            pe.persons[pers.entityID].stats = pe.personTypeList[pers.eventTarget].stats;
            pe.persons[pers.entityID].personType = pe.personTypeList[pers.eventTarget].personType;
            pe.persons[pers.entityID].eventTimer = pe.personTypeList[pers.eventTarget].eventTimer;
            pe.persons[pers.entityID].eventType = pe.personTypeList[pers.eventTarget].eventType;
            pe.persons[pers.entityID].eventTarget = pe.personTypeList[pers.eventTarget].eventTarget;
        }
        if (pers.eventType == "awaken") {
            Console.WriteLine("The entire earth trembles beneath your feet.");
            pe.persons[pers.entityID].eventTarget = 1;
            pe.persons[pers.entityID].eventType = "destroy";
        }
        if (pers.eventType == "destroy") {
            Console.WriteLine("destory");
        }
    }
    
    public static void createEvent(
        object datetime,
        object person,
        object @event,
        object target,
        object location,
        int extra = 0) {
        pe.savePlayer();
        if ((from o in history
            select o.target).ToList().Contains(target) && (from p in history
            select p.@event).ToList().Contains(@event)) {
            // todo event for items moving, buying/selling/looting
            w.world.graph["instability"] += 1;
        }
        history.append(new notableEvent(datetime, person, @event, target, location, extra));
    }
    
    public static void printHistory() {
        using (var f = open("world/historyLog.txt", "w")) {
            foreach (var i in history) {
                f.write("at {i.datetime}, {i.person} {i.event} {i.target.name} {i.target.entityID} for {i.extra} in {i.location}\n");
            }
        }
        f.close();
    }
    
    public static void doEvent(object e) {
        if (e.@event == "kills") {
            e.target.currentHP = 0;
        }
        if (e.@event == "died") {
            pe.createBody(e);
        }
        if (e.@event == "wounds") {
            e.target.currentHP -= e.extra;
        }
    }
    */
}
