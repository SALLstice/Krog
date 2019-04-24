import tkinter as tk

import items as it
import people as pe
import places as pl
import times as ti
import worlds as w


def init():
    global gwin

    root = tk.Tk()
    root.withdraw()

    gwin = gui(root)
    gwin.attributes("-topmost", True)


class gui(tk.Tk):
    def __init__(self, master):
        tk.Tk.__init__(self)

        self.columnconfigure(1, weight=1)
        self.rowconfigure(0, weight=1)
        self.geometry("600x370")

        # wwidth = int(self.winfo_width())
        # STATUS_FRAME_SCALE = int(wwidth/5)

        # ****	STATUS	FRAME *****
        self.statusFrame = tk.Frame(self, bd=2, relief=tk.SUNKEN, name="statusFrame")
        self.statusFrame.grid(row=0, column=0, sticky="ns")
        # statusFrame.grid_propagate(0)

        self.nameL = tk.Label(self.statusFrame, text=pe.me.name)
        self.nameL.pack(side=tk.TOP)

        self.HPLabel = tk.Label(self.statusFrame, anchor=tk.W, text=f"HP: {pe.me.currentHP}/{pe.me.maxHP}")
        self.HPLabel.pack(side=tk.TOP, fill=tk.X)

        self.strL = tk.Label(self.statusFrame, anchor=tk.W, text=f"Str: {pe.me.strength}")
        self.strL.pack(side=tk.TOP, fill=tk.X)

        self.touL = tk.Label(self.statusFrame, anchor=tk.W, text=f"Tough: {pe.me.tough}")
        self.touL.pack(side=tk.TOP, fill=tk.X)

        self.ovSpdL = tk.Label(self.statusFrame, anchor=tk.W, text=f"Ov Spd: {pe.me.overlandSpeed}")
        self.ovSpdL.pack(side=tk.TOP, fill=tk.X)

        self.TIBSSpdL = tk.Label(self.statusFrame, anchor=tk.W, text=f"TIBS Spd: {pe.me.speed}")
        self.TIBSSpdL.pack(side=tk.TOP, fill=tk.X)

        self.div = tk.Label(self.statusFrame, text="----------")
        self.div.pack(side=tk.TOP, fill=tk.X)

        self.eqWeapL = tk.Label(self.statusFrame, anchor=tk.W, justify=tk.LEFT,
                                text=f"Wea: {pe.me.equippedWeapon.itemType}")
        self.eqWeapL.pack(side=tk.TOP, fill=tk.X)

        self.eqShiL = tk.Label(self.statusFrame, anchor=tk.W, text=f"Shi: {pe.me.equippedShield.itemType}")
        self.eqShiL.pack(side=tk.TOP, fill=tk.X)

        self.eqArmL = tk.Label(self.statusFrame, anchor=tk.W, text=f"Arm: {pe.me.equippedArmor.itemType}")
        self.eqArmL.pack(side=tk.TOP, fill=tk.X)

        self.eqAcc1L = tk.Label(self.statusFrame, anchor=tk.W, text=f"Acc 1: {pe.me.equippedAcc1.itemType}")
        self.eqAcc1L.pack(side=tk.TOP, fill=tk.X)

        self.eqAcc2L = tk.Label(self.statusFrame, anchor=tk.W, text=f"Acc 2: {pe.me.equippedAcc2.itemType}")
        self.eqAcc2L.pack(side=tk.TOP, fill=tk.X)

        self.div2 = tk.Label(self.statusFrame, text="----------")
        self.div2.pack(side=tk.TOP)

        self.locL = tk.Label(self.statusFrame, text=f"You are in {pe.me.location}.")
        self.locL.pack(side=tk.TOP)

        self.timeL = tk.Label(self.statusFrame, text=f"Time: {w.world.graph['hour']}:00")
        self.timeL.pack(side=tk.TOP)

        self.dateL = tk.Label(self.statusFrame,
                              text=f"Date: {w.world.graph['month']}/{w.world.graph['day']}/{w.world.graph['year']}")
        self.dateL.pack(side=tk.TOP)

        # *****BUTTON	FRAME ******
        self.buttonFrame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.buttonFrame.grid(row=2, column=0, columnspan=2, sticky="ew")

        buttonPadX = 10

        self.button0 = tk.Button(self.buttonFrame, text="root width")
        self.button0.grid(row=0, column=1, padx=buttonPadX, pady=2)

        self.button1 = tk.Button(self.buttonFrame, text="button 2")
        self.button1.grid(row=0, column=2, padx=buttonPadX, pady=2)

        self.button2 = tk.Button(self.buttonFrame, text="button 3")
        self.button2.grid(row=0, column=3, padx=buttonPadX, pady=2)

        self.button3 = tk.Button(self.buttonFrame, text="button 4")
        self.button3.grid(row=0, column=4, padx=buttonPadX, pady=2)

        self.buttonFrame.grid_columnconfigure(0, weight=2)
        self.buttonFrame.grid_columnconfigure(5, weight=2)

        # *****	MAIN	WINDOW	*****
        self.mainFrame = tk.Frame(self, bd=2, relief=tk.SUNKEN, name="mainFrame")
        self.mainFrame.grid(row=0, column=1, sticky="nsew")

        self.TIBSlabel = tk.Label(self.mainFrame,
                                  text="...................................................................................................")
        self.TIBSlabel.grid(row=0, column=0)

        self.gearButton = tk.Button(self.mainFrame, text="SET", command=lambda: settings())
        self.gearButton.grid(row=0, column=3)

        self.mainFrame.rowconfigure(1, weight=1)

        self.label0 = tk.Label(self.mainFrame, text="")
        self.label0.grid(row=2, column=0)

        self.label1 = tk.Label(self.mainFrame, text="")
        self.label1.grid(row=3, column=0)

        self.label2 = tk.Label(self.mainFrame, text="")
        self.label2.grid(row=4, column=0)

        self.label3 = tk.Label(self.mainFrame, text="")
        self.label3.grid(row=5, column=0)

        self.label4 = tk.Label(self.mainFrame, text="")
        self.label4.grid(row=6, column=0)

        self.label5 = tk.Label(self.mainFrame, text="")
        self.label5.grid(row=7, column=0)

        self.label6 = tk.Label(self.mainFrame, text="")
        self.label6.grid(row=8, column=0)

        self.label7 = tk.Label(self.mainFrame, text="")
        self.label7.grid(row=9, column=0)

        self.label8 = tk.Label(self.mainFrame, text="")
        self.label8.grid(row=10, column=0)

        self.label9 = tk.Label(self.mainFrame, text="")
        self.label9.grid(row=11, column=0)

        self.mainFrame.rowconfigure(12, weight=1)
        # self.mainFrame.columnconfigure(0,weight=1)

        self.textInput = tk.Entry(self.mainFrame)
        self.textInput.grid(row=13, column=0)


