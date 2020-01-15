/*
using os;
using b = boss;
using ev = events;
using g = gui;
using it = items;
using pe = people;
using pl = places;
using t = times;
using w = worlds;
*/
using System;
using System.IO;

public static class main {
    public static int worldSize = 25;
    public static int infestation = 1;
    static void Main() {

        // g.init();

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
            if (worldSize >= 2){
                /*
                w.world = w.buildWorld(worldSize, infestation);
                pe.createPlayer("human", 0);
                b.createBoss();
                t.timePasses(1000);
                w.saveWorldState();
                w.saveWorld();
                */
            }
        }

        //g.dispTown();
        //g.gwin.mainloop();
    }
    
    // todo different race options?
    // todo adult monsters can birth babies
    
}
