
//using csv;
//using p = pickle;
//using r = random;
//using render = django.shortcuts.render;
//using g = gui;
using it = items;
using w = worlds;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Diagnostics;

public class people {
    public static Random rnd = new Random();
    public class Skill
    {
        public string name;
        public int value;
        public string businessType;
        public string stat;
    }
    public class Being
    {
        public int HP;
        public int maxHP; 
        public int age = 0;
        public int hunger = 1000;
        public List<Skill> skills = new List<Skill>();
        public w.City city;
        public int kills = 0;
        public int strength;
        public it.Equipment equippedWeapon;
        public it.Equipment equippedArmor;
        public int damageReduction = 0;
        public List<it.Item> inventory = new List<it.Item>();
        public void learnSkill(string skillName){
            
            if(!(this.skills.Any(i => i.name == skillName)))
            {
                Skill newSkill = new Skill();
                newSkill.name = skillName;
                newSkill.value = 1;

                this.skills.Add(newSkill);

                if(this is Player)
                {
                    Console.WriteLine($"You begin learning the {skillName} skill!");
                }
            }
        }
        public bool skillKnown(string skillName)
        {
            if(this.skills.Any(i => i.name == skillName))
            {
                return true;
            }
            return false;
        }
        
        public int skillValue(string skillName)
        {
           return this.skills.Find(i => i.name == skillName).value;
        }

        public int skillRoll(string skillName, int mod = 0) 
        {
            learnSkill(skillName);
            
            var roll = rnd.Next(1,1001);
            int skillval = skillValue(skillName);

            var result = roll + skillval + mod;

            return result;
        }
        
