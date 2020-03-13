
//using os;
//using b = boss;
//using ev = events;
//using g = gui;
using it = items;
//using pl = places;
using t = times;


using pe = people;
using w = worlds;
using System;
using System.IO;
using System.Diagnostics;

public class main {
    public static int worldSize = 1000;
    public static int numberOfCities = 30;
    public static int infestation = 1;
    public static w.World world = new w.World();
    public static pe.Player me = new pe.Player();

    static void Main() {

        // g.init();
        //object world;
        Stopwatch sw = new Stopwatch();

        if (File.Exists("world/world.ks"))
        {
            if (File.Exists("player.kr")) 
            {
                /*
                w.loadWorld();
                it.initMaterials();
                it.initItemTypeList();
                pe.initPersonTypeList();
                pl.initPlaceTypeList();
                ev.initEvents();
                pe.loadPlayer();
                pl.arrive();
                */
            } 
            else
            {
                /*
                w.openInitialWorld();
                it.initItemTypeList();
                pe.initPersonTypeList();
                pl.initPlaceTypeList();
                ev.initEvents();
                pe.createPlayer("human", 0);
                */
            }
        } 
        else
        {
            if (worldSize >= 3){
                File.Delete("edgelist.csv");
                File.Delete("nodelist.csv");
                File.Delete("Population.csv");
                File.Delete("Business.csv");
                File.Delete("People.csv");
                    
                Console.Write("Building World... ");
                sw.Start();
                world = w.buildWorld(worldSize, numberOfCities, infestation);
                sw.Stop();
                Console.Write("Done in ");
                Console.Write(sw.ElapsedMilliseconds);
                Console.WriteLine(" milliseconds.");

                Console.Write("Populating World... ");
                sw.Start();
                w.populateWorldWithPeople();
                sw.Stop();
                Console.Write("Done in ");
                Console.Write(sw.ElapsedMilliseconds);
                Console.WriteLine(" milliseconds.");

                var popcsv = new StringWriter();
                var popnewLine = "";
                var buscsv = new StringWriter();
                var busnewLine = "";

                foreach (w.Business business in world.businesses)
                {
                    busnewLine += ","+$"{business.city.name}-{business.GetType()}";
                }
                foreach (w.City city in world.cities)
                {
                    popnewLine += ","+$"{city.name} Humans,"+$"{city.name} Monsters ";
                }
                
                popcsv.WriteLine(popnewLine);
                buscsv.WriteLine(busnewLine);

                popcsv.WriteLine(popnewLine);
                File.AppendAllText("Population.csv", popcsv.ToString());
                buscsv.WriteLine(busnewLine);
                File.AppendAllText("Business.csv", buscsv.ToString());

                Console.Write("Simulating World... ");
                sw.Start();
                world.passTime(2, "week");
                sw.Stop();
                Console.Write("Done in ");
                Console.Write(sw.ElapsedMilliseconds);
                Console.WriteLine(" milliseconds.");

                Console.WriteLine("Monsters Appear... ");
                sw.Start();
                w.populateWorldWithSpawnMonsters();
                
                Console.WriteLine("Simulating World... ");
                world.passTime(1, "month");
                sw.Stop();
                Console.Write("Done in ");
                Console.Write(sw.ElapsedMilliseconds);
                Console.WriteLine(" milliseconds.");
                
                
                me = pe.newPlayer("Joe");
                /*
                b.createBoss();
                w.saveWorldState();
                w.saveWorld();
                */
            }
        }
        Console.WriteLine("Done");
        
        char val = 'x';

        while(val != 'q'){
            Console.WriteLine($"You are in {me.city.name}: ");
            Console.WriteLine($"It is {world.nowString()}");
            val = Console.ReadKey().KeyChar;
                
            switch(val){

                //FIGHT
                case 'f':   
                    pe.Monster badGuy = findMonsterToFight();
                    if(!(badGuy is null)){
                        enterCombatWith(badGuy);
                        world.passTime(1, "hour");
                    }
                    break;

                //REST
                case 'r':   
                    while(me.HP < me.maxHP){
                        world.passTime(1, "day");
                        me.HP += 5;
                    }
                    break;
                
                 //DRUID
                case 'd':  
                    Console.WriteLine($"There are {me.city.monsters.Count} monsters left in this area.");
                    break;
                
                //MOVE
                case 'm':
                    Console.WriteLine("0: Stay");
                    int count = 1;
                    var neighbors = me.city.findAllNeighbors();
                    foreach(w.City neighbor in neighbors){
                        Console.WriteLine($"{count}: {neighbor.name}");
                        count++;
                    }
                    val = Console.ReadKey().KeyChar;
                    int valdou = (int)Char.GetNumericValue(val);
                    if(valdou > 0){
                        var destination = neighbors[valdou-1];
                        var roadTravelled = world.findRoad(me.city.name,destination.name);
                        var timeToTravel = Math.Max(1,(int)(roadTravelled.length / 3));//FIXME:chage 3 to player overland speed
                        world.passTime(timeToTravel,"hour");
                        Console.WriteLine($"It took you {timeToTravel} hours to walk {roadTravelled.length} miles.");
                        me.city = destination;
                    }
                    break;
                    
                //INVENTORY
                case 'i':  
                    Console.WriteLine($"Equipped Weapon:{me.equippedWeapon.itemType}");
                    Console.WriteLine($"Equipped Weapon Damage:{me.equippedWeapon.baseEffectValue} ({me.equippedWeapon.baseEffectValue + (int)me.strength/2} - {me.equippedWeapon.baseEffectValue + me.strength})");
                    Console.WriteLine($"Equipped Weapon Kills:{me.equippedWeapon.kills}");
                    Console.WriteLine($"Equipped Armor:{me.equippedArmor.itemType}");
                    break;

                default:
                    break;
            }
        }
    }

    static pe.Monster findMonsterToFight()
    {
        Random rnd = new Random();
        int monCount = me.city.monsters.Count;
        if (monCount > 0){
            pe.Monster badGuy = main.me.city.monsters[rnd.Next(monCount - 1)];
            return badGuy;
        }
        else{
            Console.WriteLine("There are no more monsters here.");
            return null;
        }

    }

    static void enterCombatWith(pe.Monster badGuy){
        char action = 'x';
        Random rnd = new Random();

        while(true){
            if (me.HP <= 0){
                return;
            }
            if(badGuy.HP <= 0){
                return;
            }

            Console.WriteLine($"You:{me.HP}      Monster:{badGuy.HP}");
            action = Console.ReadKey().KeyChar;

            switch(action){
                case 'a':
                    me.attack(badGuy);
                    badGuy.attack(me);
                    
                    break;
                case 'r':
                    badGuy.attack(me);
                
                    return;

                case 'i':
                    Console.WriteLine($"Strength: {badGuy.strength}");
                    Console.WriteLine($"Unarmed: {Convert.ToString(badGuy.skillValue("Unarmed"))}");
                    Console.WriteLine($"Dodge: {Convert.ToString(badGuy.skillValue("Dodge"))}");
                    
                    
                    break;
                default:
                    break;
            }
        }
    }

    // TODO different race options?
   
    
}