def dispTown():
    clearText()
    setText(label4=f"You are in  {str(pe.me.location)}.")

    gwin.button0.configure(text="Explore", command=lambda: pl.setupExploreRT())
    gwin.button1.configure(text="Status", command=lambda: status())
    gwin.button2.configure(text="Region", command=lambda: pl.visitRegionPlace())
    gwin.button3.configure(text="World", command="")


def settings():
    gwin.button3["text"] = "Quit"
    gwin.button3["bg"] = "red"
    gwin.button3["command"] = quitGame


def clearText():
    for i in range(9):
        tempstr = "label" + str(i)
        gwin.__dict__[tempstr]["text"] = ""


def setAllText(adjust=0, *args):
    for value, key in enumerate(args[0]):
        tempstr = "label" + str(value + adjust)
        gwin.__dict__[tempstr]["text"] = key

    gwin.update()


def setText(**kwargs):
    for value, key in enumerate(kwargs):
        gwin.__dict__[list(kwargs.keys())[value]]["text"] = list(kwargs.values())[value]
    gwin.update()


def quitGame():
    w.saveWorld()
    ti.printHistory()
    quit()


def status():
    gwin.button0["text"] = "Skills"
    gwin.button0["command"] = lambda: pe.dispSkills()

    gwin.button1["text"] = "Inventory"
    gwin.button1["command"] = lambda: it.inventory(pe.me, 'use', 'town')

    gwin.button2["text"] = "-"

    gwin.button3["text"] = "Return"
    gwin.button3["command"] = lambda: dispTown()


