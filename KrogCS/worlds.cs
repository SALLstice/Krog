//using os;
//using p = pickle;
//using r = random;
//using nx = networkx;
//using b = boss;
using it = items;
//using pl = places;
//using t = times;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.IO;
using pe = people;
//using Microsoft.Msagl;
using System.Diagnostics;

public static class worlds
{
    public class World
    {

        public bool BOUNTIFUL = true; //for debugging: sets all regions to harvest every hour
        public bool showTime = false;
        public bool showPersonTime = false;
        public List<Region> regions = new List<Region>();
        public List<City> cities = new List<City>();
        public List<Road> roads = new List<Road>();
        public List<pe.Person> people = new List<pe.Person>();
        public List<Crafter> crafters = new List<Crafter>();
        public List<Business> shops = new List<Business>();
        public List<Harvester> harvesters = new List<Harvester>();
        public List<Business> businesses = new List<Business>();
        public List<Barracks> barracks = new List<Barracks>();
        public List<it.Item> items = new List<it.Item>();
        public List<it.Item> artifacts = new List<it.Item>();
        public List<Job> jobs = new List<Job>();
        public Stopwatch sw = new Stopwatch();
        public void coverWithRegions()
        {
            for (int x = 0; x <= main.worldSize; x++)
            {
                for (int y = 0; y <= main.worldSize; y++)
                {


                }

            }
        }
        public Region addNewRegion()
        {
            Region region = new Region();
            int ws = main.worldSize;
            Random rnd = new Random();

            string[] regionTypes = { "Mountains", "Coast", "Forest", "Grasslands" };
            region.type = regionTypes[rnd.Next(regionTypes.Length)];

            region.name = "The " + randomName() + " " + region.type;

            region.bounty = BOUNTIFUL ? 1 : rnd.NextDouble();

            //TODO: Create regions then populate with cities?
            // Or drop cities randomly then section grid into regions?

            this.regions.Add(region);

            return region;
        }
        public void addNewCity()
        {
            City newCity = buildNewCity();
            newCity.taxRate = 0.15;
            newCity.money = 400;

            newCity.addShop("Weapon Shop");
            newCity.addShop("Blacksmith");
            newCity.addShop("Mine");
            newCity.addShop("Barracks");

            newCity.placeCity(this);
        }
        public City buildNewCity()
        {
            City newCity = new City();
            newCity.name = randomName("city");
            newCity.world = this;
            return newCity;
        }
        public Road addNewRoad(City source, City target, string type = "road")
        {
            Random rnd = new Random();

            string[] lines = File.ReadAllLines("roaddesc.txt");

            var desc1 = lines[rnd.Next(lines.Length)];
            var desc2 = lines[rnd.Next(lines.Length)];
            var desc = $"a {desc1}, {desc2}";

            if (target != null && findRoad(source.name, target.name) is null)
            {
                Road newRoad = new Road(source, target, desc, false, 3, 0, type);
                this.roads.Add(newRoad);
                source.roads.Add(newRoad);
                target.roads.Add(newRoad);
                return newRoad;
            }
            else
            {
                return null;
            }
        }
        public City findCity(string name)
        {
            foreach (City city in this.cities)
            {
                if (name == city.name)
                {
                    return city;
                }
            }
            return null;
        }
        public Road findRoad(string cityname1, string cityname2)
        {
            foreach (Road road in this.roads)
            {
                if (road.source.name == cityname1 || road.source.name == cityname2)
                {
                    if (road.target.name == cityname1 || road.target.name == cityname2)
                    {
                        return road;
                    }
                }
            }
            return null;
        }
        public City randomCity()
        {
            Random rnd = new Random();

            int index = rnd.Next(this.cities.Count);
            return this.cities[index];
        }
        public void passTime(int timePassed, string type)
        {
            int hoursPassed = 0;

            switch (type)
            {
                case "hour":
                    hoursPassed = timePassed;
                    break;
                case "day":
                    hoursPassed = timePassed * 24;
                    break;
                case "week":
                    hoursPassed = timePassed * 168;
                    break;
                case "month":
                    hoursPassed = timePassed * 730;
                    break;
                case "year":
                    hoursPassed = timePassed * 8760;
                    break;
                default:
                    hoursPassed = timePassed;
                    break;
            }

            var csv = new StringWriter();
            var newLine = "";

            foreach (Business business in this.businesses)
            {
                newLine += ","+$"{business.city.name}-{business.GetType()}";
            }
            csv.WriteLine(newLine);

            
            for (int time = 1; time <= hoursPassed; time++)
            {
                newLine = $"{time}";

                //Each Hour
                {
                    foreach (Business business in this.businesses)
                    {
                        business.removeUnusableStock();
                        business.checkAndHireRandomEmployee();
                        business.findSupplyAndBuy();
                        newLine += ","+$"{business.money}";
                    }
                    foreach (City city in this.cities)
                    {

                    }
                    
                    foreach (Crafter crafter in this.crafters)
                    {
                        crafter.checkAndStartAllCraftJobs();
                    }
                    foreach (Harvester harvester in this.harvesters)
                    {
                        harvester.harvest();
                    }
                    foreach (Barracks barracks in this.barracks)
                    {

                        foreach (pe.Person guard in barracks.employees)
                        {
                            //guard.equippedWeapon = barracks.giveItem("Dagger");
                            //FIXME: Add back
                        }
                    }
                    foreach (pe.Person person in this.people)
                    {
                        if (person.equippedWeapon != null && !person.equippedWeapon.usable)
                        {
                            person.inventory.Add(person.equippedWeapon);
                            person.equippedWeapon = null;
                        }

                        if (person.equippedWeapon == null)
                        {
                            person.findAndBuyItem("Dagger");
                        }
                    }
                    foreach (it.Item item in this.items)
                    {

                    }
                    foreach (Craft craftingJob in this.jobs.ToList())
                    {
                        if (craftingJob.timeRemaining > 0)
                        {
                            craftingJob.timeRemaining--;
                        }

                        if (craftingJob.timeRemaining <= 0)
                        {
                            craftingJob.craftComplete();
                        }
                    }
                    
                }
                //Each Day
                if (time % 24 == 0)
                {
                    foreach (City city in this.cities)
                    {

                    }
                    foreach (Business business in this.businesses)
                    {
                        business.payEmployees();
                    }
                    foreach (Crafter crafter in this.crafters)
                    {

                    }
                    foreach (Harvester harvester in this.harvesters)
                    {

                    }
                    foreach (Barracks barracks in this.barracks)
                    {
                        barracks.findSupplyAndBuy();
                    }
                    foreach (pe.Person person in this.people)
                    {

                    }
                    foreach (it.Equipment item in this.items)
                    {
                        item.naturalWear();
                        item.ageItem();
                    }
                    foreach (Job job in this.jobs)
                    {

                    }
                }

                //Each Week
                if (time % 168 == 0)
                {
                    foreach (City city in this.cities)
                    {
                        int BARRACKS_EARNINGS = 200;

                        if (city.money >= BARRACKS_EARNINGS)
                        {
                            city.money -= BARRACKS_EARNINGS;
                            city.barracks.money += BARRACKS_EARNINGS;
                        }
                    }
                    foreach (Business business in this.businesses)
                    {
                        
                    }
                    foreach(ItemShop itemStop in this.shops)
                    {
                        itemStop.payTaxes();
                    }
                    foreach (Crafter crafter in this.crafters)
                    {
                        crafter.payTaxes();
                    }
                    foreach (Harvester harvester in this.harvesters)
                    {
                        harvester.payTaxes();
                    }
                    foreach (Barracks barracks in this.barracks)
                    {
                        
                    }
                    foreach (pe.Person person in this.people)
                    {

                    }
                    foreach (it.Item item in this.items)
                    {

                    }
                    foreach (Job job in this.jobs)
                    {

                    }
                }

                //Each Month
                if (time % 730 == 0)
                {
                    foreach (City city in this.cities)
                    {

                    }
                    foreach (Business business in this.businesses)
                    {

                    }
                    foreach (Crafter crafter in this.crafters)
                    {

                    }
                    foreach (Harvester harvester in this.harvesters)
                    {

                    }
                    foreach (Barracks barracks in this.barracks)
                    {

                    }
                    foreach (pe.Person person in this.people)
                    {
                        person.paytaxes();
                    }
                    foreach (it.Item item in this.items)
                    {

                    }
                    foreach (Job job in this.jobs)
                    {

                    }
                }

                //Each Year
                if (time % 8760 == 0)
                {
                    foreach (City city in this.cities)
                    {

                    }
                    foreach (Business business in this.businesses)
                    {

                    }
                    foreach (Crafter crafter in this.crafters)
                    {

                    }
                    foreach (Harvester harvester in this.harvesters)
                    {

                    }
                    foreach (Barracks barracks in this.barracks)
                    {

                    }
                    foreach (pe.Person person in this.people)
                    {

                    }
                    foreach (it.Item item in this.items)
                    {

                    }
                    foreach (Job job in this.jobs)
                    {

                    }
                }
                csv.WriteLine(newLine);
            }
            File.WriteAllText("Business.csv", csv.ToString());
        }
    }
    public class Region
    {
        public string name;
        public string type;
        public double bounty;
    }

