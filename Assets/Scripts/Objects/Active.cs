using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DamageType {
    none,
    physical,
    piercing,
    fire,
    cold,
    shock,
    heal,
}

public enum ActiveType {
    dmg,
    magic,
    heal,
    buff,
    wait,
}

public enum TargettingMode { 
    Target,
    Self,
    AllEnemies,
    RandomEnemy,
    AllFriendly,
    AllOtherFriendly,
}

public class Active {

    public ActiveType type;
    public int amount;
    public TargettingMode targettingMode;
    public BuffType buff;
    public DamageType damageType;

    public Active(ActiveType type, TargettingMode targettingType, int amount) {
        this.type = type;
        this.targettingMode = targettingType;
        this.amount = amount;
    }

    public Active(string data) {
        var split = data.Split('(');
        var parameters = split[1].Split(' ');

        switch (split[0]) {
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
            case "magic":
                type = ActiveType.magic;
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

        }
    }

    private TargettingMode GetTargettingMode(string s) {
        switch (s) {
            default: return TargettingMode.Target;
            case "s": return TargettingMode.Self;
            case "e": return TargettingMode.AllEnemies;
            case "re": return TargettingMode.RandomEnemy;
            case "f": return TargettingMode.AllFriendly;
            case "o": return TargettingMode.AllOtherFriendly;

        }
    }

    private DamageType getDamageType(string s) {
        switch (s) {
            default: return DamageType.physical;
            case "pierce": return DamageType.piercing;
            case "fire": return DamageType.fire;
            case "cold": return DamageType.cold;
            case "shock": return DamageType.shock;
            case "heal": return DamageType.heal;
        }
    }
}


