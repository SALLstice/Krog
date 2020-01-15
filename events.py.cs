
using csv;

using g = gui;

using pe = people;

using System.Collections.Generic;

using System.Collections;

using System.Linq;

public static class events {
    
    public static List<object> EVENT_HEADERS = new List<object>();
    
    public static List<object> eventList = new List<object>();
    
    public class @event {
        
        public @event(Hashtable kwargs, params object [] args) {
            foreach (var each in EVENT_HEADERS) {
                setattr(this, each, args);
            }
        }
    }
    
    public static object initEvents() {
        using (var evf = open("events.csv")) {
            reader = csv.reader(evf);
            EVENT_HEADERS = next(reader);
            foreach (var row in reader) {
                eventList.append(new @event(row));
                foreach (var _tup_1 in EVENT_HEADERS.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
                    idx = _tup_1.Item1;
                    attr = _tup_1.Item2;
                    try {
                        tempval = eval(row[idx]);
                    } catch {
                        tempval = row[idx];
                    }
                    setattr(eventList[eventList.Count - 1], attr, tempval);
                }
            }
        }
    }
    
    public static void runEvent(object e) {
        g.initSelect(e.introText, e, "options", "", "event", "krog");
    }
    
    public static void eventResults(object result) {
        if (result == "greed") {
            g.clearText();
            g.setText(label4: "You feel greedy");
        } else if (result == "beggardivine") {
            if (pe.me.money >= 1) {
                pe.me.money -= 1;
            }
        }
    }
}