    public class City
    {
        public string name;
        public int[] location;
        public List<Road> roads = new List<Road>();
        public List<pe.Person> residents = new List<pe.Person>();
        public List<Business> businesses = new List<Business>();
        public ItemShop itemShop;
        public Crafter blacksmith;
        public Harvester mine;
        public Barracks barracks;
        public Region region;
        public World world;
        public int money;
        public int GDP;
        public double taxRate;

        public List<City> findAllNeighbors()
        {
            List<City> neighborList = new List<City>();

            foreach (Road road in this.roads)
            {
                if (this == road.source)
                {
                    neighborList.Add(road.target);
                }
                else
                {
                    neighborList.Add(road.source);
                }
            }

            return neighborList;
        }

        public void addShop(string shopType)
        {
            Random rnd = new Random();

            switch (shopType)
            {
                case "Weapon Shop":
                    ItemShop newShop = new ItemShop();

                    newShop.newItemStock("Dagger", 2, 10, 10, true, false, false, false, true);
                    newShop.money = rnd.Next(100, 300);
                    newShop.payRate = 2;
                    newShop.maxWorkers = 2;
                    newShop.upsell = 1; //percentage increase; 1 doubles the cost

                    this.world.businesses.Add(newShop);
                    this.world.shops.Add(newShop);
                    this.itemShop = newShop;
                    this.businesses.Add(newShop);
                    newShop.city = this;
                    break;

                case "Blacksmith":
                    Crafter blacksmith = new Crafter();

                    blacksmith.newItemStock("Dagger", 1, 5, 2, false, true, true, false, false);
                    blacksmith.newResourceStock("Iron Ore", 4, 10, 5, true, false, false, false, false);
                    blacksmith.money = rnd.Next(100, 200);
                    blacksmith.payRate = 2;
                    blacksmith.maxWorkers = 3;
                    blacksmith.upsell = 1;

                    this.world.businesses.Add(blacksmith);
                    this.world.crafters.Add(blacksmith);
                    this.blacksmith = blacksmith;
                    this.businesses.Add(blacksmith);
                    blacksmith.city = this;
                    break;

                case "Mine":
                    Harvester newMine = new Harvester();

                    newMine.newResourceStock("Iron Ore", 0, 50, 10, false, true, false, true, false);
                    newMine.money = rnd.Next(100, 200);
                    newMine.payRate = 0;
                    newMine.maxWorkers = 3;
                    newMine.upsell = 1;

                    this.world.businesses.Add(newMine);
                    this.world.harvesters.Add(newMine);
                    this.mine = newMine;
                    this.businesses.Add(newMine);
                    newMine.city = this;
                    break;

                case "Barracks":
                    Barracks barracks = new Barracks();

                    barracks.newItemStock("Dagger", 3, 6, 3, true, false, false, false, false);
                    barracks.money = rnd.Next(50, 100);
                    barracks.payRate = 3;
                    barracks.maxWorkers = 3;

                    this.world.businesses.Add(barracks);
                    this.world.barracks.Add(barracks);
                    this.barracks = barracks;
                    this.businesses.Add(barracks);
                    barracks.city = this;
                    break;

                default:
                    break;
            }
        }