def initSelect(titleDisplay, holder, list, displayAttr, do, returnTo, selection=1, sellTo=""):
    gwin.button3.config(text="Return", command=lambda: retTo(returnTo))

    clearText()
    gwin.label0["text"] = titleDisplay

    if list == "":
        dispList = [getattr(o, displayAttr) for o in holder]
    else:
        dispList = [getattr(o, displayAttr) for o in getattr(holder, list)]

    setAllText(1, dispList)
    select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo)


def retTo(returnTo):
    if returnTo == "town":
        dispTown()


def select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo):
    # todo add scrolling functionality
    selectlabel = "label" + str(selection)
    holdlabel = gwin.__dict__[selectlabel]["text"]
    gwin.__dict__[selectlabel]["text"] = f"> {holdlabel}"

    gwin.button0["text"] = "^"
    gwin.button0["command"] = lambda: up(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection,
                                         sellTo)
    gwin.button2["text"] = "v"
    gwin.button2["command"] = lambda: down(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection,
                                           sellTo)

    gwin.button1["text"] = do
    gwin.button1["command"] = lambda: selectItem(titleDisplay, dispList, holder, list, displayAttr, do, returnTo,
                                                 selection, sellTo)


def down(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo):
    selectlabel = "label" + str(selection)
    holdlabel = gwin.__dict__[selectlabel]["text"]
    gwin.__dict__[selectlabel]["text"] = f"{holdlabel[2:]}"

    if selection < len(dispList):
        select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection + 1, sellTo)
    else:
        select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo)


def up(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo):
    selectlabel = "label" + str(selection)
    holdlabel = gwin.__dict__[selectlabel]["text"]
    gwin.__dict__[selectlabel]["text"] = f"{holdlabel[2:]}"

    if selection > 1:
        select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection - 1, sellTo)
    else:
        select(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo)


def selectItem(titleDisplay, dispList, holder, list, displayAttr, do, returnTo, selection, sellTo):
    # todo option to use or drop

    if do == "use":
        it.useItem(pe.me.inv[selection - 1], returnTo)

    if do == "site":
        pl.siteActivity(holder[selection - 1])

    if do == 'buy':
        it.buyItem(holder, selection)
        initSelect(titleDisplay, holder, list, displayAttr, do, returnTo, selection=1)

    if do == "sell":
        it.sellItem(sellTo, selection)
        initSelect(titleDisplay, holder, list, displayAttr, do, returnTo, 1, sellTo)

    if do == 'loot':
        holder = it.loot(holder, selection)
        if len(holder.inv) >= 1:
            initSelect(titleDisplay, holder, list, displayAttr, do, returnTo, selection=1)
        else:
            dispTown()


def setButtons(b0t="", b0c="", b1t="", b1c="", b2t="", b2c="", b3t="", b3c=""):
    if b0c == "":
        gwin.button0.grid_remove()
    else:
        gwin.button0["text"] = b0t
        gwin.button0["command"] = lambda: eval(b0c)

    if b1c == "":
        gwin.button1.grid_remove()
    else:
        gwin.button1["text"] = b1t
        gwin.button1["command"] = lambda: eval(b1c)

    if b2c == "":
        gwin.button2.grid_remove()
    else:
        gwin.button2["text"] = b2t
        gwin.button2["command"] = lambda: eval(b2c)

    if b3c == "":
        gwin.button3.grid_remove()
    else:
        gwin.button3["text"] = b3t
        gwin.button3["command"] = lambda: eval(b3c)
