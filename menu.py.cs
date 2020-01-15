
using System.Collections.Generic;

using System.Linq;

using System;

public static class menu {
    
    public static int select = 0;
    
    public static List<string> menuItems = new List<string> {
        "Attack",
        "Tactics",
        "Items"
    };
    
    public static int select = Convert.ToInt32(input());
}
