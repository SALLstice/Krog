
using time;

using System;

using System.Collections.Generic;

using System.Linq;

public static class life {
    
    public class store {
        
        public object employees;
        
        public object money;
        
        public object stocks;
        
        public store(List<stock> stocks, List<worker> employees, int money) {
            this.stocks = stocks;
            this.money = money;
            this.employees = employees;
        }
    }
    
    public class item {
        
        public object cost;
        
        public object craftable;
        
        public object craftMats;
        
        public object craftQuantity;
        
        public object craftTime;
        
        public object harvestable;
        
        public Func<object> type;
        
        public item(
            string type,
            int cost,
            bool craftable,
            bool harvestable,
            List<List<object>> craftMats,
            int craftTime,
            int craftQuantity) {
            this.type = type;
            this.cost = cost;
            this.craftable = craftable;
            this.harvestable = harvestable;
            this.craftMats = craftMats;
            this.craftTime = craftTime;
            this.craftQuantity = craftQuantity;
        }
    }
    
    public class stock {
        
        public object inStock;
        
        public item item;
        
        public object need;
        
        public object reqStock;
        
        public stock(item item, int inStock, int reqStock, int need = 0) {
            this.item = item;
            this.inStock = inStock;
            this.reqStock = reqStock;
            this.need = need;
        }
    }
    
    public class worker {
        
        public bool busy;
        
        public void job;
        
        public object name;
        
        public int wallet;
        
        public worker(string name) {
            this.name = name;
            this.busy = false;
            this.job = null;
            this.wallet = 0;
        }
    }
    
    public class job {
        
        public object craft;
        
        public object quantity;
        
        public int status;
        
        public store store;
        
        public int timeRemaining;
        
        public worker worker;
        
        public job(worker worker, store store, item craft, int quantity) {
            this.worker = worker;
            this.store = store;
            this.craft = craft;
            this.status = 0;
            this.timeRemaining = -1;
            this.quantity = quantity;
        }
    }
    
    public class shoppingTrip {
        
        public object home;
        
        public item item;
        
        public object returning;
        
        public object shop;
        
        public int status;
        
        public int timeRemaining;
        
        public object wagon;
        
        public object wallet;
        
        public worker worker;
        
        public shoppingTrip(
            worker worker,
            store home,
            store shop,
            int wallet,
            object wagon,
            item item,
            bool returning) {
            this.worker = worker;
            this.home = home;
            this.shop = shop;
            this.wallet = wallet;
            this.wagon = wagon;
            this.item = item;
            this.timeRemaining = 0;
            this.returning = returning;
            this.status = 0;
        }
    }
    
    public static List<object> activeJobs = new List<object>();
    
    public static int TRAVEL_TIME = 4;
    
    public static worker tom = new worker("tom");
    
    public static worker alex = new worker("alex");
    
    public static worker emily = new worker("emily");
    
    public static worker jonas = new worker("jonas");
    
    public static item ironOre = new item("ore", 2, false, true, null, 8, 5);
    
    public static item wood = new item("wood", 1, false, false, null, 6, 4);
    
    public static item sword = new item("sword", 10, true, false, new List<List<object>> {
        new List<object> {
            ironOre,
            3
        },
        new List<object> {
            wood,
            2
        }
    }, 6, 1);
    
    public static item mace = new item("mace", 15, true, false, new List<List<object>> {
        new List<object> {
            ironOre,
            5
        }
    }, 12, 1);
    
    public static store blacksmith = new store(new List<stock> {
        new stock(sword, 5, 6),
        new stock(mace, 2, 6),
        new stock(ironOre, 61, 60),
        new stock(wood, 61, 60)
    }, new List<worker> {
        tom,
        alex,
        emily
    }, 2000);
    
    public static store lumbermill = new store(new List<stock> {
        new stock(wood, 101, 100)
    }, new List<worker> {
        jonas
    }, 1000);
    
    static life() {
        i.need = 1;
        e.busy = true;
        activeJobs.append(new job(e, blacksmith, i.item, i.item.craftQuantity));
        i.need = 2;
        blacksmith.money = totalCost;
        activeJobs.append(new shoppingTrip(e, blacksmith, lumbermill, totalCost, new List<object>(), i.item, false));
        i.need = 2;
        i.need = 0;
        j.timeRemaining = 1;
        j.store.stocks[stockidx].inStock = j.quantity;
        j.status = 2;
        activeJobs.append(new job(j.worker, j.store, j.craft, j.quantity));
        j.shop.stocks[stockidx].inStock = Convert.ToInt32(j.wallet / stockitem.cost);
        j.shop.money = j.wallet;
        activeJobs.append(new shoppingTrip(j.worker, blacksmith, lumbermill, 0, new stock(j.item, Convert.ToInt32(j.wallet / j.item.cost), 0), j.item, true));
        j.wallet = 0;
        j.status = 2;
        j.home.stocks[stockidx].inStock = j.wagon.inStock;
        j.status = 2;
        j.status = 1;
        j.timeRemaining = j.craft.craftTime;
        j.timeRemaining = TRAVEL_TIME;
        j.store.stocks[stockidx].inStock = j.craft.craftMats[craftidx][1];
        activeJobs.remove(j);
        j.worker.busy = false;
        input();
    }
    
    public static int quantityToBuy = Convert.ToInt32(i.reqStock + i.reqStock * 0.1 - i.inStock);
    
    public static int totalCost = quantityToBuy * i.item.cost;
}