        public bool skillCheck(string skill, int DC, int mod=0)
        {
            int result = skillRoll(skill, mod);

            if (result >= DC)
            {
                return true;
            }
            else
            {
                checkForSkillIncrease(skill);
                return false;
            }
        }
        public void checkForSkillIncrease(string skillName)
        {
            learnSkill(skillName);

            int result = rnd.Next(1,1001);
            var skill = this.skills.Find(i => i.name == skillName);

            if (result > skill.value)
            {
                int skillIncrease = rnd.Next(1,11);
                skill.value += skillIncrease;
            
                if(this is Player)
                {
                    Console.WriteLine($"Your {skillName} skill increased to {skill.value}!");
                }
                else if(this is Monster)
                {
                    //Monster baby = (Monster)this;
                    //Skill parentSkill = baby.parent.skills.Find(i => i.name == skillName);
                    //parentSkill.value += skillIncrease;
                }

            }
        }
        public void equipItem(it.Equipment item){
            if(item.use == "weapon"){
                if(this.equippedWeapon != null){
                    this.inventory.Add(this.equippedWeapon);
                }
                this.equippedWeapon = item;
            }
            else if(item.use == "armor"){
                if(this.equippedArmor != null){
                    this.inventory.Add(this.equippedArmor);
                    this.damageReduction -= this.equippedArmor.baseEffectValue;
                }
                this.equippedArmor = item;
                this.damageReduction += item.baseEffectValue;
            }
        }
        public void attack(dynamic defender)
        {
            if(this.attackCheck(defender)) 
            {
                int totalDamage = 0;
                
                int baseAttackDamage = 2;  //FIXME set to attacker damage
                totalDamage += baseAttackDamage;
                if(this is Player || defender is Player){
                Console.WriteLine($"Base Attack Damage: {baseAttackDamage}");}

                int damageStrengthBonus = rnd.Next((int)this.strength/2, this.strength);
                totalDamage += damageStrengthBonus;
                if(this is Player || defender is Player){
                Console.WriteLine($"Damage Strength Bonus: {damageStrengthBonus}");}

                if(defender.equippedArmor != null){
                    int diff = totalDamage-defender.equippedArmor.baseEffectValue;
                    int damageReduction = diff > 0 ? diff : 0;
                    totalDamage = damageReduction;
                if(this is Player || defender is Player){
                    Console.WriteLine($"Damage Armor Reduction: {damageReduction}");}
                }
                if(this is Player || defender is Player){
                Console.WriteLine($"Total Damage: {totalDamage}");}
                defender.hurt(totalDamage);  

                if (defender.HP <= 0){
                    if(this.equippedWeapon != null){
                        this.equippedWeapon.kills++;
                        if(!this.equippedWeapon.artifact){
                            int artifactChance = Math.Max(0,this.equippedWeapon.kills - 100);
                            if(rnd.Next(100) < artifactChance){
                                this.equippedWeapon.artifact = true;
                                if(this is Player){
                                    Console.WriteLine($"Your {this.equippedWeapon.itemType} has become an artifact!");
                                }
                            }
                        }
                    }
                    this.kills++;
                }

                if (rnd.Next(1,101) == 1){ 
                    defender.maxHP += rnd.Next(5,8);
                    if(defender is Player){
                        Console.WriteLine($"Your max HP increased to {defender.maxHP}!");
                    }
                }
            }
        }
        public bool attackCheck(dynamic defender){
            string attackWeaponType;

            if(this.equippedWeapon != null)
            {
                attackWeaponType = this.equippedWeapon.itemType;
            }
            else
            {
                attackWeaponType = "Unarmed";
            }
            
            int attackRoll = this.skillRoll(attackWeaponType);
            int dodgeRoll = defender.skillRoll("Dodge");

            if(attackRoll > dodgeRoll)
            {
                defender.checkForSkillIncrease("Dodge");
                if (rnd.Next(1,101) <= 1){ 
                    this.strength++;
                    if(this is Player){
                        Console.WriteLine($"Your Strength increased to {this.strength}!");
                    }
                }
                return true;
            }
            else
            {
                this.checkForSkillIncrease(attackWeaponType);
                return false;
            }
        }
        public void heal(int healAmount)
        {
            if(this.HP + healAmount <= this.maxHP) //TODO:change number to mosnter stat
            {
                this.HP += healAmount;
            }
            else{
                this.HP = this.maxHP;
            }
        }
        public bool starve()
        {
            //return true if starving
            //return false if not starving

            this.hunger -= rnd.Next(1,3);

            if(this.hunger <= 0)
            {
                this.hurt(1);
                return true;
            }
            return false;

        }
        public void hurt(int damage)
        {
            this.HP -= damage;

            if(this.HP <= 0)
            {
                this.dies();
            }
        }
        public void dies()
        {
            if (this is Monster){
                this.city.world.history.deadMonsters.Add((Monster)this);
                this.city.monsters.Remove((Monster)this);
                this.city.world.monsters.Remove((Monster)this);
            }
            if (this is Person){
            
                this.city.world.history.deadPeople.Add((Person)this);
                this.city.residents.Remove((Person)this);
                this.city.world.people.Remove((Person)this);

                
            }
        }
    }
    public class Person : Being
    {           
        public string name;     
        public int money = 1000; //FIXME:
        public w.Business employer;
        public w.Job job;
        
