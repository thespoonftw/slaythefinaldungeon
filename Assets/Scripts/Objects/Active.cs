using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DamageType {
    none,
    physical,
    impact,
    fire,
    cold,
    shock,
    heal,
}

public enum ActiveType {
    dmg,
    heal,
    buff,
    wait,
    advance,
    push,
    pull,
}

public enum ActiveTarget { 
    Target,
    Self,
    AllHeroes,
    AllMonsters,
    MonstersRow1,  
    Adjacent,
}

public enum ScalingAttribute {
    None,
    Strength,
    Magic,
}

public class Active {

    public ActiveType type;
    public int amount;
    public ActiveTarget targettingMode;
    public BuffType buff;
    public DamageType damageType;

    public ScalingAttribute ScalingAttribute => (damageType == DamageType.physical || damageType == DamageType.impact) ? ScalingAttribute.Strength : ScalingAttribute.Magic;

    public Active(string data) {
        var split = data.Split('(');
        var parameters = split[1].Split(' ');

        switch (split[0]) {
            default:
                Tools.LogError("unknown active " + split[0]);
                break;
            case "dmg":
                type = ActiveType.dmg;
                targettingMode = GetTargettingMode(parameters[0]);
                amount = int.Parse(parameters[1]);
                if (parameters.Length < 3) {
                    damageType = DamageType.physical;
                } else {
                    damageType = getDamageType(parameters[2]);
                }
                break;
            case "heal":
                type = ActiveType.heal;
                targettingMode = GetTargettingMode(parameters[0]);
                amount = int.Parse(parameters[1]);
                break;
            case "wait":
                type = ActiveType.wait;
                amount = int.Parse(parameters[0]);
                break;
            case "buff":
                type = ActiveType.buff;
                targettingMode = GetTargettingMode(parameters[0]);
                amount = int.Parse(parameters[1]);
                buff = Tools.ParseEnum<BuffType>(parameters[2]);
                break;
            case "advance":
                type = ActiveType.advance;
                break;
            case "push":
                type = ActiveType.push;
                targettingMode = GetTargettingMode(parameters[0]);
                break;
            case "pull":
                type = ActiveType.pull;
                targettingMode = GetTargettingMode(parameters[0]);
                break;

        }
    }

    public static List<Active> GetActives(string input) {
        var returner = new List<Active>();
        string[] splitStrings = { ") ", ")" };
        var split = input.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in split) { returner.Add(new Active(s)); }
        return returner;
    }

    private ActiveTarget GetTargettingMode(string s) {
        switch (s) {
            case "t": return ActiveTarget.Target;
            case "s": return ActiveTarget.Self;
            case "h": return ActiveTarget.AllHeroes;
            case "e1": return ActiveTarget.MonstersRow1;            
            case "e": return ActiveTarget.AllMonsters;
            case "adj": return ActiveTarget.Adjacent;
            default: Tools.LogError("Unknown Action Targetting Mode " + s); return ActiveTarget.Target;
        }
    }

    private DamageType getDamageType(string s) {
        switch (s) {
            case "impact": return DamageType.impact;
            case "fire": return DamageType.fire;
            case "cold": return DamageType.cold;
            case "shock": return DamageType.shock;
            case "heal": return DamageType.heal;
            default: Tools.LogError("Unknown Damage Type " + s); return DamageType.physical;
        }
    }
}


