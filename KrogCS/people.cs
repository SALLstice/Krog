
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
    public class Being
    {
        public int HP;
        public int maxHP; 
        public int age = 0;
        public IDictionary<string,int> skills = new Dictionary<string, int>();
        public w.City city;
        public it.Equipment equippedWeapon;
        public void skillKnown(string skill){
            if(!this.skills.ContainsKey(skill))
            {
                this.skills.Add(skill, 0);
                if(this is Player)
                {
                    Console.WriteLine($"You begin learning the {skill} skill!");
                }
            }
        }
        public int skillRoll(string skill, int mod = 0) 
        {
            skillKnown(skill);
            
            var roll = rnd.Next(1,1001);
            var result = roll + this.skills[skill] + mod;

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
        public void checkForSkillIncrease(string skill)
        {
            skillKnown(skill);

            int result = rnd.Next(1,1001);

            if (result > this.skills[skill])
            {
                this.skills[skill] += rnd.Next(1,11);
            
                if(this is Player)
                {
                    Console.WriteLine($"Your {skill} skill increased to {this.skills[skill]}!");
                }
            }
        }
        public bool attack(dynamic defender)
        {
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
                return true;
            }
            else
            {
                this.checkForSkillIncrease(attackWeaponType);
                return false;
            }
        }
    }
    public class Person : Being
    {           
        public string name;     
            
        public int money;
        public w.Business employer;
        public w.Job job;
        
        public it.Item equippedArmor;
        public List<it.Item> inventory = new List<it.Item>();
        public bool hospitalized = false;
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
            if(this.employer == null)
            {
                //TODO: find job based on skills
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

                this.money -= boughtItem.cost;
                supplier.money += boughtItem.cost;
            }
        }
        public w.Business findItemSupplier(string itemType)
        {
            foreach(it.Stock stock in this.city.itemShop.stocks)
            {
                if(stock.item.itemType == itemType && stock.soldInStore && stock.stocks.Count > 0)
                {
                    return this.city.itemShop;
                }
            }
            
            foreach (w.City neighbor in this.city.findAllNeighbors())
            {
                foreach(it.Stock stock in neighbor.itemShop.stocks)
                {
                    if(stock.item.itemType == itemType && stock.soldInStore && stock.stocks.Count > 0)
                    {
                        return neighbor.itemShop;
                    }
                }
            }
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
            this.employer = null;
            this.city.hospital.patients.Add(this);
            this.hospitalized = true;
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
        
        public int spawnRate;
        public int hunger;
        //public int attackSkill;
        public int attackDamage;
        public int generation;
        public int kills = 0;
        public void spawn()
        {
            Monster spawn = newMonster(this.city); 

            double EVO_CHANCE = 0.1;
                        
            spawn.maxHP = rnd.NextDouble() <= EVO_CHANCE ? this.maxHP+1 : this.maxHP;
            
            foreach(KeyValuePair<string,int> skill in skills)
            {
                //spawn.attackSkill = rnd.NextDouble() <= EVO_CHANCE ? this.attackSkill+rnd.Next(1,4) : this.attackSkill;
            }
            spawn.generation = this.generation + 1;

            this.spawnRate = rnd.Next(80,121);
        }
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

                if(guard.attack(this)) 
                {
                    this.hurt(2);  
                }
            }

            if(rnd.Next(this.maxHP) < this.HP)
            {
                if (targetCity.barracks.employees.Count > 0)
                {
                    var guardAttacked = targetCity.barracks.employees[rnd.Next(targetCity.barracks.employees.Count)];

                    if(this.attack(guardAttacked)) 
                    {
                        guardAttacked.HP -= this.attackDamage;
                    
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
                }
                else
                {
                    if (targetCity.residents.Count > 0)
                    {
                        var personAttacked = targetCity.residents[rnd.Next(targetCity.residents.Count)];

                        if(this.attack(personAttacked)) 
                        {
                            personAttacked.HP -= this.attackDamage;
                        
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
        }
        public void heal()
        {
            if(this.HP < this.maxHP && rnd.Next(1,11) <= 3) //TODO:change number to mosnter stat
            {
                this.HP++;
            }
        }
        public void hurt(int damage)
        {
            this.HP -= damage;

            if(this.HP <= 0)
            {
                this.dies();
            }
        }
        public bool starve()
        {
            //return true if starving
            //return false if not starving

            this.hunger -= rnd.Next(1,3);

            if(this.hunger <= 0)
            {
                this.hurt(2);
                return true;
            }
            return false;

        }
        public void dies()
        {
            this.city.world.history.deadMonsters.Add(this);
            this.city.monsters.Remove(this);
            this.city.world.monsters.Remove(this);
        }
        public void madeAKill()
        {
            this.hunger = 1400;
            this.kills++;
        }
    }
    public static Player newPlayer(string name)
    {
        Player newb = new Player();
        newb.name = name;
        w.City newLocation = main.world.randomCity(); 

        //newb.placeInWorld(newLocation);

        return newb;
    }
    public static Person newPerson()
    {
        Person newb = new Person();

        newb.name = w.randomName("city");
        newb.money = 5000;
        newb.maxHP = 10;
        newb.HP = newb.maxHP;
        newb.placeInWorld(main.world.randomCity());

        return newb;
    }

    public static Monster newMonster(w.City loc)
    {
        //TODO: Make child literal copy of parent by stats, with slight modifications. Cause evolution.
        Monster spawn = new Monster();
        spawn.spawnRate = rnd.Next(1,3);
        spawn.hunger = 200;
        spawn.maxHP = 20;
        spawn.HP = spawn.maxHP;
        //spawn.attackSkill = 200;
        spawn.attackDamage = 1;
        spawn.generation = 0;

        spawn.placeInWorld(loc);

        return spawn;
    }

    
}