        public List<it.Item> inventory = new List<it.Item>();
        public bool hospitalized = false;
        public bool underTreatment = false;
        Random rnd = new Random();       
        public void placeInWorld(w.City loc)
        {
            main.world.people.Add(this); //FIXME: Dont point to main world?
            loc.residents.Add(this);
            this.city = loc;
        }
        public void moveLocation(w.City loc)
        {
            this.city.residents.Remove(this);
            this.city = loc;
        }
        public void findEmployment()
        {
            foreach(w.Business business in this.city.businesses)
            {
                if(business.employees.Count < business.maxWorkers)
                {
                    foreach(string skillNeeded in business.skillsNeeded)
                    {
                        if(this.skillKnown(skillNeeded))
                        {
                            this.employer = business;
                            business.employees.Add(this);

                            break;
                        }
                    }
                }
            }
        }
        public void removeFromJob()
        {
            if(this.employer != null){
                this.employer.employees.Remove(this);
                this.employer = null;
            }
        }
        public void paytaxes()
        {
            int taxesOwed = Convert.ToInt32(this.money * this.city.taxRate);

            if (this.money >= taxesOwed){
                this.money -= taxesOwed;
                this.city.money += taxesOwed;
            }
        }
        public void findAndBuyItem(string itemType)
        {       
            var supplier = this.findItemSupplier(itemType);
            
            if (supplier != null){
                var sellerStock = supplier.findStock(itemType);
                var boughtItem = supplier.giveItem(itemType);

                if(this.money >= boughtItem.cost){
                    this.money -= boughtItem.cost;
                    supplier.money += boughtItem.cost;
                    this.inventory.Add(boughtItem);
                }
            }
        }
        public bool UseItemFromInventory(string itemType){
            foreach(it.Item item in this.inventory){
                if (item.itemType == itemType){
                    useItem(item);
                    return true;
                }
            }
            return false;
        }
        public void useItem(it.Item item){
            switch(item.use){
                case "food":
                    this.hunger += 500; //FIXME: dont hardcode
                    break;
                default:
                    break;
            }
        }
        public w.Business findItemSupplier(string itemType)
        {
            foreach(dynamic business in this.city.businesses)
            {
                if (business is w.ItemShop)
                {
                    foreach(it.Stock stock in business.stocks)
                    {
                        if(stock.item.itemType == itemType && stock.soldInStore && stock.stocks.Count > 0)
                        {
                            return business;
                        }
                    }
                }
            }
            /* FIXME
            foreach (w.City neighbor in this.city.findAllNeighbors())
            {
                foreach(dynamic business in neighbor.businesses)
                {                
                    if(business is w.ItemShop){    
                        foreach(it.Stock stock in business.stocks)
                        {
                            if(stock.item.itemType == itemType && stock.soldInStore && stock.stocks.Count > 0)
                            {
                                return neighbor.itemShop;
                            }
                        }
                    }
                }
            }*/
            return null;
        }
        public void dies()
        {
            this.city.residents.Remove(this);
            this.city.world.people.Remove(this);
            this.city.world.history.deadPeople.Add(this);

            if(this.employer != null)
            {
                this.employer.employees.Remove(this);
                //FIXME: replace someone to do active job?
            }
        }
        public void checkIntoHospital()
        {
            //this.employer = null;
            //this.city.hospital.patients.Add(this);
            //this.hospitalized = true;
        }
    }
    
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
        
    }
    public class Monster : Being
    {
        public string monsterType;
        public Spawner parent;
        
        //public int attackSkill;
        //public int attackDamage;
        public int generation;
        
        
        public void placeInWorld(w.City loc)
        {
            main.world.monsters.Add(this); //FIXME: Dont point to main world?
            loc.monsters.Add(this);
            this.city = loc;
        }
        public void attackCity()
        {
            var targetCity = this.city;

            foreach(Person guard in targetCity.barracks.employees)
            {
                if (guard.equippedWeapon != null)
                {
                    guard.equippedWeapon.wear++;
                }

                guard.attack(this);
                
            }

            if(rnd.Next(this.maxHP) < this.HP)
            {
                if (targetCity.barracks.employees.Count > 0)
                {
                    var guardAttacked = targetCity.barracks.employees[rnd.Next(targetCity.barracks.employees.Count)];

                    this.attack(guardAttacked);
                    
                    if(guardAttacked.HP <= 0)
                    {
                        guardAttacked.dies();
                        this.madeAKill();
                    }
                    else
                    {
                        this.attackCity();
                    }
                    
                }
                else
                {
                    if (targetCity.residents.Count > 0)
                    {
                        var personAttacked = targetCity.residents[rnd.Next(targetCity.residents.Count)];

                        this.attack(personAttacked);
                        
                            
                        if(personAttacked.HP <= 0)
                        {
                            personAttacked.dies();
                            this.madeAKill();
                        }
                        else
                        {
                            this.attackCity();
                        }
                        
                    }
                }
            }
        }        
        
        public void madeAKill()
        {
            this.hunger = 1400;
            //this.kills++;
        }
    }
    public class Spawner : Monster
    {
        public int spawnRate;
        public int kids = 0;
        public void spawn()
        {               
            double EVO_CHANCE = 0.5;
            int SPAWN_RATE_MAX = 7; //Max days per spawn
            
            if (rnd.NextDouble() <= EVO_CHANCE)
            {
                this.generation = this.generation + 1;

                this.maxHP += 10;
                this.HP += 10;

                Skill selectedskill = GetRandomWeightedSkill(this.skills);
                this.skills.Find(j => j.name == selectedskill.name).value += rnd.Next(2,5);
                
                this.strength++;
            }
        
            Monster spawn = new Monster();
            this.spawnRate = rnd.Next(SPAWN_RATE_MAX);
            this.kids++;

            spawn.hunger = 200;
            spawn.maxHP = (int)this.maxHP / 10;
            spawn.HP = spawn.maxHP;
            spawn.strength = (int)this.strength / 10;
            
            //spawn.attackDamage = 1;
            spawn.generation = this.generation;
            spawn.parent = this;

            foreach(Skill skill in this.skills){
                Skill newskill = new Skill();

                newskill.name = skill.name;
                newskill.value = Math.Max(1, skill.value / 10);
                
                spawn.skills.Add(newskill);
            }

            spawn.placeInWorld(this.city);
        }
    }
    public static Skill GetRandomWeightedSkill(List<Skill> skills)
    {
        int totalWeight = 0;

        foreach (Skill skill in skills)
        {
            totalWeight += skill.value;
        }

        Random _rnd = new Random();
        int randomNumber = _rnd.Next(0, totalWeight);

        Skill selectedskill = null;
        foreach (Skill skill in skills)
        {
            if (randomNumber < skill.value)
            {
                selectedskill = skill;
                break;
            }
            randomNumber = randomNumber - skill.value;
        }
        return selectedskill;
    }
    
    public static Player newPlayer(string name)
    {
        Player newb = new Player();
        newb.name = name;
        w.City newLocation = main.world.randomCity(); 

        newb.city = newLocation;
        it.Equipment startingWeapon = it.createEquipment("Short Sword");
        it.Equipment startingArmor = it.createEquipment("Plate Mail");
        newb.equipItem(startingWeapon);
        newb.equipItem(startingArmor);

        newb.maxHP = 100;
        newb.HP = newb.maxHP;

        newb.strength = 14;

        //newb.placeInWorld(newLocation);

        return newb;
    }
    public static Person newPerson()
    {
        Person newb = new Person();

        newb.name = w.randomName("city");
        newb.money = 50;
        newb.maxHP = 20;
        newb.strength = 3;
        newb.HP = newb.maxHP;
        newb.placeInWorld(main.world.randomCity());

        return newb;
    }

    //public static Monster newMonster(w.City loc)
    //{
    //    Monster spawn = new Monster();
    //    //spawn.spawnRate = rnd.Next(1,3);
    //    spawn.hunger = 200;
    //    spawn.maxHP = 20;
    //    spawn.strength = 2;
    //    spawn.HP = spawn.maxHP;
    //    //spawn.attackSkill = 200;
    //    spawn.attackDamage = 1;
    //    spawn.generation = 0;
    //   spawn.placeInWorld(loc);
    //    return spawn;
    //}

    public static Spawner newSpawnMonster(w.City loc)
    {
        
        Spawner spawner = new Spawner();
        spawner.spawnRate = rnd.Next(1,3);
        spawner.hunger = 200;
        spawner.maxHP = 200;
        spawner.strength = 20;
        spawner.HP = spawner.maxHP;
        //spawn.attackSkill = 200;
        //spawner.attackDamage = 1;
        spawner.generation = 0;

        spawner.learnSkill("Unarmed");
        spawner.learnSkill("Dodge");
        //TODO: spawner.skillKnown("Armor");

        spawner.placeInWorld(loc);

        return spawner;
    }
}
