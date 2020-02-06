using System;
using System.IO;
using System.Collections.Generic;
using Deedle;
using System.Diagnostics;
using w = worlds;

public class items {
    
    public static Frame<string, string> itemList = Frame.ReadCsv("itemList.csv").IndexRows<string>("itemType");
    public static double WEAR_RATE = 0.2;
    public static int ARTIFACT_AGE = 3000;
    public static double RANDOM_USE = 0.3;
    public static Random rnd = new Random();

    public class Item
    {
        public string itemType;
        public string use;
        public int cost;
        public List<Tuple<string,int>> materialsToCraft = new List<Tuple<string,int>>();
        public List<Item> materialsUsed = new List<Item>();
        public int craftTime;
        //public List<Tuple<string,int>> materialsToCraft = new List<Tuple<string,int>>();
        //public List<Item> materialsUsed = new List<Item>();
        //public int wear;
        //public bool wears;
        //public bool usable = true;
        //public int age = 0;
        //public bool artifact = false;
        //public bool canBeArtifact;
        //public people.Person crafter;
        //public int craftTime;
        //public people.Person wielder;
    }

    public class Equipment : Item
    {
        
        public int wear;
        public bool usable = true;
        public int age = 0;
        public bool artifact = false;
        public people.Person crafter;
        
        public people.Person wielder;

        public void naturalWear()
        {
            
                var randnum = items.rnd.NextDouble();

                if (randnum <= items.WEAR_RATE && !this.artifact)
                {
                    this.wear += 100; //FIXME:
                    this.checkWear();
                }
            
        }

        public void checkWear()
        {
            if (this.wear >= 100)
            {
                this.usable = false;
                main.world.items.Remove(this);
                main.world.history.unusableItems.Add(this);
            }
        }

        public void ageItem()
        {
            this.age++;

            if(this.age >= ARTIFACT_AGE && this.usable && this.artifact == false)
            {
                this.artifact = true;
                main.world.artifacts.Add(this);
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
        public List<dynamic> stocks = new List<dynamic>();
        public Item item; 
        //public object job;
        public int minStock;
        public int maxStock;
        public bool willSell;
        public bool willHarvest;
        public bool soldInStore;
        public w.Business supplier;
        //public object source;
        //public object storeStock;

        public void startingStock(int num)
        {
            if(this.item is items.Equipment)
            {
                for(int i = 1; i <= num; i++){
                    Item newItem = createItem(this.item.itemType);
                    this.stocks.Add(newItem);
                }
            }
            else if(this.item is items.Resource)
            {
                for(int i = 1; i <= num; i++){
                    Resource newResource = createResource(this.item.itemType);
                    this.stocks.Add(newResource);
                }
            }
        }
    }

    public static Item createItem(string itemType)
    {   
        var series = itemList.GetRow<string>(itemType);
        var use = Convert.ToString(series["use"]);
        
        switch(use){
            case "weapon":
            case "armor":
                Equipment newEquipment = createEquipment(itemType);
                return newEquipment;
            case "resource":
                Resource newResource = createResource(itemType);
                return newResource;
            default:
                return null;
        }
        //var newItem = new Item();

        
    }

    public static Equipment createEquipment(string itemType)
    {
        var series = itemList.GetRow<string>(itemType);
        Equipment newEquipment = new Equipment();

        newEquipment.itemType = itemType;

        newEquipment.cost = Convert.ToInt32(series["cost"]);
        newEquipment.use = Convert.ToString(series["use"]);
        
        if(series["craftTime"] != ""){
            
            var seriesRead = series["craftTime"];
            var craftTime = 1;
            if (seriesRead != "True")
            {
                craftTime = Convert.ToInt32(seriesRead);
            }

            newEquipment.craftTime = craftTime;
        }

        string raw = Convert.ToString(series["recipe"]);
        
        if (raw != ""){
            var splitraw = raw.Split("|");
            foreach(var pair in splitraw)
            {   
                var newsplit = pair.Split("-");
                Tuple<string,int> toAdd = new Tuple<string,int>(newsplit[0],Convert.ToInt32(newsplit[1]));
                newEquipment.materialsToCraft.Add(toAdd);    
            }
        }

        return newEquipment;
    }

    public static Resource createResource(string itemType)
    {
        
        var newItem = new Resource();

        newItem.itemType = itemType;
        var series = itemList.GetRow<string>(itemType);
        
        newItem.cost = Convert.ToInt32(series["cost"]);
        newItem.use = Convert.ToString(series["use"]);

        string raw = Convert.ToString(series["harvestRange"]);
        string[] rawsplit = raw.Split(":");
        newItem.harvestRange[0] = Convert.ToInt32(rawsplit[0]);
        newItem.harvestRange[1] = Convert.ToInt32(rawsplit[1]);

        return newItem;
    }

    
}
