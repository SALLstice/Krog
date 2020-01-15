
using tk = tkinter;

using ev = events;

using it = items;

using pe = people;

using pl = places;

using ti = times;

using w = worlds;

using System.Collections.Generic;

using System;

using System.Linq;

public static class gui {
    
    public static void init() {
        var root = tk.Tk();
        root.withdraw();
        gwin = new gui(root);
        gwin.attributes("-topmost", true);
        gwin.attributes("-topmost", false);
    }
    
    public class gui
        : tk.Tk {
        
        public object button0;
        
        public object button1;
        
        public object button2;
        
        public object button3;
        
        public object buttonFrame;
        
        public object dateL;
        
        public object div;
        
        public object div2;
        
        public object eqAcc1L;
        
        public object eqAcc2L;
        
        public object eqArmL;
        
        public object eqShiL;
        
        public object eqWeapL;
        
        public object gearButton;
        
        public object HPL;
        
        public object label0;
        
        public object label1;
        
        public object label2;
        
        public object label3;
        
        public object label4;
        
        public object label5;
        
        public object label6;
        
        public object label7;
        
        public object label8;
        
        public object label9;
        
        public object locL;
        
        public object mainFrame;
        
        public object nameL;
        
        public object ovSpdL;
        
        public object statusFrame;
        
        public object strL;
        
        public object textInput;
        
        public object TIBSlabel;
        
        public object TIBSSpdL;
        
        public object timeL;
        
        public object touL;
        
        public gui(object master) {
            this.columnconfigure(1, weight: 1);
            this.rowconfigure(0, weight: 1);
            this.geometry("600x370");
            // wwidth = int(self.winfo_width())
            // STATUS_FRAME_SCALE = int(wwidth/5)
            // ****	STATUS	FRAME *****
            this.statusFrame = tk.Frame(this, bd: 2, relief: tk.SUNKEN, name: "statusFrame");
            this.statusFrame.grid(row: 0, column: 0, sticky: "ns");
            // statusFrame.grid_propagate(0)
            this.nameL = tk.Label(this.statusFrame);
            this.nameL.pack(side: tk.TOP);
            this.HPL = tk.Label(this.statusFrame, anchor: tk.W);
            this.HPL.pack(side: tk.TOP, fill: tk.X);
            this.strL = tk.Label(this.statusFrame, anchor: tk.W);
            this.strL.pack(side: tk.TOP, fill: tk.X);
            this.touL = tk.Label(this.statusFrame, anchor: tk.W);
            this.touL.pack(side: tk.TOP, fill: tk.X);
            this.ovSpdL = tk.Label(this.statusFrame, anchor: tk.W);
            this.ovSpdL.pack(side: tk.TOP, fill: tk.X);
            this.TIBSSpdL = tk.Label(this.statusFrame, anchor: tk.W);
            this.TIBSSpdL.pack(side: tk.TOP, fill: tk.X);
            this.div = tk.Label(this.statusFrame, text: "----------");
            this.div.pack(side: tk.TOP, fill: tk.X);
            this.eqWeapL = tk.Label(this.statusFrame, anchor: tk.W, justify: tk.LEFT);
            this.eqWeapL.pack(side: tk.TOP, fill: tk.X);
            this.eqShiL = tk.Label(this.statusFrame, anchor: tk.W);
            this.eqShiL.pack(side: tk.TOP, fill: tk.X);
            this.eqArmL = tk.Label(this.statusFrame, anchor: tk.W);
            this.eqArmL.pack(side: tk.TOP, fill: tk.X);
            this.eqAcc1L = tk.Label(this.statusFrame, anchor: tk.W);
            this.eqAcc1L.pack(side: tk.TOP, fill: tk.X);
            this.eqAcc2L = tk.Label(this.statusFrame, anchor: tk.W);
            this.eqAcc2L.pack(side: tk.TOP, fill: tk.X);
            this.div2 = tk.Label(this.statusFrame, text: "----------");
            this.div2.pack(side: tk.TOP);
            this.locL = tk.Label(this.statusFrame);
            this.locL.pack(side: tk.TOP);
            this.timeL = tk.Label(this.statusFrame);
            this.timeL.pack(side: tk.TOP);
            this.dateL = tk.Label(this.statusFrame);
            this.dateL.pack(side: tk.TOP);
            // *****BUTTON	FRAME ******
            this.buttonFrame = tk.Frame(this, bd: 2, relief: tk.SUNKEN);
            this.buttonFrame.grid(row: 2, column: 0, columnspan: 2, sticky: "ew");
            var buttonPadX = 10;
            this.button0 = tk.Button(this.buttonFrame, text: "root width");
            this.button0.grid(row: 0, column: 1, padx: buttonPadX, pady: 2);
            this.button1 = tk.Button(this.buttonFrame, text: "button 2");
            this.button1.grid(row: 0, column: 2, padx: buttonPadX, pady: 2);
            this.button2 = tk.Button(this.buttonFrame, text: "button 3");
            this.button2.grid(row: 0, column: 3, padx: buttonPadX, pady: 2);
            this.button3 = tk.Button(this.buttonFrame, text: "button 4");
            this.button3.grid(row: 0, column: 4, padx: buttonPadX, pady: 2);
            this.buttonFrame.grid_columnconfigure(0, weight: 2);
            this.buttonFrame.grid_columnconfigure(5, weight: 2);
            // *****	MAIN	WINDOW	*****
            this.mainFrame = tk.Frame(this, bd: 2, relief: tk.SUNKEN, name: "mainFrame");
            this.mainFrame.grid(row: 0, column: 1, sticky: "nsew");
            this.TIBSlabel = tk.Label(this.mainFrame, text: "...................................................................................................");
            this.TIBSlabel.grid(row: 0, column: 0);
            this.gearButton = tk.Button(this.mainFrame, text: "SET", command: () => settings());
            this.gearButton.grid(row: 0, column: 3);
            this.mainFrame.rowconfigure(1, weight: 1);
            this.label0 = tk.Label(this.mainFrame, text: "");
            this.label0.grid(row: 2, column: 0);
            this.label1 = tk.Label(this.mainFrame, text: "");
            this.label1.grid(row: 3, column: 0);
            this.label2 = tk.Label(this.mainFrame, text: "");
            this.label2.grid(row: 4, column: 0);
            this.label3 = tk.Label(this.mainFrame, text: "");
            this.label3.grid(row: 5, column: 0);
            this.label4 = tk.Label(this.mainFrame, text: "");
            this.label4.grid(row: 6, column: 0);
            this.label5 = tk.Label(this.mainFrame, text: "");
            this.label5.grid(row: 7, column: 0);
            this.label6 = tk.Label(this.mainFrame, text: "");
            this.label6.grid(row: 8, column: 0);
            this.label7 = tk.Label(this.mainFrame, text: "");
            this.label7.grid(row: 9, column: 0);
            this.label8 = tk.Label(this.mainFrame, text: "");
            this.label8.grid(row: 10, column: 0);
            this.label9 = tk.Label(this.mainFrame, text: "");
            this.label9.grid(row: 11, column: 0);
            this.mainFrame.rowconfigure(12, weight: 1);
            // self.mainFrame.columnconfigure(0,weight=1)
            this.textInput = tk.Entry(this.mainFrame);
            this.textInput.grid(row: 13, column: 0);
        }
    }
    
