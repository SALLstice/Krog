
//using csv;
//using p = pickle;
//using r = random;
//using render = django.shortcuts.render;
//using g = gui;
//using it = items;

using w = worlds;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class people {
    
    //public static List<object> persons = new List<object>();    
    //public static List<object> personTypeList = new List<object>();
    //public static List<object> futureDead = new List<object>();
    //public static List<object> PERSON_HEADERS = new List<object>();
     
    public class Person 
    {
        
        //public int currentHP;        
        //public bool employed;        
        //public int entityID;        
        //public object homeLocation;        
        //public job job;              
        public string name;      
        public w.City location;    
        public int money;
        public w.Business jobSite;
        //public object personType;        
        //public object status;        
        //public int wallet;
        
        public Person(string name="", w.City loc = null) 
        {
            //this.name = (name == "") ? w.randomName("Person") : name;
            //this.location = (loc == null) ? main.world.randomCity() : loc;

        }
        
        public void placeInWorld(w.City loc)
        {
            main.world.people.Add(this); //FIXME Dont point to main world
            loc.residents.Add(this);
            this.location = loc;
        }

        public void moveLocation(w.City loc)
        {
            this.location.residents.Remove(this);
            this.location = loc;
        }
        //public virtual int useSkill(object skill, int mod = 0) 
        //{
        //    if (!hasattr(this, skill)) {
        //        setattr(this, skill, 0);
        //    }
        //    var skillLevel = getattr(this, skill);
        //    var result = r.randrange(1, 101) + skillLevel + mod;
        //    if (r.randrange(1, 101) >= skillLevel && skillLevel < 100) {
        //        setattr(this, skill, skillLevel + 1);
        //    }
        //    return result;
        //}
    } 
    
    
    // 
    //     def __init__(self, personType, inv, strength, maxHP, defense, dodge, speed, event, eventTimer, atkRate, atkStr, atkTIBS, atkMod, atkDesc, addInv):
    //         self.personType = personType
    //         self.inv = inv
    //         self.strength = strength
    //         self.maxHP = maxHP
    //         self.defense = defense
    //         self.dodge = dodge
    //         self.speed = speed
    //         self.event = event
    //         self.eventTimer = eventTimer
    //         self.atkRate = atkRate
    //         self.atkStr = atkStr
    //         self.atkTIBS = atkTIBS
    //         self.atkMod = atkMod
    //         self.atkDesc = atkDesc
    //         self.addInv = addInv
    //     

    /*
    public class personType {
        
        public personType(Hashtable kwargs, params object [] args) {
            foreach (var each in PERSON_HEADERS) {
                setattr(this, each, args);
            }
        }
    }
    
    public class dead {
        
        public object deathDate;        
        public object deathLocation;        
        public object entityID;        
        public object inv;        
        public object name;        
        public personType personType;
        
        public dead(
            object entityID,
            object personType,
            object name,
            object inv,
            object deathDate,
            object deathLocation) {
            this.entityID = entityID;
            this.personType = personType;
            this.name = name;
            this.inv = inv;
            this.deathDate = deathDate;
            this.deathLocation = deathLocation;
        }
    }
    */
    public class Player : Person
    {
        
        //public bool awake;        
        //public int hunger;        
        //public w.city location;        
        //public magic magic;        
        //public string name;        
        //public skills skills;        
        //public int timeAwake;
        
        //public bool retreating = false;
        //public bool dodging = false;
        //public bool blocking = false;

        //public int currentHP = 10;
        //setattr(me, "maxHP", 10);
        //setattr(me, "strength", 2);
        //setattr(me, "tough", 2);
        //setattr(me, "overlandSpeed", 3);
        //setattr(me, "TIBS", 50);
        //setattr(me, "speed", 3);
        

        //setattr(me, "attackType", "N");
        //setattr(me, "exploring", 0);
        //setattr(me, "travelling", false);
        //setattr(me, "foraging", false);
        //setattr(me, "sneaking", false);

        public Player(string name) : base(name)
        {

        }
        
    }
    public static Player newPlayer(string name)
    {
        Player newb = new Player(name);
        w.City newLocation = main.world.randomCity(); 

        newb.placeInWorld(newLocation);

        return newb;
    }
    
    public static Person newPerson()
    {
        Person newb = new Person();

        newb.name = w.randomName("city");
        newb.placeInWorld(main.world.randomCity());

        return newb;
    }
    
    /*
    public static void peopleLayout(object request) {
        var varDict = new Dictionary<object, object> {
            {
                "content",
                "this came from people"}};
        return render(request, "krog/personLayout.html", varDict);
    }
    
    public static object initPersonTypeList() 
    {
        using (var f = open("personList.csv")) {
            reader = csv.reader(f);
            PERSON_HEADERS = next(reader);
            foreach (var row in reader) {
                personTypeList.append(new personType(PERSON_HEADERS));
                foreach (var _tup_1 in PERSON_HEADERS.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                    val = _tup_1.Item1;
                    attr = _tup_1.Item2;
                    if (type(attr) == str) {
                        attr = attr.strip();
                    }
                    try {
                        tempval = Convert.ToInt32(row[val]);
                    } catch {
                        if (new List<string> {
                            "inv",
                            "atkRate",
                            "atkStr",
                            "atkTIBS",
                            "atkMod",
                            "addInv"
                        }.Contains(attr)) {
                            tempval2 = row[val].split();
                            tempval = (from x in tempval2
                                select Convert.ToInt32(x)).ToList();
                        } else if (attr == "atkDesc") {
                            tempval = row[val].split();
                        } else {
                            tempval = row[val].strip();
                        }
                    }
                    setattr(personTypeList[personTypeList.Count - 1], attr, tempval);
                }
            }
        }
    }
    */

    

        /*
        
        
        
        Func<object> setMagic = () => {
        };
        Func<object> setSkills = () => {
            setattr(me.skills, "Dodge", 10);
            setattr(me.skills, "Club", 50);
        };
        Func<object, object> setInventory = ininv => {
            var inv = new List<object>();
            foreach (var j in Enumerable.Range(0, ininv.Count)) {
                inv.append(it.createItem(ininv[j]));
            }
            setattr(me, "inv", inv);
            setattr(me, "money", 0);
        };
        Func<object> setEquipment = () => {
            setattr(me, "equippedWeapon", it.createItem("Club"));
            setattr(me, "equippedShield", it.createItem(0));
            setattr(me, "equippedArmor", it.createItem("Clothing"));
            setattr(me, "equippedAcc1", it.createItem(0));
            setattr(me, "equippedAcc2", it.createItem(0));
        };
        setFlags();
        setStats();
        setSkills();
        setInventory(new List<int> {
            5,
            5
        });
        setEquipment();
        setMagic();
    
    */
    /*
    public static List<object> createPerson(
        object pTID,
        int number = 1,
        string name = "null",
        int currentHP = -500,
        int location = -1,
        int homeLocation = -1) {
        var multiAdd = new List<object>();
        foreach (var i in Enumerable.Range(0, number)) {
            var inv = new List<object>();
            if (currentHP == -500) {
                currentHP = personTypeList[pTID].maxHP;
            }
            if (name == "null") {
                //if the person has no name (like a krog), make the name the person type
                var setname = personTypeList[pTID].personType;
            } else if (name == "rand") {
                setname = w.randomName("city");
            } else {
                setname = name;
            }
            persons.append(new person(Convert.ToInt32(persons.Count), personTypeList[pTID], setname, currentHP, 0, location, homeLocation));
            foreach (var _tup_1 in personTypeList[0].@__dict__.keys().ToList().Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                var val = _tup_1.Item1;
                var attr = _tup_1.Item2;
                inv = new List<object>();
                //if attr == 'busy':
                //    setattr(persons[len(persons) - 1], attr, False)
                if (attr == "inv" || attr == "addInv") {
                    var invList = getattr(personTypeList[pTID], attr);
                    if (invList != 0) {
                        foreach (var i in invList) {
                            inv.append(it.createItem(i));
                        }
                    }
                    setattr(persons[persons.Count - 1], attr, inv);
                } else if (attr == "money") {
                    setattr(persons[persons.Count - 1], attr, 0);
                } else {
                    setattr(persons[persons.Count - 1], attr, getattr(personTypeList[pTID], attr));
                }
            }
            multiAdd.append(persons[persons.Count - 1]);
        }
        if (number == 1) {
            return persons[Convert.ToInt32(persons.Count - 1)];
        }
        if (number > 1) {
            return multiAdd;
        }
    }
    
    public static void createBody(object e) {
        persons.append(new person(Convert.ToInt32(persons.Count), personTypeList[2].personType, futureDead[e.person].name, futureDead[e.person].inv, 0, 0, 0, 0, 0));
        w.world.nodes[e.location]["monsters"].append(persons[Convert.ToInt32(persons.Count - 1)].entityID);
    }
    
    public static void dispSkills() {
        var skillList = new List<object>();
        var skil = "";
        foreach (var skil in me.skills.@__dict__.keys()) {
            if (skil[0::2] != "__") {
                skillList.append("{skil}: {getattr(me.skills, skil)}");
            }
        }
        g.setAllText(1, skillList);
    }
    
    public static void nameDebugCheck(object name) {
        if (name == "Superman") {
            setattr(me.skills, "Dodge", 500);
            setattr(me.skills, "Club", 500);
        }
        if (name == "Debug") {
            me.skills.Club = 100;
            me.skills.Dodge = 100;
            me.strength = 20;
            me.currentHP = 500;
        }
        if (name == "Poison") {
            me.inv.append(it.createItem("Noxious Elixir"));
        }
        if (name == "Hunter") {
            setattr(me.skills, "Dissection", 50);
        }
        if (name == "Richboi") {
            me.money = 10000000;
        }
    }
    
    public static void savePlayer() {
        using (var play = open(@"player.kr", "wb")) {
            p.dump(me, play);
            p.dump(me.skills, play);
        }
    }
    
    public static void loadPlayer() {
        using (var play = open(@"player.kr", "rb")) {
            me = p.load(play);
            me.skills = p.load(play);
        }
    }
    
    public static bool skillCheck(object skill, int mod = 0) {
        var skillLevel = getSkill(skill);
        var randRoll = r.randrange(100);
        if (randRoll <= skillLevel + mod) {
            return true;
        } else {
            var skillGainRoll = r.randrange(100);
            if (skillGainRoll >= skillLevel) {
                setattr(me.skills, skill, skillLevel + 1);
                Console.WriteLine(skill, skillLevel);
            }
            return false;
        }
    }
    
    public static object getSkill(object skill) {
        try {
            return getattr(me.skills, skill);
        } catch {
            setattr(me.skills, skill, 0);
            return getattr(me.skills, skill);
        }
    */
}
