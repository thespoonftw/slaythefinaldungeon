using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ActiveType {
    dmg,
    heal,
    buff,
}

public enum TargettingType { 
    Self,
    Enemy,
    AllEnemies,
    Adjacent,
    AllAdjacent,   
    Friendly,
    AllFriendly,
}

public class Active {

    public ActiveType type;
    public int amount;
    public TargettingType targettingType;
    public BuffType buff;

    public Active(ActiveType type, TargettingType targettingType, int amount) {
        this.type = type;
        this.targettingType = targettingType;
        this.amount = amount;
    }

    public Active(string data) {
        var split = data.Split('-');

        var splitFirst = split[0].Split('(');

        switch (splitFirst[0]) {
            case "dmg":
                type = ActiveType.dmg; break;
            case "heal":
                type = ActiveType.heal; break;
            case "buff":
                type = ActiveType.buff;
                var buffcode = splitFirst[1].Substring(0, splitFirst[1].Length - 1);
                buff = Helper.ParseEnum<BuffType>(buffcode);
                break;

        }

        switch (split[1]) {
            case "s":
                targettingType = TargettingType.Self; break;
            case "e":
                targettingType = TargettingType.Enemy; break;
            case "ae":
                targettingType = TargettingType.AllEnemies; break;
            case "a":
                targettingType = TargettingType.Adjacent; break;
            case "aa":
                targettingType = TargettingType.AllAdjacent; break;
            case "f":
                targettingType = TargettingType.Friendly; break;
            case "af":
                targettingType = TargettingType.AllFriendly; break;

        }


        amount = int.Parse(split[2]);


    }
}