    public static void updateStatus() {
        gwin.nameL["text"] = pe.me.name;
        gwin.HPL["text"] = "HP: {pe.me.currentHP}/{pe.me.maxHP}";
        gwin.strL["text"] = "Str: {pe.me.strength}";
        gwin.touL["text"] = "Tough: {pe.me.tough}";
        gwin.ovSpdL["text"] = "Ov Spd: {pe.me.overlandSpeed}";
        gwin.TIBSSpdL["text"] = "TIBS Spd: {pe.me.speed}";
        gwin.eqWeapL["text"] = "Wea: {pe.me.equippedWeapon.name}";
        gwin.eqShiL["text"] = "Shi: {pe.me.equippedShield.itemType}";
        gwin.eqArmL["text"] = "Arm: {pe.me.equippedArmor.itemType}";
        gwin.eqAcc1L["text"] = "Acc 1: {pe.me.equippedAcc1.itemType}";
        gwin.eqAcc2L["text"] = "Acc 2: {pe.me.equippedAcc2.itemType}";
        gwin.locL["text"] = "{w.world.nodes[pe.me.location]['name']}";
        //gwin.timeL['text'] = f"Time: {w.world['hour']}:00"
        //gwin.dateL['text'] = f"Date: {w.world['month']}/{w.world['day']}/{w.world['year']}"
        //todo figure out why world keeps losing time
    }
    
    public static void dispTown() {
        gwin.button0.grid();
        gwin.button1.grid();
        gwin.button2.grid();
        gwin.button3.grid();
        clearText();
        setText(label4: "You are in  {w.world.nodes[pe.me.location]['name']}.");
        gwin.button0.configure(text: "Explore", command: () => pl.setupExploreRT());
        gwin.button1.configure(text: "Status", command: () => status());
        gwin.button2.configure(text: "Region", command: () => pl.visitRegionPlace());
        gwin.button3.configure(text: "World", command: () => initSelect("Travel", w.world[pe.me.location], "_atlas", "description", "travel", "krog"));
    }
    