        public void placeCity(World world)
        {

            int size = main.worldSize;
            Random rnd = new Random();
            bool repeat = false;

            int[] cityLocation = { 0, 0 };
            do
            {
                cityLocation[0] = rnd.Next(size);
                cityLocation[1] = rnd.Next(size);
                repeat = false;

                foreach (City cityToCheck in main.world.cities)
                {
                    if (cityToCheck.location.SequenceEqual(cityLocation)) //TODO: set range of minimu distance with pythagorean
                    {
                        repeat = true;
                    }
                }

            } while (repeat);

            this.region = world.addNewRegion();
            world.cities.Add(this);
            this.location = cityLocation;
        }

    }

    public class Business
    {
        public List<it.Stock> stocks = new List<it.Stock>();
        public City city = new City();
        public int money;
        public List<pe.Person> employees = new List<pe.Person>();
        public int maxWorkers;
        public Random rnd = new Random();
        public int payRate;
        public double upsell;
        public List<Job> jobs = new List<Job>();

        public void findSupplyAndBuy(){

            foreach (var stock in this.stocks)
            {
                if (stock.stocks.Count < stock.minStock)
                {
                    var supplier = stock.supplier;

                    if (supplier == null || supplier.stockCount(stock.item.itemType) <= 0)
                    {
                        supplier = this.findSupplier(stock.item);
                    }

                    if (supplier != null)
                    {
                        this.BuySellItemWithUpsellAdj(this, supplier, stock);
                    }
                }
            }
        }
        
        public Business findSupplier(it.Item item)
        {
            foreach (Business shopAroundTown in this.city.businesses)
            {
                foreach (it.Stock stock in shopAroundTown.stocks)
                {
                    if (stock.item.itemType == item.itemType && stock.willSell && stock.stocks.Count > 0)
                    {
                        return shopAroundTown;
                    }
                }
            }
            

            foreach (City neighbor in this.city.findAllNeighbors())
            {
                foreach (Business neighborshop in neighbor.businesses)
                {
                    foreach (it.Stock stock in neighborshop.stocks)
                    {
                        if (stock.item.itemType == item.itemType && stock.willSell && stock.stocks.Count > 0)
                        {
                            return neighborshop;
                        }
                    }
                }
            }
            return null;
        }

        public void BuySellItemWithUpsellAdj(Business buyer, Business seller, it.Stock buyerStock)
        {
            var sellerStock = seller.findStock(buyerStock.item.itemType);
            var boughtItem = seller.giveItem(sellerStock.item.itemType);

            buyer.money -= boughtItem.cost;
            seller.money += boughtItem.cost;

            double newCost = boughtItem.cost * (1 + buyer.upsell);
            boughtItem.cost = Convert.ToInt32(newCost);
            buyerStock.stocks.Add(boughtItem);
        }

        public void checkAndHireRandomEmployee()
        {
            if (this.employees.Count < this.maxWorkers && this.money >= this.payRate)
            {
                foreach (pe.Person newWorker in this.city.residents)
                {
                    if (newWorker.employer == null)
                    {
                        newWorker.employer = this;
                        this.employees.Add(newWorker);

                        break;
                    }
                }
            }
        }

        public void payEmployees()
        {
            foreach (pe.Person employee in this.employees.ToList())
            {
                if (this.money >= this.payRate)
                {
                    employee.money += this.payRate;
                    this.money -= this.payRate;
                }
                else
                {
                    this.employees.Remove(employee);
                    employee.employer = null;
                }
            }
        }

        public void payTaxes()
        {
            int taxesOwed = Convert.ToInt32(this.money * this.city.taxRate);

            if (this.money >= taxesOwed)
            {
                this.money -= taxesOwed;
                this.city.money += taxesOwed;
            }
        }

        public void newItemStock(string itemType, int min, int max, int start, bool willBuy, bool willSell, bool willCraft, bool willHarvest, bool soldInStore)
        {
            it.Stock newStock = new it.Stock();
            
            newStock.item = it.createItem(itemType);
            newStock.minStock = min;
            newStock.maxStock = max;
            newStock.willBuy = willBuy;
            newStock.willSell = willSell;
            newStock.willCraft = willCraft;
            newStock.willHarvest = willHarvest;
            newStock.soldInStore = soldInStore;
            this.stocks.Add(newStock);

            if (start > 0)
            {
                newStock.startingStock(start);
            }
        }

        public void newResourceStock(string itemType, int min, int max, int start, bool willBuy, bool willSell, bool willCraft, bool willHarvest, bool soldInStore)
        {
            it.Stock newStock = new it.Stock();
            
            newStock.item = it.createResource(itemType);
            newStock.minStock = min;
            newStock.maxStock = max;
            newStock.willBuy = willBuy;
            newStock.willSell = willSell;
            newStock.willCraft = willCraft;
            newStock.willHarvest = willHarvest;
            newStock.soldInStore = soldInStore;
            this.stocks.Add(newStock);

            if (start > 0)
            {
                newStock.startingStock(start);
            }
        }

