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
    public static Random rnd = new Random();
    public class World
    {
        public bool BOUNTIFUL = false; //for debugging: sets all regions to harvest every hour
        public bool showTime = false;
        public bool showPersonTime = false;
        public bool monstersSpawed = false;
        public List<Region> regions = new List<Region>();
        public List<City> cities = new List<City>();
        public List<Road> roads = new List<Road>();
        public List<pe.Person> people = new List<pe.Person>();
        public List<pe.Monster> monsters = new List<pe.Monster>();
        public List<Crafter> crafters = new List<Crafter>();
        public List<Business> shops = new List<Business>();
        public List<Harvester> harvesters = new List<Harvester>();
        public List<Business> businesses = new List<Business>();
        public List<Barracks> barracks = new List<Barracks>();
        public List<Hospital> hospitals = new List<Hospital>();
        public List<it.Item> items = new List<it.Item>();
        public List<it.Item> artifacts = new List<it.Item>();
        public History history = new History();
        public List<Craft> craftingJobs = new List<Craft>();
        public List<Treatment> treatmentJobs = new List<Treatment>();
        public IDictionary<string,int> datetime = new Dictionary<string,int>();
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
            newCity.addShop("Hospital");
            newCity.addShop("Bakery");

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
                double distance = findDistanceBetweenCities(source,target);

                int x1 = source.location[0];
                int y1 = source.location[1];
                int x2 = target.location[0];
                int y2 = target.location[1];

                double angle = Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI;
                double angle2 = Math.Atan2(y1 - y2, x1 - x2) * 180 / Math.PI;

                Road newRoad = new Road(source, target, desc, false, distance, angle, 0, type);
                Road newRoad2 = new Road(target, source, desc, false, distance, angle2, 0, type);
                this.roads.Add(newRoad);
                source.roads.Add(newRoad);
                target.roads.Add(newRoad2);
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
        public void runClock()
        {
            this.datetime["Hour"]++;

            if(this.datetime["Hour"] == 24)
            {
                this.datetime["Hour"] = 0;
                this.datetime["Day"]++;
            
                if(this.datetime["Day"] == 30)
                {
                    this.datetime["Day"] = 0;
                    this.datetime["Month"]++;

                    if(this.datetime["Month"] == 12)
                    {
                        this.datetime["Month"] = 0;
                        this.datetime["Year"]++;
                    }
                }
            }
        }
        public string nowString()
        {
            string now = "";

            string[] months = {"First", "Second","Third","Fourth","Fifth","Sixth","Seventh",
            "Eighth","Ninth","Tenth","Eleventh","Twelveth"};
            
            int month = this.datetime["Month"];
            string monthName = months[month];
            string day = Convert.ToString(this.datetime["Day"]);
            string year = Convert.ToString(this.datetime["Year"]);
            string hour = Convert.ToString(this.datetime["Hour"]);

            now = $"{hour}:00 of day {day} of {monthName} month in the year {year}.";

            return now;
        }

        public int nowInt(){
            int yearHours = (this.datetime["Year"] - 100) * 8760;
            int monthHours = (this.datetime["Month"] - 1) * 730;
            int dayHours = (this.datetime["Day"] - 1) * 24;
            int hours = (this.datetime["Hour"] - 6);

            int totalHours = yearHours + monthHours + dayHours + hours;

            return totalHours;
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

            var popcsv = new StringWriter();
            var popnewLine = "";
            var buscsv = new StringWriter();
            var busnewLine = "";

            
            
            for (int time = 1; time <= hoursPassed; time++)
            {
                popnewLine = $"{nowInt()}";
                busnewLine = $"{nowInt()}";

                if(this.people.Count <= 0)
                {
                    Console.WriteLine("The world is dead.");
                    Console.WriteLine(this.nowString());
                    
                    popcsv.WriteLine(popnewLine);
                    File.AppendAllText("Population.csv", popcsv.ToString());
                    buscsv.WriteLine(busnewLine);
                    File.AppendAllText("Business.csv", buscsv.ToString());

                    System.Environment.Exit(1);
                }
                else if (this.monsters.Count <= 0 && this.monstersSpawed)
                {
                    Console.WriteLine("The world is saved.");
                    Console.WriteLine(this.nowString());
                    
                    popcsv.WriteLine(popnewLine);
                    File.AppendAllText("Population.csv", popcsv.ToString());
                    buscsv.WriteLine(busnewLine);
                    File.AppendAllText("Business.csv", buscsv.ToString());

                    System.Environment.Exit(1);
                }
                //Each Hour
                {
                    runClock();

                    foreach (Business business in this.businesses)
                    {
                        business.removeUnusableStock();
                        business.checkAndHireRandomEmployee();
                        business.findSupplyAndBuy();
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
                        //FIXME: This shouldn't all be hardcoded to daggers

                        foreach (pe.Person guard in barracks.employees)
                        {
                            if (guard.equippedWeapon == null){
                                guard.equippedWeapon = (it.Equipment) barracks.giveItem("Dagger");
                            }

                            //Simulate Training
                            guard.checkForSkillIncrease("Dagger");
                        }
                    }
                    foreach (pe.Person person in this.people.ToList())
                    {
                        person.starve();
                        if(person.hunger <= 500){
                            if(!person.UseItemFromInventory("Bread")){
                                person.findAndBuyItem("Bread"); 
                            }
                        }

                        if (!person.hospitalized){
                            if (person.equippedWeapon != null && !person.equippedWeapon.usable)
                            {
                                person.inventory.Add(person.equippedWeapon);
                                person.equippedWeapon = null;
                            }

                            if (person.equippedWeapon == null)
                            {
                                person.findAndBuyItem("Dagger");
                            }
                            if(person.HP < person.maxHP)
                            {
                                //person.checkIntoHospital();  FIXME: put back in
                                person.removeFromJob();
                                person.city.hospital.patients.Add(person);
                                person.hospitalized = true;
                            }
                            if(person.job == null)
                            {
                                person.findEmployment();
                            }
                        }
                    }
                    
                    foreach (Craft craftingJob in this.craftingJobs.ToList())
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
                    foreach(dynamic monster in this.monsters.ToList())
                    {
                        if(!(monster is pe.Spawner)){
                            if(!monster.starve())
                            {
                                if (rnd.NextDouble() <= 0.3){   //TODO: change to monster stat
                                    monster.heal(1);
                                }
                            }
                        
                            if (rnd.Next(1,50) >= monster.hunger)
                            {
                                monster.attackCity();
                            }
                        }
                    }
                }
                //Each Day
                if (time % 24 == 0)
                {
                    foreach (City city in this.cities)
                    {
                        popnewLine += ","+$"{city.residents.Count}";
                        popnewLine += ","+$"{city.monsters.Count}";

                        for(int i = 1; i <= 20; i++){
                            city.bakery.spawnItemInStock("Bread");
                        }
                    }
                    foreach (Business business in this.businesses)
                    {
                        business.payEmployees();

                        busnewLine += ","+$"{business.money}";
                    }
                    

                    foreach (Barracks barracks in this.barracks)
                    {
                        barracks.findSupplyAndBuy();
                    }
                    foreach (Hospital hospital in this.hospitals)
                    {
                        foreach(pe.Person doctor in hospital.employees){
                            if(doctor.job == null)
                            {
                                foreach(pe.Person patient in hospital.patients)
                                {
                                    if(!patient.underTreatment)
                                    {
                                        Treatment treatment = new Treatment();
                                        treatment.employee = doctor;
                                        treatment.patient = patient;
                                        doctor.job = treatment;
                                        treatment.jobSite = hospital;
                                        treatment.timeRemaining = 0; //FIXME: maybe?
                                        patient.underTreatment = true;
                                        this.treatmentJobs.Add(treatment);
                                        break;
                                    }
                                }
                            }
                        }

                        
                    }
                    foreach (pe.Person person in this.people)
                    {
                        person.age++;
                    }
                    foreach (it.Equipment item in this.items.ToList())
                    {
                        item.naturalWear();
                        item.ageItem();
                    }
                    
                    foreach(dynamic monster in this.monsters.ToList())
                    {
                        monster.age++;
                        
                        if (monster is pe.Spawner)
                        {
                            pe.Spawner spawner = (pe.Spawner) monster;
                            spawner.spawnRate--;

                            if (spawner.spawnRate <= 0 && spawner.hunger > 100)
                            {
                                spawner.spawn();
                            }
                        }
                    }

                    foreach(Treatment treatment in this.treatmentJobs.ToList())
                    {
                        if(treatment.patient.HP < treatment.patient.maxHP){
                            int damageSustained = treatment.patient.maxHP - treatment.patient.HP;
                            if(treatment.employee.skillCheck("Medical", damageSustained * 100)){
                                treatment.patient.heal(1);
                            }                            
                        }
                        else
                        {
                            treatment.patient.hospitalized = false;
                            treatment.patient.underTreatment = false;
                            Hospital treatmentCenter = (Hospital)treatment.jobSite;
                            treatmentCenter.patients.Remove(treatment.patient);
                            this.treatmentJobs.Remove(treatment);
                        }                        
                    }
                }

                //Each Week
                if (time % 168 == 0)
                {
                    foreach (City city in this.cities)
                    {
                        int BARRACKS_EARNINGS = 1000;

                        if (city.money >= BARRACKS_EARNINGS)
                        {
                            city.money -= BARRACKS_EARNINGS;
                            city.barracks.money += BARRACKS_EARNINGS;
                        }

                        int HOSPITAL_EARNINGS = 1000;

                        if (city.money >= HOSPITAL_EARNINGS)
                        {
                            city.money -= HOSPITAL_EARNINGS;
                            city.hospital.money += HOSPITAL_EARNINGS;
                        }

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
                        //barracks.payTaxes();
                        //FIXME: maybe?
                    }
                    
                }

                //Each Month
                if (time % 730 == 0)
                {
                    
                    foreach (pe.Person person in this.people)
                    {
                        person.paytaxes();
                    }
                    
                }

                //Each Year
                if (time % 8760 == 0)
                {
                   
                    Console.WriteLine($"Year {time / 8760} Complete.");
                }
            
                popcsv.WriteLine(popnewLine);
                buscsv.WriteLine(busnewLine);
            }
            try{
                File.AppendAllText("Population.csv", popcsv.ToString());
                File.AppendAllText("Business.csv", buscsv.ToString());

                var peepcsv = new StringWriter();
                var peepnewLine = "";
                foreach(pe.Person person in this.people)
                {
                    peepnewLine += $"{person.name},";
                    peepnewLine += $"{person.HP}--{person.maxHP},";
                    peepnewLine += $"{person.kills},";
                    peepnewLine += $"{person.money},";
                    
                    foreach(pe.Skill skill in person.skills)
                    {
                        peepnewLine += $"{skill.name} {skill.value},";
                    }
                    
                    peepcsv.WriteLine(peepnewLine);
                    peepnewLine = "";
                }
                File.AppendAllText("People.csv", peepcsv.ToString());

            }
            catch
            {
                
            }
            
            
        }
    }
    public class Region
    {
        public string name;
        public string type;
        public double bounty;
        public City city;
        public List<pe.Monster> monsters = new List<pe.Monster>();
    }
    public class City
    {
        public string name;
        public int[] location;
        public List<Road> roads = new List<Road>();
        public List<pe.Person> residents = new List<pe.Person>();
        public List<pe.Monster> monsters = new List<pe.Monster>();
        public List<Business> businesses = new List<Business>();
        public ItemShop itemShop;
        public ItemShop bakery;
        public Crafter blacksmith;
        public Hospital hospital;
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
                    //newShop.skillsNeeded.Add();

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

                case "Hospital":
                    Hospital hospital = new Hospital();

                    hospital.money = rnd.Next(50, 100);
                    hospital.payRate = 3;
                    hospital.maxWorkers = 3;
                    hospital.skillsNeeded.Add("Medical");

                    this.world.businesses.Add(hospital);
                    this.world.hospitals.Add(hospital);
                    this.hospital = hospital;
                    this.businesses.Add(hospital);
                    hospital.city = this;
                    break;

                case "Bakery":
                    ItemShop bakery = new ItemShop();

                    bakery.newItemStock("Bread", 10,20,10,false,false,false,false,true);
                    bakery.money = rnd.Next(100, 300);
                    bakery.payRate = 2;
                    bakery.maxWorkers = 2;
                    bakery.upsell = 0; //percentage increase; 1 doubles the cost
                    //newShop.skillsNeeded.Add();

                    this.world.businesses.Add(bakery);
                    this.world.shops.Add(bakery);
                    this.bakery = bakery;
                    this.businesses.Add(bakery);
                    bakery.city = this;
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

                foreach (City cityToCheck in world.cities)
                {
                    int x1 = cityLocation[0];
                    int y1 = cityLocation[1];
                    int x2 = cityToCheck.location[0];
                    int y2 = cityToCheck.location[1];

                    double distance = Math.Sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2));

                    if (distance <= 20) 
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
        public List<string> skillsNeeded = new List<string>();
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
            try{
                boughtItem.cost = Convert.ToInt32(newCost);
                buyerStock.stocks.Add(boughtItem);
            }
            catch (System.OverflowException)
            {
                //TODO: Make caught items artifacts
            }

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

        public void spawnItemInStock(string itemType){
            it.Stock stock = findStock(itemType);
            stock.stocks.Add(it.createItem(itemType));
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
                                    this.city.world.craftingJobs.Add(newJob);
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
    public class Hospital : Business
    {
        public List<pe.Person> patients = new List<pe.Person>();
    }
    public class Road
    {
        public string desc;
        public bool known;
        public double length;
        public double angle;
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
            double length,
            double angle,
            int roughness,
            string roadType)
        {
            this.source = source;
            this.target = target;
            this.desc = desc;
            this.length = length;
            this.angle = angle;
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
            this.jobSite.city.world.craftingJobs.Remove(this);
        }
    }

    public class Treatment : Job
    {
        public pe.Person patient;
    }
    public class History
        {
            public List<pe.Monster> deadMonsters = new List<pe.Monster>();
            public List<pe.Person> deadPeople = new List<pe.Person>();
            public List<it.Item> unusableItems = new List<it.Item>();
        }
    public static string randomName(string type = "")
    {
        var name = "";
        Random rnd = new Random();
        var cnsnnts = new List<string> {  //TODO: Add weight to make it more realistic
            "B",
            "C",
            "CH",
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
            "SH",
            "T",
            "TH",
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
            name += rnd.Next(1,5)==1 ? cnsnnts[rnd.Next(cnsnnts.Count - 1)] : "";
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

        //TODO: detect crossroads and create trade post there somehow
        foreach (City startcity in web.cities)
        {
            int roadCount = 0;

            //Build Roads
            while (roadCount <= 1)  //FIXME: Figure out why 1 is bad and 2 is too much
            {
                string closestCityName = "";
                double shortestDistance = 999999;

                foreach (City checkcity in web.cities)
                {
                    double checkDistance = findDistanceBetweenCities(startcity,checkcity);

                    if (checkDistance < shortestDistance && checkDistance > 0 && web.findRoad(startcity.name, checkcity.name) == null)
                    {
                        closestCityName = checkcity.name;
                        shortestDistance = checkDistance;
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
                    //roadCount++;
                }
                
                roadCount = startcity.findAllNeighbors().Count;
            }
        }

        foreach(City startcity in web.cities)
        {
            foreach(Road road in startcity.roads.ToList())
            {
                foreach(Road checkedRoad in startcity.roads.ToList())
                {
                    double angleDiff = Math.Abs(road.angle - checkedRoad.angle);

                    if (angleDiff < 19 && angleDiff != 0)
                    {
                        if(road.length >= checkedRoad.length)
                        {
                            //FIXME: road still exists as leaving target city
                            web.roads.Remove(road);
                            startcity.roads.Remove(road);
                        }
                        else
                        {
                            web.roads.Remove(checkedRoad);
                            startcity.roads.Remove(checkedRoad);
                        }
                    }
                }
            }
        }

        var edges = new StreamWriter("edgelist.csv");
        var nodes = new StreamWriter("nodelist.csv");

        foreach (City city in web.cities)
        {
            //var line = city.name + "," + city.location[0] + "," + city.location[1] + ",";
            //var line = "";
            foreach (City cityList in city.findAllNeighbors())
            {
                var edgeline = $"{city.name},{cityList.name},";
                edges.WriteLine(edgeline);
                edges.Flush();
            }
            var nodeline = $"{city.name},{city.location[0]},{city.location[1]}";
            nodes.WriteLine(nodeline);
            nodes.Flush();
        }

        //TODO: if cities are grouped together. Make them a kingdom. Otherwise, they are not part of a state

        web.datetime.Add("Year", 100);
        web.datetime.Add("Month", 1);
        web.datetime.Add("Day", 1);
        
        web.datetime.Add("Hour", 6);

        
        
        return web;

    }
    public static void populateWorldWithPeople()
    {
        Random rnd = new Random();

        double range = 0.7;
        double exp = 2.5;

        double num = Math.Pow(main.numberOfCities, exp);
        int lownum = Convert.ToInt32(num * (1 - range));
        int hinum = Convert.ToInt32(num * (1 + range));

        int numResidents = rnd.Next(lownum, hinum);

        for (int i = 0; i <= numResidents; i++)
        {
            pe.newPerson();
        }

        

    }
    public static void populateWorldWithSpawnMonsters()
    {
        foreach(City city in main.world.cities)
        {
            pe.newSpawnMonster(city);
        }

        main.world.monstersSpawed = true;
    }

    public static double findDistanceBetweenCities(City city1, City city2)
    {
        double distance;

        int x1 = city1.location[0];
        int y1 = city1.location[1];

        int x2 = city2.location[0];
        int y2 = city2.location[1];

        distance = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        distance = Math.Round(distance,1);

        return distance;
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