    public static object setName(object text) {
        object gen;
        var nameList = new List<object>();
        var highest = 0;
        pe.nameDebugCheck(text);
        if (text == "") {
            text = "Satchmo";
        }
        foreach (var @event in ti.history) {
            if (@event.person.Contains(text)) {
                nameList.append(@event.person);
            }
        }
        if (nameList.Count > 0) {
            foreach (var names in nameList) {
                var tmp = names.split();
                try {
                    gen = Convert.ToInt32(tmp[tmp.Count - 1]);
                } catch {
                    gen = tmp[tmp.Count - 1];
                }
                if (type(gen) == @int) {
                    if (gen > highest) {
                        highest = gen;
                    }
                } else if (tmp[tmp.Count - 1] == "Jr.") {
                    highest = 3;
                } else {
                    highest = 2;
                }
            }
            if (highest > 0) {
                if (highest == 2) {
                    text += " Jr.";
                } else if (highest > 2) {
                    text += " " + (highest + 1).ToString();
                    //todo Roman numerals
                }
            }
        }
        pe.me.name = text;
        gwin.nameL["text"] = text;
        gwin.textInput.delete(0, "end");
        pl.arrive();
    }
    
    public static void settings() {
        gwin.button3["text"] = "Quit";
        gwin.button3["bg"] = "red";
        gwin.button3["command"] = quitGame;
    }
    
    public static void clearText(List<object> skip = new List<object>()) {
        foreach (var i in Enumerable.Range(0, 9)) {
            if (!skip.Contains(i)) {
                var tempstr = "label" + i.ToString();
                gwin.@__dict__[tempstr]["text"] = "";
            }
        }
    }
    
    public static void setAllText(int adjust = 0, params object [] args) {
        foreach (var _tup_1 in args[0].Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            var value = _tup_1.Item1;
            var key = _tup_1.Item2;
            var tempstr = "label" + (value + adjust).ToString();
            gwin.@__dict__[tempstr]["text"] = key;
        }
        gwin.update();
    }
    
    public static void setText(string kwargs) {
        foreach (var _tup_1 in kwargs.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            var value = _tup_1.Item1;
            var key = _tup_1.Item2;
            gwin.@__dict__[kwargs.keys().ToList()[value]]["text"] = kwargs.values().ToList()[value];
        }
        gwin.update();
    }
    
    public static void quitGame() {
        w.saveWorld();
        if (pe.me.currentHP > 0) {
            pe.savePlayer();
        }
        ti.printHistory();
        quit();
    }
    
    public static void status() {
        gwin.button0["text"] = "Skills";
        gwin.button0["command"] = () => pe.dispSkills();
        gwin.button1["text"] = "Inventory";
        gwin.button1["command"] = () => it.inventory(pe.me, "use", "krog");
        gwin.button2["text"] = "-";
        gwin.button3["text"] = "Return";
        gwin.button3["command"] = () => dispTown();
    }
    
    public static void initSelect(
        object titleDisplay,
        object holder,
        Func<object> holderList,
        object displayAttr,
        object @do,
        object returnTo,
        int selection = 1,
        string sellTo = "",
        List<object> doNotClear = new List<object>()) {
        gwin.button3.config(text: "Return", command: () => retTo(returnTo));
        var dispList = new List<object>();
        clearText(doNotClear);
        gwin.label0["text"] = titleDisplay;
        var loc = pe.me.location;
        if (@do == "travel") {
            foreach (var j in Enumerable.Range(0, w.world.neighbors(loc).ToList().Count)) {
                //dispList.append(w.world[loc][list(w.world.neighbors(loc))[j]]['route'].desc)
                if (w.world[loc][w.world.neighbors(loc).ToList()[j]]["route"].known == 0) {
                    // check if the road is unknown
                    dispList.append(w.world[loc][w.world.neighbors(loc).ToList()[j]]["route"].desc);
                } else {
                    //check if road is known
                    dispList.append("The road leading to {w.world.nodes[list(w.world.neighbors(loc))[j]]['name']}");
                }
            }
            holderList = w.world.neighbors(loc).ToList();
        } else if (@do == "buy") {
            foreach (var each in holder.stocks) {
                dispList.append("{len(each.entities)}x {each.item.itemType} {each.item.cost}c");
            }
        } else if (holderList == "") {
            dispList = (from o in holder
                select getattr(o, displayAttr)).ToList();
            //for attrs in displayAttr:
            //    tempstr += getattr(holder, attrs) + " -- "
            //
            //dispList.append(tempstr)
        } else if (type(displayAttr) == list) {
            foreach (var item in getattr(holder, holderList)) {
                var tempstr = "";
                foreach (var attrs in displayAttr) {
                    tempstr += getattr(item, attrs).ToString() + " -- ";
                }
                tempstr = tempstr[:: - 4];
                dispList.append(tempstr);
            }
        } else if (displayAttr == "") {
            dispList = (from o in getattr(holder, holderList)
                select o).ToList();
        } else {
            dispList = (from o in getattr(holder, holderList)
                select getattr(o, displayAttr)).ToList();
        }
        setAllText(1, dispList);
        select(titleDisplay, dispList, holder, holderList, displayAttr, @do, returnTo, selection, sellTo);
    }
    
