/*
using r = random;

using os;

using t = time;

using g = gui;

using pe = people;

using ti = times;

using System.Collections.Generic;

using System.Linq;

using System;

using it = items;

public static class newCombat {
    
    public static int baddie = -1;
    
    public class statusEffect {
        
        public object duration;
        
        public object effect;
        
        public object effectValue;
        
        public object name;
        
        public object secondaryValue;
        
        public object timedEffect;
        
        public statusEffect(
            string name,
            string effect,
            int effectValue,
            int secondaryValue,
            int duration,
            int timedEffect = 0) {
            this.name = name;
            this.effect = effect;
            this.effectValue = effectValue;
            this.secondaryValue = secondaryValue;
            this.duration = duration;
            this.timedEffect = timedEffect;
        }
    }
    
    static newCombat() {
        @"
class activeStatus:
    def __init__(self, statusEffect, duration, effectStrength):
        self.statusEffect = statusEffect
        self.duration = duration
        self.effectStrength = effectStrength
";
    }
    
    public static void initCombat(object tempBaddie) {
        g.gwin.button1.grid();
        g.gwin.button2.grid();
        g.gwin.button3.grid();
        baddie = tempBaddie;
        setattr(baddie, "TIBS", 0);
        setattr(baddie, "status", new List<object>());
        setattr(baddie, "atk", 0);
        g.setText(label5: baddie.name + ": " + baddie.currentHP.ToString());
        var _tup_1 = readyMonsterAttack();
        baddie.TIBS = _tup_1.Item1;
        baddie.atk = _tup_1.Item2;
        playerTurn();
    }
    
    public static void readyPlayerAttack() {
        if (pe.me.attackType == "Q") {
            pe.me.TIBS = 30;
        }
        if (pe.me.attackType == "N") {
            pe.me.TIBS = 50;
        }
        if (pe.me.attackType == "S") {
            pe.me.TIBS = 60;
        }
        if (pe.me.attackType == "A") {
            pe.me.TIBS = 75;
        }
        tickUntilTurn();
    }
    
    public static Tuple<object, object> readyMonsterAttack() {
        //monsterActions = [5, 35, 100], [10, 5, 2], [75, 60, 30], [-10, 0, 5], ["powerful attack!", "strong attack.", "fast attack."]
        var actionSelect = r.randrange(100);
        var idx = 0;
        var monsterAttack = 0;
        foreach (var _tup_1 in baddie.atkRate.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            idx = _tup_1.Item1;
            var actionRate = _tup_1.Item2;
            if (actionSelect <= actionRate) {
                baddie.atk = idx;
                break;
            }
        }
        g.setText(label4: "The monster is making a {baddie.atkDesc[idx]} attack.");
        g.gwin.button0["text"] = "Continue";
        g.gwin.update();
        baddie.TIBS = baddie.atkTIBS[idx];
        g.gwin.button0["command"] = () => tickUntilTurn();
        return Tuple.Create(baddie.TIBS, monsterAttack);
    }
    
    public static void tickUntilTurn() {
        g.clearText(new List<int> {
            8
        });
        if (pe.me.retreating) {
            g.setText(label2: "You are running away.");
        }
        if (pe.me.dodging) {
            g.setText(label2: "You are dodging.");
        }
        if (pe.me.blocking) {
            g.setText(label2: "You are blocking.");
        }
        while (pe.me.TIBS > 0 && baddie.TIBS > 0) {
            g.updateStatus();
            var tempSpeedMod = 0;
            var Ticker = "...................................................................................................";
            foreach (var each in baddie.status) {
                if (each.effect == "slow") {
                    tempSpeedMod += each.effectValue;
                }
            }
            //todo combine both status checks
            var modSpeed = max(1, Convert.ToInt32(baddie.speed) + tempSpeedMod);
            pe.me.TIBS -= pe.me.speed + pe.me.equippedWeapon.secondaryEffectValue;
            if (!(from o in baddie.status
                select o.effect).ToList().Contains("stop")) {
                baddie.TIBS -= modSpeed;
            }
            pe.me.TIBS = max(0, pe.me.TIBS);
            baddie.TIBS = max(0, baddie.TIBS);
            Ticker = Ticker[0::pe.me.TIBS] + "P" + Ticker[pe.me.TIBS::len(Ticker)];
            Ticker = Ticker[0::baddie.TIBS] + "K" + Ticker[baddie.TIBS::len(Ticker)];
            g.gwin.TIBSlabel["text"] = Ticker;
            g.setText(label5: baddie.name + ": " + baddie.currentHP.ToString());
            g.gwin.update();
            if (baddie.status.Count == 0) {
                g.setText(label6: "");
            } else {
                foreach (var eff in baddie.status) {
                    // todo re-add
                    if (eff.effect == "poison") {
                        eff.timedEffect -= 1;
                        if (eff.timedEffect <= 0) {
                            eff.timedEffect = eff.secondaryValue;
                            damageBaddie(eff.effectValue);
                            g.setText(label8: "The monster takes {eff.effectValue} damage from poison");
                        }
                    }
                    eff.duration -= 1;
                    if (eff.duration <= 0) {
                        baddie.status.remove(eff);
                    }
                }
                g.setAllText(6, (from o in baddie.status
                    select (o.name + o.duration.ToString())).ToList());
            }
            t.sleep(0.1);
        }
        if (Convert.ToInt32(baddie.currentHP) <= 0) {
            killedTheMonster(baddie);
        }
        if (baddie.TIBS <= 0) {
            var _tup_1 = monsterTurn();
            baddie.TIBS = _tup_1.Item1;
            var monsterAction = _tup_1.Item2;
        } else if (pe.me.TIBS <= 0) {
            if (pe.me.blocking || pe.me.dodging) {
                playerTurn();
            } else {
                attack();
            }
        }
    }
    
    public static void tickUntilStatusClear(object monsterStatus) {
        var poisonDamage = 0;
        while (monsterStatus.Count > 0) {
            foreach (var each in monsterStatus) {
                if (each.statusEffect.effect == "poison") {
                    if (r.randrange(100) < each.statusEffect.secondaryValue) {
                        poisonDamage += r.randrange(Convert.ToInt32(each.statusEffect.effectValue / 2), each.statusEffect.effectValue);
                    }
                }
                // todo apply damage here
                // todo check if monster dies
                // todo option to return to loot
                each.duration -= 1;
                if (each.duration < 0) {
                    monsterStatus.remove(each);
                }
            }
        }
        if (poisonDamage >= 1) {
            Console.WriteLine("The monster takes {poisonDamage} damage from poison");
        }
    }
    
    public static Tuple<object, object> monsterTurn() {
        object blocking;
        object dodging;
        var atk = baddie.atk;
        if (pe.me.dodging) {
            dodging = Convert.ToInt32(pe.me.skills.Dodge / 2);
        } else {
            dodging = 0;
        }
        if (pe.me.blocking) {
            blocking = 2 + 2 * pe.me.equippedShield.baseEffectValue;
        } else {
            blocking = 0;
        }
        //todo blocking skill
        var dodgeChance = pe.me.skills.Dodge - Convert.ToInt32(pe.me.equippedArmor.secondaryEffectValue) + dodging;
        var hit = r.randrange(100) + baddie.atkMod[atk];
        var blockedDamage = Convert.ToInt32(pe.me.equippedArmor.baseEffectValue) - pe.me.equippedShield.baseEffectValue - blocking;
        var damage = r.randrange(baddie.atkStr[atk]) + baddie.strength - blockedDamage;
        g.setText(label1: "The monster attacks!");
        if (hit >= dodgeChance) {
            if (damage <= 0) {
                g.setText(label2: "You block all damage");
            } else if (damage > 0) {
                g.setText(label2: "you take {damage}");
                g.updateStatus();
                pe.me.currentHP -= damage;
                //g.gwin.HPLabel["text"]=f"HP: {pe.me.currentHP}/{pe.me.maxHP}"
                if (pe.me.currentHP <= 0) {
                    playerDied();
                } else if (r.randrange(100) >= pe.me.skills.Dodge) {
                    pe.me.skills.Dodge += 1;
                    g.setText(label3: "Your Dodge skill increases to {pe.me.skills.Dodge}!");
                }
            }
        } else {
            g.setText(label2: "You dodge");
        }
        g.gwin.button0["text"] = "Continue";
        g.gwin.button0["command"] = () => readyMonsterAttack();
        g.gwin.update();
        return Tuple.Create(baddie.atkTIBS[atk], atk);
    }
    
    public static void playerTurn() {
        pe.me.blocking = false;
        pe.me.dodging = false;
        if (pe.me.retreating) {
            pe.me.retreating = false;
            g.setText(label1: "You escape.");
            g.gwin.button0["text"] = "Return to Town";
            g.gwin.button0["command"] = () => g.dispTown();
            //todo re-add tickUntilStatusClear(baddie)
        } else {
            g.setText(label7: "Choose your action");
            if (pe.me.attackType == "Q") {
                g.gwin.button0["text"] = "Quick Attack";
            } else if (pe.me.attackType == "S") {
                g.gwin.button0["text"] = "Strong Attack";
            } else if (pe.me.attackType == "N") {
                g.gwin.button0["text"] = "Normal Attack";
            } else if (pe.me.attackType == "A") {
                g.gwin.button0["text"] = "Accurate Attack";
            }
            g.gwin.button0["command"] = () => readyPlayerAttack();
            g.gwin.button1["text"] = "Defend";
            g.gwin.button1["command"] = () => defend();
            g.gwin.button2["text"] = "Items";
            g.gwin.button2["command"] = () => g.initSelect("combat use:", pe.me, "inv", "name", "use", "combat");
            g.gwin.button3["text"] = "Tactics";
            g.gwin.button3["command"] = () => tactics();
        }
    }
    
    public static void setupAttackType() {
        g.gwin.button0["text"] = "Quick Attack";
        g.gwin.button0["command"] = () => setAttackType("Q");
        g.gwin.button1["text"] = "Normal Attack";
        g.gwin.button1["command"] = () => setAttackType("N");
        g.gwin.button2["text"] = "Strong Attack";
        g.gwin.button2["command"] = () => setAttackType("S");
        g.gwin.button3["text"] = "Accurate Attack";
        g.gwin.button3["command"] = () => setAttackType("A");
    }
    
    public static void setAttackType(object aT) {
        pe.me.attackType = aT;
        playerTurn();
    }
    
    public static object attack() {
        object attackBonus;
        object attackSkill;
        // todo put this at top of every func becuase double clicking can double attack
        g.gwin.button0["command"] = "";
        var eqWep = pe.me.equippedWeapon;
        try {
            attackSkill = getattr(pe.me.skills, eqWep.itemType);
        } catch {
            setattr(pe.me.skills, eqWep.itemType, 0);
            attackSkill = getattr(pe.me.skills, eqWep.itemType);
        }
        if (pe.me.attackType == "A") {
            attackBonus = 20;
        } else {
            attackBonus = 0;
        }
        var hitChance = attackSkill + eqWep.secondaryEffectValue + attackBonus;
        var damage = 0;
        g.setText(label1: "You attack!");
        if (r.randrange(100) <= hitChance) {
            damage = r.randrange(Convert.ToInt32(eqWep.baseEffectValue / 2), eqWep.baseEffectValue) + pe.me.strength;
            if (pe.me.attackType == "S") {
                damage += pe.me.strength;
            }
            if (pe.me.attackType == "Q") {
                damage -= pe.me.strength;
            }
            g.setText(label2: "You deal {damage}");
            damageBaddie(damage);
            if (eqWep.magicEffect == "Freezing") {
                if (r.randrange(100) <= eqWep.magicEffectValue) {
                    baddie.status.append(new statusEffect("Held", "stop", 0, 0, 30));
                }
            }
            if (eqWep.magicEffect == "Icy") {
                if (r.randrange(100) <= 100) {
                    baddie.status.append(new statusEffect("Slow", "speed", -3, 20, 10));
                }
            }
            @"
        if equippedWeapon.specialEffect == ""Firey"":
            if r.randrange(100) <= 40:
                temp = equippedWeapon.specialEffectValue
                print(f""You deal an additional {r.randrange(int(temp / 2), temp)} fire damage."")
        ";
            //if pe.me.attackType == "Q":
            //    pe.me.TIBS = 30
            //if pe.me.attackType == "S":
            //    pe.me.TIBS = 70
        } else {
            g.setText(label1: "You miss");
            //pe.me.TIBS = 25
            //if pe.me.attackType == "S":
            //    pe.me.TIBS += 10
            //todo update to UseSkill
            if (r.randrange(100) >= getattr(pe.me.skills, eqWep.itemType)) {
                setattr(pe.me.skills, eqWep.itemType, getattr(pe.me.skills, eqWep.itemType) + 1);
                g.setText(label2: "Your {eqWep.itemType} skill increases to {getattr(pe.me.skills, eqWep.itemType)}!");
                // todo cap at 100
            }
        }
        if (Convert.ToInt32(baddie.currentHP) > 0) {
            playerTurn();
        } else if (Convert.ToInt32(baddie.currentHP) == 0) {
            killedTheMonster(baddie);
        }
    }
    
    public static void damageBaddie(object damage) {
        baddie.currentHP -= damage;
        baddie.currentHP = max(0, baddie.currentHP);
        ti.createEvent(ti.now(), pe.me.name, "wounds", baddie, pe.me.location, extra: damage);
    }
    
    public static void defend() {
        g.gwin.button0["text"] = "Dodge";
        g.gwin.button0["command"] = () => dodge();
        g.gwin.button1["text"] = "Block";
        g.gwin.button1["command"] = () => block();
        g.gwin.button2["text"] = "Return";
        g.gwin.button2["command"] = () => playerTurn();
    }
    
    public static void dodge() {
        pe.me.dodging = true;
        pe.me.TIBS = 30;
        tickUntilTurn();
    }
    
    public static void block() {
        pe.me.blocking = true;
        pe.me.TIBS = 30;
        tickUntilTurn();
    }
    
    public static void tactics() {
        g.gwin.button0["text"] = "Attack Type";
        g.gwin.button0["command"] = () => setupAttackType();
        g.gwin.button1["text"] = "Retreat";
        g.gwin.button1["command"] = () => retreat();
        g.gwin.button2["text"] = "-";
        g.gwin.button2["command"] = "";
        g.gwin.button3["text"] = "Return";
        g.gwin.button3["command"] = () => playerTurn();
    }
    
    public static void retreat() {
        pe.me.retreating = true;
        pe.me.TIBS = 99;
        tickUntilTurn();
    }
    
    public static void killedTheMonster(object badd) {
        //todo clear Ticker
        var display = "You killed the {badd.name}!";
        ti.createEvent(ti.now(), pe.me.name, "kills", badd, pe.me.location);
        g.setText(label3: display);
        g.gwin.button0["text"] = "Loot";
        g.gwin.button0["command"] = () => g.initSelect(display, baddie, "inv", "itemType", "loot", "dispTown");
        g.gwin.button1["text"] = "Dissect";
        g.gwin.button1["command"] = () => dissect(badd);
    }
    
    public static void playerDied() {
        g.clearText();
        g.setText(label4: "You died!");
        if (os.path.exists("player.kr")) {
            os.remove("player.kr");
        }
        g.gwin.button0["text"] = "Start New";
        g.gwin.button0["command"] = "";
        g.gwin.button1["text"] = "Quit";
        g.gwin.button1["command"] = () => g.quitGame();
        //todo delete player.kr
    }
    
    public static void dissect(object baddy) {
        foreach (var each in baddy.addInv) {
            var testrand = r.randrange(100);
            if (pe.skillCheck("Dissection", each.harvestDifficulty)) {
                baddy.inv.append(each);
                baddy.addInv.remove(each);
                Console.WriteLine("add {each.name}");
                ti.timePasses(each.harvestTime);
            } else {
                Console.WriteLine("destroyed {each.name}");
                baddy.addInv.remove(each);
                var temptime = Convert.ToInt32(each.harvestTime / 2);
                if (temptime > 0) {
                    ti.timePasses(temptime);
                }
            }
        }
    }
}
*/