        public it.Stock findStock(string itemType)
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == itemType)
                {
                    return stock;
                }
            }
            return null;
        }

        public int stockCount(string itemType)
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == itemType)
                {
                    return stock.stocks.Count;
                }
            }
            return 0;
        }


        public void removeUnusableStock()
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.item is it.Equipment)
                {
                    foreach (it.Equipment item in stock.stocks.ToList())
                    {
                        if (!item.usable)
                        {
                            stock.stocks.Remove(item);
                        }
                    }
                }
            }
        }

        public it.Item giveItem(string itemType)
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == itemType && stock.stocks.Count > 0)
                {
                    var item = stock.stocks[0];
                    stock.stocks.Remove(item);
                    return item;
                }
            }
            return null;
        }

        public void addItemToStocks(it.Item item)
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == item.itemType)
                {
                    stock.stocks.Add(item);
                    break;
                }
            }
        }
    }

    public class ItemShop : Business
    {

    }

    public class Crafter : Business
    {
        public void checkAndStartAllCraftJobs()
        {
            foreach (it.Stock stock in this.stocks)
            {
                if (stock.willCraft && stock.stocks.Count < stock.maxStock)
                { 
                    foreach (Tuple<string, int> pair in stock.item.materialsToCraft)
                    {
                        if (this.stockCount(pair.Item1) >= pair.Item2 && this.employees.Count > 0)
                        {
                            foreach (pe.Person employee in this.employees)
                            {
                                if (employee.job == null)
                                {
                                    var newJob = new Craft();

                                    newJob.item = stock.item;
                                    newJob.timeRemaining = stock.item.craftTime;
                                    newJob.employee = employee;
                                    newJob.jobSite = this;

                                    employee.job = newJob;

                                    for (int i = 1; i <= pair.Item2; i++)
                                    {
                                        newJob.craftingMaterials.Add(this.giveItem(pair.Item1));
                                    }

                                    this.jobs.Add(newJob);
                                    this.city.world.jobs.Add(newJob);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class Harvester : Business
    {
        public void harvest()
        {
            //FIXME: don't leave this hard coded to ore
            Random rnd = new Random();

            foreach (it.Stock stock in this.stocks)
            {
                if (stock.stocks.Count <= stock.maxStock && stock.stocks.Count <= stock.minStock && this.employees.Count > 0)
                {
                    //var ha = stock.item.harvestAmount; //FIXME: make resource materials a differ ent type of stock?

                    if (rnd.NextDouble() <= this.city.region.bounty)
                    {
                        for (int i = 1; i <= rnd.Next(3, 10); i++)
                        {  //FIXME: make option by range of resource

                            stock.stocks.Add(it.createResource("Iron Ore"));

                        }
                    }
                }
            }
        }
    }

    public class Barracks : Business
    {

    }

    public class Road
    {
        public string desc;
        public bool known;
        public int length;
        public int roughness;
        //public object travellers; todo: add back in maybe
        public string roadType;
        public City source;
        public City target;

        public Road(
            City source,
            City target,
            string desc,
            bool known,
            int length,
            int roughness,
            string roadType)
        {
            this.source = source;
            this.target = target;
            this.desc = desc;
            this.length = length;
            this.roadType = roadType;
            this.known = known;
            this.roughness = roughness;
        }
    }

    public class Job
    {
        public Business jobSite;
        public pe.Person employee;
        public int timeRemaining;
    }

    public class Craft : Job
    {
        //public List<object> craftMatProgress;
        //public List<object> craftMatsApplied;
        public it.Item item;
        public List<it.Item> craftingMaterials = new List<it.Item>();
        //public object quantity;
        //public object skill;
        //public object status;

        public void craftComplete()
        {
            var newItem = it.createItem(this.item.itemType);

            //newItem.crafter = this.employee;
            //newItem.materialsUsed = this.craftingMaterials;
            //foreach (it.Item craftMat in newItem.materialsUsed)
            //{
            //    newItem.cost += craftMat.cost;
            //}
            //TODO: Maybe increase newtiem cost by upsell?
            //FIXME: Put all this back
            newItem.cost = 10;

            this.jobSite.addItemToStocks(newItem);
            this.jobSite.city.world.items.Add(newItem);

            this.employee.job = null;
            this.jobSite.jobs.Remove(this);
            this.jobSite.city.world.jobs.Remove(this);
        }
    }

    public static string randomName(string type = "")
    {
        var name = "";
        Random rnd = new Random();
        var cnsnnts = new List<string> {
            "B",
            "C",
            "D",
            "F",
            "G",
            "H",
            "J",
            "K",
            "L",
            "M",
            "N",
            "P",
            "QU",
            "R",
            "S",
            "T",
            "V",
            "W",
            "X",
            "Z"
        };
        var oe = new List<string> {
            "A",
            "E",
            "U",
            "I",
            "O",
            "Y"
        };
        if (type == "city" || 1 == 1)
        { //TODO: Make different random name types
            foreach (var i in Enumerable.Range(0, rnd.Next(2, 4)))
            {
                name += cnsnnts[rnd.Next(cnsnnts.Count - 1)] + oe[rnd.Next(oe.Count - 1)];
            }
        }
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

        return ti.ToTitleCase(name.ToLower());
    }

    public static World buildWorld(int worldSize, int numCities, int infestation)
    {
        //TODO: alternate worlds?

        Random rnd = new Random();
        World web = new World();

        for (int i = 0; i < numCities; i++)
        {
            //web.addNewRegion();

            //performs full build and construction of city and places it in the world.
            web.addNewCity();
        }

        int roadCount = 0;
        //TODO: detect crossroads and create trade post there somehow
        foreach (City startcity in web.cities)
        {
            roadCount = 0;
            while (roadCount < 1)
            {
                string closestCityName = "";

                double dist = 999999;

                int x1 = startcity.location[0];
                int y1 = startcity.location[1];

                foreach (City checkcity in web.cities)
                {
                    int x2 = checkcity.location[0];
                    int y2 = checkcity.location[1];
                    double checkdist = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                    if (checkdist < dist && checkdist > 0 && web.findRoad(startcity.name, checkcity.name) == null)
                    {
                        closestCityName = checkcity.name;
                        dist = checkdist;
                    }
                }

                //TODO: 
                //1 pick city "HOME"
                //2 find home's 4 closest neightbors
                //3 check if roads do not exist between them and home
                //4 check that each nieghbor is not beween another nieghbor and home
                //4A draw box around neighbor
                //connect each corner of the box to home
                //if neighbor exists within that drawn figure, do not draw road to far neighbor
                //4B list all neighbors, sort by distance.
                // From closest, check...
                // road does not already exist
                // road is not within the same Math.Atan2 +- 15degrees of another closer city
                // if so, draw raod
                //5 draw roads which meet those requirements

                City targetCity = web.findCity(closestCityName);
                Road newRoad = web.addNewRoad(startcity, targetCity);

                if (newRoad != null)
                {
                    roadCount++;
                }
            }
        }


        var w = new StreamWriter("worldplot.csv");

        foreach (City city in web.cities)
        {
            var line = city.name + "," + city.location[0] + "," + city.location[1] + ",";

            foreach (City cityList in city.findAllNeighbors())
            {
                line += (cityList.name + ",");
            }
            w.WriteLine(line);
            w.Flush();
        }

        //TODO: if cities are grouped together. Make them a kingdom. Otherwise, they are not part of a state

        return web;

    }

    public static void populateWorld()
    {
        Random rnd = new Random();

        double range = 0.25;
        int exp = 3;

        double num = Math.Pow(main.numberOfCities, exp);
        int lownum = Convert.ToInt32(num * (1 - range));
        int hinum = Convert.ToInt32(num * (1 + range));

        int numResidents = rnd.Next(lownum, hinum);

        for (int i = 0; i <= numResidents; i++)
        {
            pe.newPerson();
        }

    }


    /*

        /// FIXME: web = t.createCalendar(web);
        // setattr(web,'mine',[])
        // setattr(web,'lumbermill',[])
        // setattr(web,'blacksmith', [])
        capidx = 0;
        //construct and describe roads
        using (var f = open("roaddesc.txt")) {
            lines = f.read().splitlines();
        }

        //give each node sites and people
        foreach (var x in Enumerable.Range(0, web.nodes.Count)) {
            web.nodes[x]["name"] = randomName("city");
            web.nodes[x]["sites"] = new List<object>();
            web.nodes[x]["label"] = Convert.ToInt32(x);
            web.nodes[x]["population"] = pe.createPerson(2, 20, "rand", location: x, homeLocation: x);
            web.nodes[x]["ruined"] = false;
            if (x == 0) {
                web.nodes[x]["sites"].append(pl.createPlace("Homes"));
                tempsite = pl.createPlace("Weapon Shop");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Short Sword"), 0, 5, true, false, false, true),
                    new stock(it.@ref("Dagger"), 0, 10, true, false, false, true),
                    new stock(it.@ref("Club"), 0, 3, true, false, false, true)
                });
                setattr(tempsite, "money", r.randrange(1000, 3000));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
                tempsite = pl.createPlace("Armor Shop");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Plate Mail"), 0, 2, true, false, false, true)
                });
                setattr(tempsite, "money", r.randrange(1000, 3000));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
                tempsite = pl.createPlace("Inn");
                setattr(tempsite, "stocks", new List<object>());
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", false);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
                tempsite = pl.createPlace("Enchanter");
                setattr(tempsite, "stocks", new List<object>());
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", false);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
            }
            if (Enumerable.Range(1, 5 - 1).Contains(x)) {
                // todo: dynamically create stocks based on what items the shop could create, based on the items craftMats
                // todo: increase prices at tradeposts
                web.nodes[x]["sites"].append(pl.createPlace("Homes"));
                tempsite = pl.createPlace("Blacksmith");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Short Sword"), 0, 4, false, true, true, true),
                    new stock(it.@ref("Plate Mail"), 0, 1, false, true, true, true),
                    new stock(it.@ref("Dagger"), 0, 10, false, true, true, true),
                    new stock(it.@ref("Iron Ore"), 0, 20, true, false, false, false),
                    new stock(it.@ref("Wood"), 0, 20, true, false, false, false)
                });
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
                tempsite = pl.createPlace("Woodworker");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Club"), 0, 4, false, true, true, true),
                    new stock(it.@ref("Wood"), 0, 20, true, false, false, false)
                });
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, x, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[x]["sites"].append(tempsite);
                //todo: food sites generate food supply
            }
            var numKrog = r.randrange(infestation + 3, 2 * (infestation + 4));
            web.nodes[x]["monsters"] = pe.createPerson(pTID: 1, number: numKrog, location: x);
            // todo: add savagery attr to nodes which affect number and strength of krogs
            // todo: randomize krog growth times
        }
        foreach (var x in Enumerable.Range(5, 21 - 5)) {
            var randnode = x * 4 + r.randrange(1, 5);
            color_map[randnode] = "blue";
            if (web.nodes[randnode]["terrain"] == "mountains") {
                tempsite = pl.createPlace("Mining Camp");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Iron Ore"), 0, 500, false, true, true, false)
                });
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, randnode, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[randnode]["sites"].append(tempsite);
            } else if (web.nodes[randnode]["terrain"] == "forest") {
                tempsite = pl.createPlace("Lumbermill");
                setattr(tempsite, "stocks", new List<stock> {
                    new stock(it.@ref("Wood"), 0, 500, false, true, true, false)
                });
                setattr(tempsite, "money", r.randrange(100, 300));
                setattr(tempsite, "employees", employRandom(web, randnode, 3));
                setattr(tempsite, "economic", true);
                setattr(tempsite, "location", x);
                web.nodes[randnode]["sites"].append(tempsite);
            }
        }
        // nx.draw(web, node_color=color_map, with_labels=True)
        // plt.show()
        fillEmptySources(web);
        // create Druid Circle in a random node
        //web.nodes[r.randrange(len(web.nodes))]['sites'].append(pl.createPlace(4, "Druid Circle"))
        createSiteAtRandomLoc(web, 4, "Druid Circle");
        createSiteAtRandomLoc(web, 8, "Hunter Camp");
        createSiteAtRandomLoc(web, 13, "Witch");
        // create krog Hill in a random node
        web.nodes[r.randrange(web.nodes.Count)]["monsters"].append(pl.createPlace(4));
        web.graph["instability"] = 0;
        web.graph["capital"] = capidx;
        //kkloc = 3 #todo: dont do it this way lol
        //kkloc = r.randrange(r.randrange(len(web.nodes)))
        //pe.createBoss()  # todo: re-add but change to normal Person of kingkrog object
        //pe.kingKrog.location = kkloc

        */




    /*
    public static void fillEmptySources(object web) {
        foreach (var y in web.nodes) {
            foreach (var s in web.nodes[y]["sites"]) {
                if (s.stocks != new List<object>()) {
                    foreach (var tocks in s.stocks) {
                        if (tocks.source == null) {
                            tocks.source = findClosestSource(web, y, tocks.item.itemType);
                        }
                    }
                }
            }
        }
    }
    
    public static string findClosestSource(int web, object homeNode, object itType) {
        var tempItem = it.@ref(itType);
        var shortestPath = 99999;
        var genlist = new List<object>();
        object closest = null;
        foreach (var node in web.nodes) {
            foreach (var site in web.nodes[node]["sites"]) {
                if (site.economic) {
                    foreach (var stk in site.stocks) {
                        if (stk.item.itemType == itType && stk.craft && site.location == homeNode) {
                            return "self";
                        } else if (stk.item.itemType == itType && stk.sell && stk.entities.Count > 0) {
                            // if the store's stock matches the item being searched for and that site sells that item, and they have stock...
                            genlist.append(site);
                        }
                    }
                }
            }
        }
        if (tempItem.harvestable || tempItem.craftable) {
            // craftList = [o.location for o in getattr(web,tempItem.craftSite)]
            foreach (var harvSite in genlist) {
                if (nx.shortest_path_length(web, homeNode, harvSite.location) < shortestPath) {
                    closest = harvSite;
                    shortestPath = nx.shortest_path_length(web, homeNode, harvSite.location);
                }
            }
            return closest;
        }
        return null;
    }
    
    public static void runWorld(object hours) {
        object stidx;
        object allowedCost;
        object tempHarvest;
        var TRAVEL_TIME = 4;
        var progBar = "|..................................................|";
        var prog = 0;
        var craftFlag = new List<object>();
        var bonusAttrs = new List<object>();
        foreach (var i in Enumerable.Range(0, hours)) {
            //progBar = '|................................................|'
            //progBar = progBar[0:int(i / (hours / 50))] + "|" + progBar[int(i / (hours / 50)):]
            //print(progBar)
            foreach (var node in world.nodes) {
                foreach (var shop in world.nodes[node]["sites"]) {
                    foreach (var i in shop.stocks) {
                        if (i.entities.Count < i.reqStock) {
                            if (!i.job) {
                                // if the stock doesn't currently have a job
                                if (i.source == "self" && i.item.craftable && i.craft) {
                                    var tempCraft = new craft(activeJobs.Count, null, shop, i.item.craftQuantity, it.createItem(i.item.itemType), "inactive", i.item.craftSkill);
                                    foreach (var craftMatsOfItem in i.item.craftMats) {
                                        tempCraft.craftMatProgress.append(new List<int> {
                                            craftMatsOfItem[0],
                                            0,
                                            craftMatsOfItem[1]
                                        });
                                    }
                                    setWorkerToJob(tempCraft);
                                    i.job = tempCraft;
                                } else if (i.source == "self" && i.item.harvestable) {
                                    tempHarvest = new harvest(0, null, shop, i.item);
                                    setWorkerToJob(tempHarvest);
                                    i.job = tempHarvest;
                                } else if (i.source == null) {
                                    i.source = findClosestSource(world, shop.location, i.item.itemType);
                                } else if (i.source != null && i.buy) {
                                    // if the item has a source and the homeshop buys this item, set up shopping trip
                                    var quantityToBuy = max(1, Convert.ToInt32(i.reqStock * 0.3));
                                    var fullCost = quantityToBuy * i.item.cost;
                                    if (fullCost > shop.money) {
                                        // figure out how much the home shop can buy
                                        allowedCost = Convert.ToInt32(shop.money / i.item.cost) * i.item.cost;
                                    } else {
                                        allowedCost = fullCost;
                                    }
                                    if (allowedCost > 0) {
                                        shop.money -= allowedCost;
                                        // e.busy = True
                                        i.source = findClosestSource(world, shop.location, i.item.itemType);
                                        if (i.source) {
                                            var distance = 4;
                                            var tempJob = new shoppingTrip(activeJobs.Count, null, shop, i.source, distance, allowedCost, new shoppingBag(i.item, 0, quantityToBuy), i.item, false);
                                            setWorkerToJob(tempJob);
                                            i.job = tempJob;
                                            // e.job = tempJob
                                            // e.location = [shop.location, i.source.location]
                                            // i.needState = 'being worked'
                                        }
                                    } else {
                                        break;
                                    }
                                } else {
                                    // if len(i.entities) >= i.reqStock:
                                    // i.needState = 'fully stocked'
                                }
                            }
                        }
                    }
                }
            }
            foreach (var node in world.nodes) {
                foreach (var shop in world.nodes[node]["sites"]) {
                    foreach (var j in shop.stocks) {
                        if (j.job) {
                            if (j.job.status == "need stock") {
                                foreach (var craftProgress in j.job.craftMatProgress) {
                                    if (craftProgress[1] < craftProgress[2]) {
                                        // if the number of materials applies is less than is needed
                                        foreach (var homeStocks in j.job.homeShop.stocks) {
                                            if (homeStocks.item.typeID == craftProgress[0]) {
                                                if (homeStocks.entities.Count > 0) {
                                                    continue;
                                                } else {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (j.job.status == "inactive") {
                                if (j.job.worker == null) {
                                    foreach (var e in j.job.homeShop.employees) {
                                        if (!e.job) {
                                            j.job.worker = e;
                                            j.job.status = "active";
                                            e.job = j;
                                            // j.worker.busy = True
                                            if (type(j) == shoppingTrip) {
                                                e.location = new List<object> {
                                                    j.job.homeShop.location,
                                                    j.job.shop.location
                                                };
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            if (j.job.status == "active") {
                                if (type(j.job) == harvest) {
                                    // todo: have ites go into worker inv then move to store after inv limite reached
                                    stidx = findStockIndex(j.job.homeShop, j.job.item);
                                    foreach (var i in Enumerable.Range(0, j.job.item.craftQuantity)) {
                                        tempHarvest = it.createItem(j.job.item.itemType);
                                        setattr(tempHarvest, "craftMatsSource", j.job.homeShop.location);
                                        j.job.homeShop.stocks[stidx].entities.append(tempHarvest);
                                    }
                                    if (checkStockFull(j.job.homeShop, j.job.item)) {
                                        j.job.status = "complete";
                                    }
                                } else if (type(j.job) == craft) {
                                    foreach (var craftProgress in j.job.craftMatProgress) {
                                        if (craftProgress[1] < craftProgress[2]) {
                                            // if the number of materials applies is less than is needed
                                            foreach (var homeStocks in j.job.homeShop.stocks) {
                                                if (homeStocks.item.typeID == craftProgress[0]) {
                                                    if (homeStocks.entities.Count > 0) {
                                                        var smithSkill = j.job.worker.useSkill(j.job.skill);
                                                        if (smithSkill >= 100) {
                                                            bonusAttrs = new List<object>();
                                                            foreach (var atr in dir(j.job.item)) {
                                                                if (atr[0::5] == "bonus") {
                                                                    bonusAttrs.append(atr);
                                                                }
                                                            }
                                                            if (bonusAttrs.Count > 0) {
                                                                var bonAtr = bonusAttrs[r.randrange(0, bonusAttrs.Count)];
                                                                setattr(j.job.item, bonAtr, getattr(j.job.item, bonAtr) + 1);
                                                            }
                                                        }
                                                        if (smithSkill >= 30) {
                                                            j.job.craftMatsApplied.append(homeStocks.entities.pop(0));
                                                            craftProgress[1] += 1;
                                                            break;
                                                        } else {
                                                            homeStocks.entities.pop(0);
                                                            break;
                                                        }
                                                    } else {
                                                        j.job.status = "need stock";
                                                        try {
                                                            j.job.worker.job = null;
                                                            j.job.worker = null;
                                                        } catch {
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        } else {
                                            continue;
                                        }
                                        break;
                                    }
                                    foreach (var each in j.job.craftMatProgress) {
                                        if (each[1] < each[2]) {
                                            break;
                                        }
                                    }
                                } else if (type(j.job) == shoppingTrip) {
                                    if (j.job.distance > 0) {
                                        j.job.distance -= 1;
                                    } else if (j.job.distance <= 0) {
                                        if (!j.job.returning) {
                                            // buying from the shop
                                            stidx = findStockIndex(j.job.shop, j.job.item);
                                            foreach (var count in Enumerable.Range(0, j.job.wagon.wants)) {
                                                // for the quantity the buyer wants...
                                                if (j.job.money >= j.job.item.cost && j.job.shop.stocks[stidx].entities.Count >= 1) {
                                                    // check if the buyer can afford it and the store has actual stock of the item
                                                    j.job.wagon.entities.append(j.job.shop.stocks[stidx].entities.pop(0));
                                                    j.job.wagon.holding += 1;
                                                    j.job.money -= j.job.item.cost;
                                                    j.job.shop.money += j.job.item.cost;
                                                } else {
                                                    break;
                                                }
                                            }
                                            j.job.returning = true;
                                            j.job.distance = 4;
                                            j.job.worker.location = new List<object> {
                                                j.job.shop.location,
                                                j.job.homeShop.location
                                            };
                                        } else {
                                            // returning with purchased goods
                                            stidx = findStockIndex(j.job.homeShop, j.job.item);
                                            foreach (var wagonItem in j.job.wagon.entities) {
                                                j.job.homeShop.stocks[stidx].entities.append(wagonItem);
                                            }
                                            // if len(j.homeShop.stocks[stidx].entities) >= j.homeShop.stocks[stidx].reqStock:
                                            // j.homeShop.stocks[stidx].needState = 'fully stocked'
                                            // else:
                                            // j.homeShop.stocks[stidx].needState = 'under stocked'
                                            j.job.homeShop.money += j.job.money;
                                            j.job.status = "complete";
                                            j.job.worker.location = j.job.homeShop.location;
                                        }
                                    }
                                }
                            }
                            if (j.job.status == "complete") {
                                j.job.worker.job = null;
                                // activeJobs.remove(j)
                                stidx = findStockIndex(j.job.homeShop, j.job.item);
                                j.job.homeShop.stocks[stidx].job = null;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public static object setWorkerToJob(object job) {
        object mostTalented = null;
        var highestTalent = 0;
        object talent = null;
        if (type(job) == craft) {
            if (hasattr(job, "skill")) {
                talent = getattr(job, "skill");
            }
        } else if (type(job) == shoppingTrip) {
            talent = "speed";
        } else if (type(job) == harvest) {
            talent = job.item.craftSkill;
        }
        if (!job.worker) {
            if (job.homeShop.employees.Count > 0) {
                foreach (var e in job.homeShop.employees) {
                    if (hasattr(e, talent)) {
                        if (getattr(e, talent) > highestTalent && !e.job) {
                            mostTalented = e;
                            highestTalent = getattr(e, talent);
                        }
                    }
                }
                if (!mostTalented) {
                    mostTalented = job.homeShop.employees[r.randrange(job.homeShop.employees.Count)];
                }
                job.worker = mostTalented;
                job.status = "active";
                mostTalented.job = job;
                if (type(job) == shoppingTrip) {
                    try {
                        mostTalented.location = new List<object> {
                            job.homeShop.location,
                            job.shop.location
                        };
                    } catch (AttributeError) {
                        Console.WriteLine("none error");
                    }
                }
            } else {
                Console.WriteLine("hiring");
            }
        }
    }
    
    public static bool checkStockFull(object store, object item) {
        foreach (var storeStocks in store.stocks) {
            if (storeStocks.item.typeID == item.typeID) {
                if (storeStocks.entities.Count >= storeStocks.reqStock) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
    
    public static void findStockIndex(object store, object item) {
        foreach (var _tup_1 in store.stocks.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            var stockidx = _tup_1.Item1;
            var storeStocks = _tup_1.Item2;
            if (storeStocks.item.typeID == item.typeID) {
                return stockidx;
            }
        }
    }
    
    public static void createSiteAtRandomLoc(object web, int sTID, string name) {
        web.nodes[r.randrange(web.nodes.Count)]["sites"].append(pl.createPlace(sTID, name));
    }
    
    public static void worldInfo() {
        Console.WriteLine("The capital city is " + world.graph["capital"].ToString());
    }
    
    public static void saveWorld() {
        nx.write_gpickle(world, @"world/world.kr");
        // nx.write_gml(world, r'world/world.kr')
        using (var pit = open(@"world/items.kr", "wb")) {
            p.dump(it.items, pit);
        }
        pit.close();
        using (var ppl = open(@"world/places.kr", "wb")) {
            p.dump(pl.places, ppl);
        }
        ppl.close();
        using (var ppe = open(@"world/Persons.kr", "wb")) {
            p.dump(pe.Persons, ppe);
        }
        ppe.close();
        using (var phi = open(@"world/history.kr", "wb")) {
            p.dump(t.history, phi);
        }
        phi.close();
        using (var obi = open(@"world/obituary.kr", "wb")) {
            p.dump(pe.futureDead, obi);
        }
        obi.close();
        t.printHistory();
    }
    
    public static void loadWorld() {
        //todo: time is not loaded
        world = nx.read_gpickle(@"world/world.kr");
        //world = nx.read_gml(r'world/world.kr') dont use this
        world = nx.convert_node_labels_to_integers(world);
        world = nx.read_gpickle(@"world/world.kr");
        using (var pit = open(@"world/items.kr", "rb")) {
            it.items = p.load(pit);
        }
        using (var ppl = open(@"world/places.kr", "rb")) {
            pl.places = p.load(ppl);
        }
        using (var ppe = open(@"world/Persons.kr", "rb")) {
            pe.Persons = p.load(ppe);
        }
        using (var phi = open(@"world/history.kr", "rb")) {
            t.history = p.load(phi);
        }
        using (var obi = open(@"world/obituary.kr", "rb")) {
            pe.futureDead = p.load(obi);
        }
        b.findBoss();
    }
    
    public static void openInitialWorld() {
        // todo: I think dead monster inventory loot resets
        world = nx.read_gpickle(@"world/worldStart.kr");
        //world = nx.read_gml('world/worldStart.kr')
        world = nx.convert_node_labels_to_integers(world);
        world = nx.read_gpickle(@"world/worldStart.kr");
        using (var pit = open(@"world/itemsStart.kr", "rb")) {
            it.items = p.load(pit);
        }
        pit.close();
        using (var ppl = open(@"world/placesStart.kr", "rb")) {
            pl.places = p.load(ppl);
        }
        ppl.close();
        using (var ppe = open(@"world/PersonsStart.kr", "rb")) {
            pe.Persons = p.load(ppe);
        }
        ppe.close();
        using (var phi = open(@"world/history.kr", "rb")) {
            t.history = p.load(phi);
        }
        phi.close();
        using (var obi = open(@"world/obituary.kr", "rb")) {
            pe.futureDead = p.load(obi);
        }
        obi.close();
        b.findBoss();
        // todo: new characters don't keep known info about locs and roads?
        // todo: can find journal of dead characters to learn all stuff they knew
    }
    
    public static void saveWorldState() {
        if (!os.path.exists("world")) {
            os.makedirs("world");
        }
        nx.write_gpickle(world, @"world/worldStart.kr");
        //nx.write_gml(world, 'world/worldStart.kr')  # saves the world state for future characters
        using (var pit = open(@"world/itemsStart.kr", "wb")) {
            p.dump(it.items, pit);
        }
        pit.close();
        using (var ppl = open(@"world/placesStart.kr", "wb")) {
            p.dump(pl.places, ppl);
        }
        ppl.close();
        using (var ppe = open(@"world/PersonsStart.kr", "wb")) {
            p.dump(pe.Persons, ppe);
        }
        ppe.close();
    }
    */



    /*
    public static object employRandom(object web, int loc, int num) {
        var templist = new List<object>();
        foreach (var pers in web.nodes[loc]["population"]) {
            if (pers.employed == null) {
                templist.append(pers);
                pers.employed = true;
                if (templist.Count >= num) {
                    break;
                }
            }
        }
        return templist;
    }
    */
}
