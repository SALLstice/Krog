/*
using os;
using b = boss;
using ev = events;
using g = gui;
using it = items;
using pl = places;
using t = times;
*/

using pe = people;
using w = worlds;
using System;
using System.IO;

public class main {
    public static int worldSize = 100;
    public static int numberOfCities = 30;
    public static int infestation = 1;

    public static w.world world = new w.world();
    public static pe.Player me = new pe.Player("No Name");

    static void Main() {

        // g.init();
        //object world;

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
            if (true){
                Console.WriteLine("Building World...");
                world = w.buildWorld(worldSize, numberOfCities, infestation);
                w.populateWorld();
                me = pe.newPlayer("Joe");

                /*
                b.createBoss();
                t.timePasses(1000);
                w.saveWorldState();
                w.saveWorld();
                */
            }
        }
        Console.WriteLine("Done");
        //g.dispTown();
        //g.gwin.mainloop();
    }
    
    // todo different race options?
    // todo adult monsters can birth babies
    
}