    public static void retTo(object returnTo) {
        if (returnTo == "krog") {
            pl.arrive();
        } else {
            pl.arrive();
        }
    }
    
    public static void select(
        object titleDisplay,
        List<object> dispList,
        object holder,
        Func<object> list,
        object displayAttr,
        object @do,
        object returnTo,
        int selection,
        object sellTo) {
        // todo add scrolling functionality
        var selectlabel = "label" + selection.ToString();
        var holdlabel = gwin.@__dict__[selectlabel]["text"];
        gwin.@__dict__[selectlabel]["text"] = "> {holdlabel}";
        gwin.button0["text"] = "^";
        gwin.button0.grid();
        gwin.button0["command"] = () => up(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection, sellTo);
        gwin.button2["text"] = "v";
        gwin.button2.grid();
        gwin.button2["command"] = () => down(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection, sellTo);
        gwin.button1["text"] = @do;
        gwin.button1.grid();
        gwin.button1["command"] = () => selectItem(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection, sellTo);
    }
    
    public static void down(
        object titleDisplay,
        object dispList,
        object holder,
        object list,
        object displayAttr,
        object @do,
        object returnTo,
        object selection,
        object sellTo) {
        var selectlabel = "label" + selection.ToString();
        var holdlabel = gwin.@__dict__[selectlabel]["text"];
        gwin.@__dict__[selectlabel]["text"] = "{holdlabel[2:]}";
        if (selection < dispList.Count) {
            select(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection + 1, sellTo);
        } else {
            select(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection, sellTo);
        }
    }
    
    public static void up(
        object titleDisplay,
        object dispList,
        object holder,
        object list,
        object displayAttr,
        object @do,
        object returnTo,
        object selection,
        object sellTo) {
        var selectlabel = "label" + selection.ToString();
        var holdlabel = gwin.@__dict__[selectlabel]["text"];
        gwin.@__dict__[selectlabel]["text"] = "{holdlabel[2:]}";
        if (selection > 1) {
            select(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection - 1, sellTo);
        } else {
            select(titleDisplay, dispList, holder, list, displayAttr, @do, returnTo, selection, sellTo);
        }
    }
    
    public static void selectItem(
        object titleDisplay,
        object dispList,
        object holder,
        object list,
        object displayAttr,
        object @do,
        object returnTo,
        object selection,
        object sellTo) {
        // todo option to use or drop
        if (@do == "use") {
            it.useItem(pe.me.inv[selection - 1], returnTo);
        }
        if (@do == "site") {
            pl.siteActivity(holder[selection - 1]);
        }
        if (@do == "buy") {
            it.buyItem(holder, selection);
            // initSelect(titleDisplay, holder, list, displayAttr, do, returnTo, selection=1)
            pl.arrive();
        }
        if (@do == "sell") {
            it.sellItem(sellTo, selection);
            initSelect(titleDisplay, holder, list, displayAttr, @do, returnTo, 1, sellTo);
        }
        if (@do == "loot") {
            holder = it.loot(holder, selection);
            if (holder.inv.Count >= 1) {
                initSelect(titleDisplay, holder, list, displayAttr, @do, returnTo, selection: 1);
            } else {
                dispTown();
            }
        }
        if (@do == "brew") {
            it.brew(holder, selection - 1);
        }
        if (@do == "travel") {
            var dest = list[selection - 1];
            pl.setupTravelRT(dest);
        }
        if (@do == "event") {
            ev.eventResults(holder.result[selection - 1]);
        }
    }
    
    public static void setButtons(
        string b0t = "",
        string b0c = "",
        string b1t = "",
        string b1c = "",
        string b2t = "",
        string b2c = "",
        string b3t = "",
        string b3c = "") {
        if (b0c == "") {
            gwin.button0.grid_remove();
        } else {
            gwin.button0["text"] = b0t;
            gwin.button0["command"] = () => eval(b0c);
        }
        if (b1c == "") {
            gwin.button1.grid_remove();
        } else {
            gwin.button1["text"] = b1t;
            gwin.button1["command"] = () => eval(b1c);
        }
        if (b2c == "") {
            gwin.button2.grid_remove();
        } else {
            gwin.button2["text"] = b2t;
            gwin.button2["command"] = () => eval(b2c);
        }
        if (b3c == "") {
            gwin.button3.grid_remove();
        } else {
            gwin.button3["text"] = b3t;
            gwin.button3["command"] = () => eval(b3c);
        }
    }
}
