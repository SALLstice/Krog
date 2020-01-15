
using r = random;

using nx = networkx;

using pe = people;

using w = worlds;

using System.Linq;

using System.Collections.Generic;

public static class boss {
    
    public static void findBoss() {
        foreach (var baddie in pe.persons) {
            if (baddie.name == "King krog") {
                kingKrog = baddie;
            }
        }
    }
    
    public static void createBoss() {
        kingKrog = pe.createPerson(4, 1, "King krog");
        setattr(kingKrog, "attackTarget", null);
        setattr(kingKrog, "sleepTimer", 5);
        setattr(kingKrog, "location", 83);
        setattr(kingKrog, "travelling", false);
        setattr(kingKrog, "travelRemaining", 0);
        setattr(kingKrog, "awake", false);
    }
    
    public static void awaken() {
        kingKrog.awake = true;
    }
    
    public static void attackTown() {
        var loc = kingKrog.location;
        var targ = getattr(kingKrog, "attackTarget");
        if (targ == null) {
            if ((from o in w.world.nodes[loc]["sites"]
                where !o.destroyed
                select o).ToList().Count == 0) {
                w.world.nodes[loc]["ruined"] = true;
                Console.WriteLine("{loc} left in ruins");
                bossTravel();
            } else {
                setattr(kingKrog, "attackTarget", r.choice((from o in w.world.nodes[loc]["sites"]
                    where !o.destroyed
                    select o).ToList()));
                targ = getattr(kingKrog, "attackTarget");
            }
        } else {
            targ.open = false;
            targ.currentHP -= 1;
            Console.WriteLine("attacks {targ.type}");
            // todo wear all items in stock
            if (targ.currentHP <= 0) {
                setattr(targ, "destroyed", true);
                Console.WriteLine("destroys {targ.name}");
                setattr(kingKrog, "attackTarget", null);
                if ((from o in w.world.nodes[loc]["sites"]
                    where !o.destroyed
                    select o).ToList().Count <= 0) {
                    w.world.nodes[loc]["ruined"] = true;
                    Console.WriteLine("{loc} left in ruins");
                    bossTravel();
                    // else:
                    //    setattr(kingKrog, 'attackTarget', r.choice([o for o in w.world.nodes[loc]['sites'] if not o.destroyed]))
                }
            }
        }
    }
    
    public static object bossTravel() {
        object dest;
        var loc = kingKrog.location;
        try {
            dest = r.choice((from o in w.world.neighbors(loc).ToList()
                where !w.world.nodes[o]["ruined"]
                select o).ToList());
        } catch (IndexError) {
            dest = findClosestNotRuin(loc);
        }
        Console.WriteLine("leaving {loc} for {dest}");
        setattr(kingKrog, "travelling", true);
        setattr(kingKrog, "location", new List<object> {
            loc,
            dest
        });
        try {
            setattr(kingKrog, "travelRemaining", findDistance(loc, dest));
        } catch {
            Console.WriteLine("dd");
        }
    }
    
    public static void bossArrive() {
        setattr(kingKrog, "travelling", false);
        kingKrog.location = kingKrog.location[1];
        Console.WriteLine("arrives in {kingKrog.location}");
    }
    
    public static int findClosestNotRuin(object loc) {
        var distance = 10000;
        var closestNode = -1;
        foreach (var n in w.world.nodes) {
            if (!w.world.nodes[n]["ruined"]) {
                if (nx.shortest_path(w.world, loc, n).Count < distance && loc != n) {
                    closestNode = n;
                    distance = nx.shortest_path(w.world, loc, n).Count;
                }
            }
        }
        if (closestNode != -1) {
            return closestNode;
        } else {
            Console.WriteLine("The world has ended...");
            quit();
        }
    }
    
    public static int findDistance(object start, object end) {
        var route = nx.shortest_path(w.world, start, end);
        var distance = 0;
        foreach (var _tup_1 in route.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            var idx = _tup_1.Item1;
            var val = _tup_1.Item2;
            if (idx > 0) {
                distance += w.world.edges[val,route[idx - 1]]["route"].length;
            }
        }
        return distance;
    }
}
