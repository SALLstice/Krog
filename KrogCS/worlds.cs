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

public static class worlds
{
    public class World
    {
        public List<Region> regions = new List<Region>();
        public List<City> cities = new List<City>();
        public List<Road> roads = new List<Road>();
        public List<pe.Person> people = new List<pe.Person>();
        public List<Crafter> crafters = new List<Crafter>();
        public List<Business> shops = new List<Business>();
        public List<Harvester> harvesters = new List<Harvester>();
        public List<Business> businesses = new List<Business>();
        public List<it.Item> items = new List<it.Item>();
        public List<it.Item> artifacts = new List<it.Item>();

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

            region.bounty = rnd.NextDouble();

            //TODO: Create regions then populate with cities?
            // Or drop cities randomly then section grid into regions?

            this.regions.Add(region);

            return region;
        }

        public void addNewCity()
        {
            City newCity = buildNewCity();
            newCity.placeCity(this);

            newCity.addShop("Weapon Shop");
            newCity.addShop("Blacksmith");
            newCity.addShop("Mine");
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

            if (findRoad(source.name, target.name) is null)  //FIXME: sometimes this bugs out
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

        public void passTime(int hoursPassed)
        {

            for (int time = 1; time <= hoursPassed; time++)
            {
                foreach(Business business in this.businesses)
                {
                    business.checkAndHireRandomEmployee();

                    if(hoursPassed % 7 == 0){
                        business.payEmployees();
                    }
                }

                foreach(ItemShop itemShop in this.shops)
                {
                    itemShop.randomSales();
                }

                foreach (Crafter crafter in this.crafters)
                {
                    foreach (it.Stock stock in crafter.stocks)
                    {
                        if (stock.willCraft && stock.stocks.Count <= stock.maxStock)
                        {
                            crafter.craftAll();
                        }
                    }
                    
                    crafter.tradeAwayItem();
                }

                foreach(var harvester in this.harvesters)
                {
                    harvester.harvest();
                    harvester.tradeAwayItem();
                }
                
                foreach(var item in this.items)
                {
                    item.naturalWear();
                    item.ageItem();
                }

                foreach(City city in this.cities)
                {
                    city.totalMoney = 0;
                    foreach(Business shop in city.businesses)
                    {
                        city.totalMoney += shop.money;
                    }
                }
            }

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
        public Region region;
        public World world;
        public int totalMoney;

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

                    newShop.newStock("Dagger", 5, 10, true, false, false, false);
                    newShop.money = rnd.Next(50,100);
                    newShop.payRate = 3;
                    newShop.maxWorkers = 2;

                    this.world.businesses.Add(newShop);
                    this.world.shops.Add(newShop);
                    this.businesses.Add(newShop);
                    newShop.city = this;
                    break;

                case "Blacksmith":
                    Crafter blacksmith = new Crafter();

                    blacksmith.newStock("Dagger", 3, 5, false, true, true,false);
                    blacksmith.newStock("Iron Ore", 20,50, true,false,false,false);
                    blacksmith.money = rnd.Next(50,100);
                    blacksmith.payRate = 2;
                    blacksmith.maxWorkers = 3;

                    this.world.businesses.Add(blacksmith);
                    this.world.crafters.Add(blacksmith);
                    this.businesses.Add(blacksmith);
                    blacksmith.city = this;
                    break;

                case "Mine":
                    Harvester newMine = new Harvester();

                    newMine.newStock("Iron Ore", 5, 50,false,true,false,true);
                    newMine.money = rnd.Next(50,100);
                    newMine.payRate = 1;
                    newMine.maxWorkers = 10;

                    this.world.businesses.Add(newMine);
                    this.world.harvesters.Add(newMine);
                    this.businesses.Add(newMine);
                    newMine.city = this;
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

        public void checkAndHireRandomEmployee()
        {
            if(this.employees.Count < this.maxWorkers && this.money >= this.payRate)
            {                
                foreach(pe.Person newWorker in this.city.residents){
                    if (newWorker.jobSite == null)
                    {
                        newWorker.jobSite = this;
                        this.employees.Add(newWorker);

                        break;
                    }
                }
            }
        }

        public void payEmployees()
        {
            foreach(pe.Person employee in this.employees.ToList())
            {
                if(this.money >= this.payRate)
                {
                    employee.money += this.payRate;
                    this.money -= this.payRate;
                }
                else
                {
                    this.employees.Remove(employee);
                    employee.jobSite = null;
                }
            }
        }

        public void newStock(string itemType, int min, int max, bool willBuy, bool willSell, bool willCraft, bool willHarvest)
        {
            it.Stock newStock = new it.Stock();
            newStock.item = it.createItem(itemType);
            newStock.minStock = min;
            newStock.maxStock = max;
            newStock.willBuy = willBuy;
            newStock.willSell = willSell;
            newStock.willCraft = willCraft;
            newStock.willHarvest = willHarvest;
            this.stocks.Add(newStock);
        }

        public void tradeAwayItem()
        {
            foreach (Business shopAroundTown in this.city.businesses)
            {
                checkAndSellToNeighbors(shopAroundTown);
            }

            foreach (City neighbor in this.city.findAllNeighbors())
            {
                foreach (Business neighborshop in neighbor.businesses)
                {
                    checkAndSellToNeighbors(neighborshop);
                }
            }
        }

        public void checkAndSellToNeighbors(Business neighborshop)
        {
            foreach (it.Stock neighborstock in neighborshop.stocks)
            {
                if (neighborstock.stocks.Count < neighborstock.maxStock && neighborshop.money >= neighborstock.item.cost)
                {
                    foreach (it.Stock homestock in this.stocks)
                    {
                        if (homestock.item.itemType == neighborstock.item.itemType && homestock.willSell == true)
                        {
                            if (homestock.stocks.Count >= homestock.minStock)
                            {
                                for (int i = 1; i <= 1; i++)
                                { //FIXME: make it diff from nieghbor stock. not 1
                                    it.Item item = homestock.stocks[0];
                                    homestock.stocks.RemoveAt(0);
                                    neighborstock.stocks.Add(item);

                                    neighborshop.money -= homestock.item.cost;
                                    this.money += homestock.item.cost;
                                }
                            }
                        }
                    }
                }
            }
        }

        public int stockCount(string itemType)
        {
            foreach(it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == itemType){
                    return stock.stocks.Count;
                }
            }
            return 0;
        }

        public void destoryStock(string itemType)
        {
            foreach(it.Stock stock in this.stocks)
            {
                if (stock.item.itemType == itemType){
                    stock.stocks.RemoveAt(0);
                }
            }
        }
    
        public void removeUnusableStock(){
            foreach(it.Stock stock in this.stocks)
            {
                foreach(it.Item item in stock.stocks)
                {
                    if(!item.usable)
                    {
                        stock.stocks.Remove(item);
                    }
                }
            }
        }
    }

