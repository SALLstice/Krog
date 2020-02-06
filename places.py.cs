/*
using csv;

using r = random;

using time;

using nx = networkx;

using ev = events;

using g = gui;

using it = items;

using c = newCombat;

using pe = people;

using t = times;

using w = worlds;

using System.Collections.Generic;

using System.Collections;

using System.Linq;

using System;

public static class places {
    
    public static List<object> placeTypeList = new List<object>();
    
    public static List<object> places = new List<object>();
    
    public static List<object> PLACE_HEADERS = new List<object>();
    
    public class place {
        
        public object currentHP;
        
        public bool destroyed;
        
        public object name;
        
        public bool open;
        
        public place(object name, object currentHP) {
            this.name = name;
            this.currentHP = currentHP;
            this.open = true;
            this.destroyed = false;
        }
    }
    
    // 
    //     def __init__(self, type, use, area, known, inv, maxHP, extraSiteOption, recipes):
    //         self.type = type
    //         self.use = use
    //         self.area = area
    //         self.known = known
    //         self.inv = inv
    //         self.maxHP = maxHP
    //         self.extraSiteOption = extraSiteOption
    //         self.recipes = recipes
    //     
    public class placeType {
        
        public placeType(Hashtable kwargs, params object [] args) {
            foreach (var each in PLACE_HEADERS) {
                setattr(this, each, args);
            }
        }
    }
    
    public static object initPlaceTypeList() {
        using (var f = open("placeList.csv")) {
            reader = csv.reader(f);
            headers = next(reader);
            foreach (var row in reader) {
                placeTypeList.append(new placeType(headers));
                foreach (var _tup_1 in headers.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                    idx = _tup_1.Item1;
                    attr = _tup_1.Item2;
                    if (type(attr) == str) {
                        attr = attr.strip();
                    }
                    try {
                        tempval = Convert.ToInt32(row[idx]);
                    } catch {
                        if (new List<string> {
                            "inv",
                            "recipes"
                        }.Contains(attr)) {
                            tempval = row[idx].split();
                            tempval = (from x in tempval
                                select Convert.ToInt32(x)).ToList();
                        } else {
                            tempval = row[idx].strip();
                        }
                    }
                    setattr(placeTypeList[placeTypeList.Count - 1], attr, tempval);
                }
            }
        }
    }
    
    public static List<object> createPlace(object sTID, string name = "", int currentHP = -500) {
        // todo gain ability to reverse lookup sites by location
        var inv = new List<object>();
        if (type(sTID) == str) {
            sTID = findTID(sTID);
        }
        if (currentHP == -500) {
            currentHP = placeTypeList[sTID].maxHP;
        }
        places.append(new place(name, currentHP));
        foreach (var _tup_1 in placeTypeList[0].@__dict__.keys().ToList().Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            var val = _tup_1.Item1;
            var attr = _tup_1.Item2;
            if (new List<string> {
                "inv",
                "recipes"
            }.Contains(attr)) {
                var templist = new List<object>();
                var invList = getattr(placeTypeList[sTID], attr);
                if (invList != 0) {
                    foreach (var i in invList) {
                        templist.append(it.createItem(i));
                    }
                }
                setattr(places[places.Count - 1], attr, templist);
            } else {
                setattr(places[places.Count - 1], attr, getattr(placeTypeList[sTID], attr));
            }
        }
        return places[Convert.ToInt32(places.Count - 1)];
    }
    
    public static void findTID(object inplT) {
        if (type(inplT) == str) {
            foreach (var _tup_1 in placeTypeList.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                var idx = _tup_1.Item1;
                var plT = _tup_1.Item2;
                if (inplT == plT.type) {
                    var plTID = idx;
                    break;
                }
            }
        }
        return plTID;
    }
    
    public static void arrive() {
        g.updateStatus();
        if (pe.kingKrog.location == pe.me.location) {
            g.clearText();
            g.setText(label4: "The King krog Is Here!");
            c.initCombat(pe.kingKrog);
        } else {
            g.dispTown();
        }
    }
    
    public static void visitRegionPlace() {
        // todo split lodInfo into branch options, shop, rumors, revisit known wild location, krog inventory, food stores, etc
        // print("Shop\nRumors\nInfo") #todo make rumors and info
        // g.setText(label4 = f"You are in {loc}.")
        var regSites = new List<object>();
        //todo once you learn krog count, make that available in loc info. note "As of datetime, there were X Krogs in loc"
        //prints out every known site in location
        foreach (var site in w.world.nodes[pe.me.location]["sites"]) {
            if (site.area == "krog") {
                regSites.append(site);
            }
            if (site.area == "wild" && site.known == 1) {
                regSites.append(site);
            }
        }
        // dispList = [o.itemType for o in pe.me.inv]
        var display = "You are in {pe.me.location}.";
        g.initSelect(display, regSites, "", "type", "site", "krog");
        // siteActivity(places[w.world.nodes[loc]['sites'][whereGo]])
    }
    
    public static void siteActivity(object store) {
        //Shops
        if (store.use == "store") {
            // todo note how damaged the building if it is damaged
            g.clearText();
            g.setText(label4: "You are at {store.name} {store.type}.");
            g.gwin.button0["text"] = "Buy";
            g.gwin.button0["command"] = () => it.displayItems(store);
            g.gwin.button1["text"] = "Sell";
            g.gwin.button1["command"] = () => it.inventory(pe.me, "sell", "krog", sellTo: store);
            if (store.extraSiteOption != "") {
                g.gwin.button2["text"] = store.extraSiteOption;
                g.gwin.button2["command"] = () => siteExtra(store);
            } else {
                g.gwin.button3["text"] = "-";
                g.gwin.button3["command"] = "";
                // g.setButtons("Buy","pl.buyItem(store)", "Sell", "sellItem", "Return", "dispTown")
            }
        } else if (store.use == "druid") {
            druid(store);
        } else if (store.use == "inn") {
            inn(store);
        } else if (store.use == "witch") {
            witch(store);
        }
        Func<object, object> siteExtra = store => {
            if (store.extraSiteOption == "Brew") {
                g.clearText();
                g.initSelect("Brew Potions", store, "recipes", new List<string> {
                    "itemType"
                }, "brew", "krog");
            }
        };
    }
    
    public static void setupTravelRT(object dest) {
        g.clearText();
        g.gwin.button1.grid_remove();
        g.gwin.button2.grid_remove();
        g.gwin.button3.grid_remove();
        pe.me.exploring = 1;
        var loc = pe.me.location;
        var cityname = w.world.nodes[dest]["name"];
        if (w.world[loc][dest]["route"].known == 1) {
            g.setText(label4: "Travelling to {cityname}.");
        } else {
            g.setText(label4: "Travelling along {w.world[loc][dest]['route'].desc}.");
        }
        g.gwin.button0["text"] = "Turn Back";
        g.gwin.update();
        g.gwin.button0["command"] = 0;
        var lengthOfTrip = w.world[loc][dest]["route"].length;
        //t.createEvent(t.now(), pe.me.name, 'departs', dest, loc)  # create depart event todo make
        var encounteredEntity = -1;
        var timer = 0;
        var montage = new List<string> {
            "",
            ".",
            ".  .",
            ".  .  ."
        };
        realTimeActivity(timer, "r", montage, "travel", dest, lengthOfTrip);
    }
    
    public static void setupExploreRT() {
        pe.me.exploring = 1;
        g.setText(label4: "Exploring");
        g.gwin.button0["text"] = "Stop Exploring";
        //g.gwin.update()
        g.gwin.button0["command"] = () => stopExplore();
        if (pe.me.foraging) {
            g.gwin.button1["text"] = "Stop Sneaking";
        } else {
            g.gwin.button1["text"] = "Sneak";
        }
        g.gwin.button1["command"] = () => toggleSneak();
        if (pe.me.foraging) {
            g.gwin.button2["text"] = "Stop Foraging";
        } else {
            g.gwin.button2["text"] = "Forage";
        }
        g.gwin.button2["command"] = () => toggleForage();
        //encounteredEntity = -1
        var timer = 0;
        var montage = new List<string> {
            "",
            ".",
            ".  .",
            ".  .  ."
        };
        realTimeActivity(timer, "rmws", montage, "explore");
    }
    
    public static void realTimeActivity(
        int timer,
        string encounterCode,
        List<string> montage,
        string activity,
        int dest = -1,
        int journeyLength = -1,
        int progress = 0) {
        var ENCOUNTER_RATE = 0.25;
        var encounterType = 0;
        var encounteredEntity = -1;
        var FORAGE_ENC_RATE = 0.2;
        var timerRollover = 4;
        //durationOfTrip = max(1,m.ceil(int((w.world[loc][dest]['route'].length) / pe.me.overlandSpeed)*w.world[loc][dest]['route'].roughness))
        time.sleep(0.5);
        g.setText(label5: montage[timer]);
        timer += 1;
        timer %= timerRollover;
        if (pe.me.sneaking && timer == timerRollover / 2) {
            if (!pe.skillCheck("Sneaking")) {
                t.timePasses();
            }
        }
        if (timer == 0) {
            t.timePasses();
            var modEncRate = ENCOUNTER_RATE;
            if (pe.me.sneaking) {
                modEncRate -= Convert.ToInt32(pe.getSkill("Sneaking") / 5);
            }
            if (pe.me.foraging) {
                modEncRate = +FORAGE_ENC_RATE;
                if (r.randrange(3) == 0) {
                    //easy 33% chance of maybe finding something
                    if (pe.skillCheck("Foraging")) {
                        pe.me.inv.append(it.createItem("Healing Herbs"));
                        g.setText(label8: "You find Healing Herbs");
                    }
                }
            }
            var _tup_1 = randomEncounter(encounterCode, modEncRate);
            encounteredEntity = _tup_1.Item1;
            encounterType = _tup_1.Item2;
            // todo increase encounter chance based on number of krog
            //todo have button to flag for forage. disp on label6 item found until timer resets label7 skill increase, increase enc rate
            if (activity == "travel") {
                progress += pe.me.overlandSpeed * w.world[pe.me.location][dest]["route"].roughness;
                if (progress >= journeyLength) {
                    pe.me.exploring = 2;
                    w.world[pe.me.location][dest]["route"].known = 1;
                    pe.me.location = dest;
                    g.clearText();
                    g.setText(label4: "You walked for {journeyLength} miles and arrive in {w.world.nodes[dest]['name']}.");
                    // todo make travel work like ExploreRT
                    // todo time passes
                    //t.createEvent(t.now(), pe.me.name, 'arrives', dest, dest)  # todo re-add
                    g.gwin.button0["text"] = "Continue";
                    g.gwin.button0["command"] = () => arrive();
                }
            }
        }
        if (encounteredEntity != -1) {
            if (encounterType == "m") {
                pe.me.exploring = 2;
                var status = "";
                if (encounteredEntity.currentHP <= 0) {
                    status = "dead ";
                } else if (encounteredEntity.currentHP < encounteredEntity.maxHP) {
                    status = "wounded ";
                }
                g.setText(label4: "You encounter a {status}{encounteredEntity.name}");
                if (status != "dead ") {
                    g.gwin.button0["text"] = "Enter Combat";
                    g.gwin.button0["command"] = () => c.initCombat(encounteredEntity);
                } else {
                    var display = "Loot:";
                    g.gwin.button0["text"] = "Loot";
                    g.gwin.button0["command"] = () => g.initSelect(display, encounteredEntity, "inv", "itemType", "loot", "dispTown");
                    g.gwin.button1["text"] = "Keep Exploring";
                    g.gwin.button1["command"] = () => setupExploreRT();
                    g.gwin.button1.grid();
                    g.gwin.button2["text"] = "Stop Exploring";
                    g.gwin.button2["command"] = () => g.dispTown();
                    g.gwin.button2.grid();
                    // combatResult = c.combat(encounteredEntity)
                }
            }
            if (encounterType == "s") {
                pe.me.exploring = 2;
                g.setText(label4: "You encounter a {encounteredEntity.type}");
                encounteredEntity.known = 1;
                g.gwin.button0["text"] = "Continue";
                g.gwin.button0["command"] = () => siteActivity(encounteredEntity);
            }
            if (encounterType == "e") {
                pe.me.exploring = 2;
                ev.runEvent(encounteredEntity);
            }
        }
        if (pe.me.exploring == 1) {
            realTimeActivity(timer, encounterCode, montage, activity, dest, journeyLength, progress);
        }
        if (pe.me.exploring == 0) {
            g.gwin.button1.grid();
            g.gwin.button2.grid();
            g.gwin.button3.grid();
            arrive();
        }
    }
    
    public static void stopExplore() {
        pe.me.exploring = 0;
    }
    
    public static void toggleSneak() {
        if (!pe.me.sneaking) {
            pe.me.sneaking = true;
            g.gwin.button1["text"] = "Stop Sneaking";
        } else {
            pe.me.sneaking = false;
            g.gwin.button1["text"] = "Sneak";
        }
    }
    
    public static void toggleForage() {
        if (!pe.me.foraging) {
            pe.me.foraging = true;
            g.gwin.button2["text"] = "Stop Foraging";
        } else {
            pe.me.foraging = false;
            g.gwin.button2["text"] = "Forage";
        }
    }
    
    public static object randomEncounter(string encounterCode, double encounterRate) {
        var encounterList = new List<object>();
        if (pe.me.name == "Druid") {
            //for debugging
            pe.me.inv.append(it.createItem("krog Guts"));
            pe.me.inv.append(it.createItem("krog Guts"));
            foreach (var i in w.world.nodes[pe.me.location]["sites"]) {
                if (i.area == "wild") {
                    encounterList.append(new List<string> {
                        i,
                        "s"
                    });
                }
            }
        } else if (encounterCode == "rmws") {
            //rmws: region monsters and wild sites
            foreach (var i in w.world.nodes[pe.me.location]["monsters"]) {
                // generate encounter list from monsters and wild sites
                encounterList.append(new List<string> {
                    i,
                    "m"
                });
            }
            foreach (var i in w.world.nodes[pe.me.location]["sites"]) {
                if (i.area == "wild") {
                    encounterList.append(new List<string> {
                        i,
                        "s"
                    });
                    // todo check if anyone is travelling to encoutner them
                }
            }
        }
        if (encounterCode == "r") {
            //t: travel
            foreach (var e in ev.eventList) {
                if (e.location == "road") {
                    //todo random events on the road
                    encounterList.append(new List<string> {
                        e,
                        "e"
                    });
                    //todo make random enoucnters more common the more populated the region is 2 * (infestation + 4)
                }
            }
        }
        if (encounterList.Count >= 1) {
            if (r.random() <= encounterRate) {
                //determine random enounter, if any
                var found = r.randrange(encounterList.Count);
                return Tuple.Create(encounterList[found][0], encounterList[found][1]);
            }
        }
        return Tuple.Create(-1, -1);
    }
    
    public static void druid(object drood) {
        //todo add option to trade krog teeth for curative herbs
        //todo druid doesn't become known until payedextra. make even for becoming known
        g.clearText();
        g.setText(label4: "Give me 2 krog Guts and I can tell you how many Krogs are left in a region.");
        var gutCount = 0;
        var guts = new List<object>();
        var gutList = it.checkInvForItems("krog Guts", 2);
        g.gwin.button0["command"] = () => checkGuts(gutList);
        Func<object, object> checkGuts = gutCount => {
            if (gutCount.Count >= 2) {
                // if the player has enough guts and gives them up
                g.setText(label4: "Which region? ", label6: "", label7: "");
                g.gwin.button0["text"] = "Confirm";
                g.gwin.button0["command"] = () => giveGuts(Convert.ToInt32(g.gwin.textInput.get()));
            } else {
                arrive();
            }
            Func<object, object> giveGuts = regionToSearch => {
                var krogCount = 0;
                foreach (var mon in w.world.node[regionToSearch]["monsters"]) {
                    // count all living krogs in the node
                    if (mon.currentHP >= 1) {
                        krogCount += 1;
                    }
                }
                g.setText(label4: "There are {krogCount} krogs still alive around {regionToSearch}.");
                it.removeItemsFromInv(2, "krog Guts");
                g.gwin.button0["command"] = () => arrive();
            };
        };
    }
    
    public static void inn(object store) {
        g.gwin.button0["text"] = "Gather Rumors";
        g.gwin.button0["command"] = () => rumors();
        g.gwin.button1["text"] = "Get directions";
        g.gwin.button1["command"] = () => directions();
        g.gwin.button2["text"] = "Rent a Room";
        g.gwin.button2["command"] = () => rentRoom();
        g.gwin.button3["text"] = "Return";
        g.gwin.button3["command"] = () => pl.arrive();
        Func<object> rumors = () => {
            //todo learn neighboring cities
            //todo learn locations of druid, dungeons, etc
            //todo if a PC does well, create rumors of them
            //todo location of king krog
            //todo side quests
        };
        Func<object> directions = () => {
            var dest = Convert.ToInt32(input("To where? "));
            var route = nx.shortest_path(w.world, pe.me.location, dest);
            t.timePasses();
            if (route.Count >= 6) {
                Console.WriteLine("Never heard of it.");
            } else if (route.Count < 6 && route.Count >= 4) {
                Console.WriteLine("Not sure exactly, but I know you have to travel {w.world[pe.me.location][route[1]]['description']} to {route[1]} to get there.");
            } else if (route.Count < 4) {
                Console.WriteLine(route);
            }
        };
        Func<object> rentRoom = () => {
            // todo option to keep things in your room and rent for X days, etc
            g.clearText();
            g.setText(label4: "Sleep for how many hours?");
            g.gwin.button0["text"] = "Continue";
            g.gwin.button0["command"] = () => sleep();
            g.gwin.button3["text"] = "Return";
            g.gwin.button3["command"] = () => arrive();
            Func<object> sleep = () => {
                var sleepTime = Convert.ToInt32(g.gwin.textInput.get());
                t.timePasses(sleepTime);
                var healed = sleepTime * Convert.ToInt32(pe.me.tough / 2);
                pe.me.currentHP = min(pe.me.maxHP, pe.me.currentHP + healed);
                g.setText(label4: "You sleep for {sleepTime} hours.");
                g.setText(label5: "You heal {healed}.");
                g.updateStatus();
                g.gwin.button0["command"] = () => arrive();
            };
        };
    }
    
    public static void witch(object store) {
    }
}
*/