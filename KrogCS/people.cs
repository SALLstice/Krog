
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

public class people {
     
    public class Person 
    {           
        public string name;      
        public w.City location;    
        public int money;
        public w.Business employer;
        public w.Job job;
        public it.Item equippedWeapon;
        public it.Item equippedArmor;
        public List<it.Item> inventory = new List<it.Item>();
        public IDictionary<string,int> skills = new Dictionary<string, int>();
        Random rnd = new Random();
        
        
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
        public bool skillCheck(string skill, int DC, int mod = 0) 
        {
            if(!this.skills.ContainsKey(skill))
            {
                this.skills.Add(skill, 0);
            }
            
            var roll = rnd.Next(1,101);
            var result = roll + this.skills[skill];

           if (result + mod >= DC)
           {
               return true;
           }
           else
           {
               roll = rnd.Next(1,101);
               if (roll > this.skills[skill])
               {
                   this.skills[skill]++;
               }
               if(this is Player)
               {
                   Console.WriteLine($"Your {skill} skill increased to {this.skills[skill]}!");
               }
               return false;
           }
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
    
    public static Player newPlayer(string name = "Satchmo")
    {
        Player newb = new Player();
        newb.name = name;
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
}