    public class ItemShop : Business
    {
        public double RANDOM_SALES = 0.6;

        public void randomSales()
        {
            foreach(it.Stock stock in this.stocks)
            {
                if(stock.willSell && this.rnd.NextDouble() <= RANDOM_SALES && this.employees.Count > 0)
                {
                    stock.stocks.RemoveAt(0);
                    this.money += stock.item.cost;
                }
            }
        }
    }

    public class Crafter : Business
    {
        public void craftAll()
        {
            foreach(it.Stock stock in this.stocks)
            {
                if(stock.willCraft && stock.stocks.Count < stock.maxStock)
                {
                    foreach(Tuple<string,int> pair in stock.item.toCraft)
                    {
                        if (this.stockCount(pair.Item1) >= pair.Item2 && this.employees.Count > 0) 
                        {
                            for (int i = 1; i <= pair.Item2; i++)
                            {
                                this.destoryStock(pair.Item1);
                            }
                            stock.stocks.Add(it.createItem(stock.item.itemType));
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
                    if (rnd.NextDouble() <= this.city.region.bounty){
                        stock.stocks.Add(it.createResource("Iron Ore"));
                    }
                }
            }
        }
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

    public class shoppingBag
    {
        public List<object> entities;
        public object holding;
        public object item;
        public object wants;

        public shoppingBag(object item,
            int holding,
            int wants)
        {
            this.item = item;
            this.holding = holding;
            this.wants = wants;
            this.entities = new List<object>();
        }
    }

    public class Job
    {

    }

    public class craft : Job
    {
        public List<object> craftMatProgress;
        public List<object> craftMatsApplied;
        public object homeShop;
        public object item;
        public object jobID;
        public object quantity;
        public object skill;
        public object status;
        public object worker;

        public craft(
            int jobID,
            object worker,
            object homeShop,
            object quantity,
            object item,
            string status,
            object skill)
        {
            this.jobID = jobID;
            this.worker = worker;
            this.homeShop = homeShop;
            this.status = status;
            // self.timeRemaining = -1
            this.quantity = quantity;
            this.item = item;
            this.craftMatProgress = new List<object>();
            this.craftMatsApplied = new List<object>();
            this.skill = skill;
        }
    }

    public class shoppingTrip : Job
    {
        public object distance;
        public object homeShop;
        public object item;
        public object jobID;
        public object money;
        public object returning;
        public object shop;
        public object status;
        public object wagon;
        public object worker;

        public shoppingTrip(
            int jobID,
            object worker,
            object homeShop,
            object shop,
            int distance,
            object money,
            shoppingBag wagon,
            object item,
            bool returning,
            string status = "inactive")
        {
            this.jobID = jobID;
            this.worker = worker;
            this.homeShop = homeShop;
            this.shop = shop;
            this.distance = distance;
            this.money = money;
            this.wagon = wagon;
            this.item = item;
            // self.timeRemaining = 0
            this.returning = returning;
            this.status = status;
        }
    }

    public class harvest : Job
    {
        public object homeShop;
        public object item;
        public object jobID;
        public string status;
        public object worker;

        public harvest(int jobID,
            object worker,
            object homeShop,
            object item)
        {
            this.jobID = jobID;
            this.worker = worker;
            this.homeShop = homeShop;
            this.item = item;
            this.status = "inactive";
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

        Console.WriteLine("Done Build World");
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
