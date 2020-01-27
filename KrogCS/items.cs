using System;
using System.IO;
using System.Collections.Generic;

public class items {
    public class Item
    {
        public string typeName;
        public string use;
        public int cost;
    }

    public class Stock 
    {    
        //public object buy;   
        //public object craft;
        public List<Item> stocks = new List<Item>();
        public Item item; //FIXME change to object
        //public int inStock; //fixme use entity list to count stock
        //public object job;
        public int reqStock;
        //public object sell;
        //public object source;
        //public object storeStock;
        
    }

    public static Item createItem(string itemType)
    {
        
        Item newItem = new Item();

        var r = new StreamReader("itemList.csv");
        var line = r.ReadLine();

        while(line != null)
        {
            var splitline = line.Split(",");

            if (splitline[1] == itemType)
            {
                newItem.typeName = splitline[1];
                newItem.use = splitline[2];
                newItem.cost = Convert.ToInt32(splitline[3]);

                break;
            }
            line = r.ReadLine();
        } 
        return newItem;
    }
}