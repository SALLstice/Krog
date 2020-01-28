using System;
using System.IO;
using System.Collections.Generic;
using Deedle;

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
        public List<Tuple<string,int>> toCraft = new List<Tuple<string,int>>();
        public int wear;
        public bool wears;
        public bool usable = true;
        public int age = 0;
        public bool artifact = false;
        public bool canBeArtifact;

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
    {
        
        var newItem = new Item();

        newItem.itemType = itemType;
        newItem.cost = Convert.ToInt32(itemList["cost", itemType]);
        newItem.use = Convert.ToString(itemList["use", itemType]);
        newItem.wears = Convert.ToBoolean(itemList["wears", itemType]);
        newItem.canBeArtifact = Convert.ToBoolean(itemList["canBeArtifact", itemType]);


        String raw = Convert.ToString(itemList["recipe",itemType]);
        
        if (raw != ""){
            var splitraw = raw.Split("|");
            foreach(var pair in splitraw)
            {   
                var newsplit = pair.Split("-");
                Tuple<string,int> toAdd = new Tuple<string,int>(newsplit[0],Convert.ToInt32(newsplit[1]));
                newItem.toCraft.Add(toAdd);    
            }
        }

        main.world.items.Add(newItem);
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

        return newItem;
    }

    
}