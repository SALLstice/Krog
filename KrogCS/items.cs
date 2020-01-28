using System;
using System.IO;
using System.Collections.Generic;
using Deedle;
using System.Diagnostics;

public class items {
    
    public static Frame<string, string> itemList = Frame.ReadCsv("itemList.csv").IndexRows<string>("itemType");
    public static double WEAR_RATE = 0.1;
    public static int ARTIFACT_AGE = 1000;
    public static double RANDOM_USE = 0.3;
    public static Random rnd = new Random();

    public class Item
    {
        public string itemType;
        public string use;
        public int cost;
        public List<Tuple<string,int>> materialsToCraft = new List<Tuple<string,int>>();
        public List<Item> materialsUsed = new List<Item>();
        public int wear;
        public bool wears;
        public bool usable = true;
        public int age = 0;
        public bool artifact = false;
        public bool canBeArtifact;
        public people.Person crafter;
        public int craftTime;
        public people.Person wielder;

        public void naturalWear()
        {
            if (this.wears){
                var randnum = items.rnd.NextDouble();

                if (randnum <= items.WEAR_RATE && !this.artifact)
                {
                    this.wear++;
                    this.checkWear();
                }
            }
        }

        public void checkWear()
        {
            if (this.wear >= 100)
            {
                this.usable = false;
            }
        }

        public void ageItem()
        {
            this.age++;

            if(this.age >= ARTIFACT_AGE && this.usable && this.canBeArtifact && this.artifact == false)
            {
                this.artifact = true;
                main.world.artifacts.Add(this);
            }
        }

        public void simulateRandomUse()
        {
            if (this.wears){
                var randnum = items.rnd.NextDouble();

                if (randnum <= items.RANDOM_USE && !this.artifact)
                {
                    this.wear++;
                    this.checkWear();
                }
            }
        }
    }

    public class Resource : Item
    {
        public int[] harvestRange = {0,0};
    }

    public class Stock 
    {    
        public bool willBuy;   
        public bool willCraft;
        public List<Item> stocks = new List<Item>();
        public Item item; //FIXME change to object
        //public object job;
        public int minStock;
        public int maxStock;
        public bool willSell;
        public bool willHarvest;
        public bool soldInStore;
        //public object source;
        //public object storeStock;
        
    }

    public static Item createItem(string itemType)
    {   //FIXME: Creating an item takes too long
        Stopwatch sw = new Stopwatch();
        
        sw.Start();
        var newItem = new Item();

        newItem.itemType = itemType;
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        newItem.cost = Convert.ToInt32(itemList["cost", itemType]);
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        newItem.use = Convert.ToString(itemList["use", itemType]);
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        newItem.wears = Convert.ToBoolean(itemList["wears", itemType]);
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        newItem.canBeArtifact = Convert.ToBoolean(itemList["canBeArtifact", itemType]);
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        
        if(itemList["craftTime", itemType] != ""){
            newItem.craftTime = Convert.ToInt32(itemList["craftTime", itemType]);
        }

        Console.WriteLine("1: " + sw.ElapsedMilliseconds);

        String raw = Convert.ToString(itemList["recipe",itemType]);
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        
        if (raw != ""){
            var splitraw = raw.Split("|");
            foreach(var pair in splitraw)
            {   
                var newsplit = pair.Split("-");
                Tuple<string,int> toAdd = new Tuple<string,int>(newsplit[0],Convert.ToInt32(newsplit[1]));
                newItem.materialsToCraft.Add(toAdd);    
            }
        }
        Console.WriteLine("1: " + sw.ElapsedMilliseconds);
        sw.Stop();
        sw.Reset();

        //main.world.items.Add(newItem);
        return newItem;
    }

    public static Item createResource(string itemType)
    {
        
        var newItem = new Resource();

        newItem.itemType = itemType;
        newItem.cost = Convert.ToInt32(itemList["cost", itemType]);
        newItem.use = Convert.ToString(itemList["use", itemType]);
        newItem.wears = Convert.ToBoolean(itemList["wears", itemType]);
        newItem.canBeArtifact = Convert.ToBoolean(itemList["canBeArtifact", itemType]);
        string raw = Convert.ToString(itemList["harvestRange", itemType]);
        string[] rawsplit = raw.Split(":");
        newItem.harvestRange[0] = Convert.ToInt32(rawsplit[0]);
        newItem.harvestRange[1] = Convert.ToInt32(rawsplit[1]);

        return newItem;
    }

    
}