using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ActiveType {
    attack,
    heal,
}

public enum TargettingType { 
    none,
    enemy,
    allEnemies,
    ally,
    allAllies,
    all,
}

public class Active {

    public ActiveType type;
    public int amount;
    public TargettingType targettingType;

    public Active(ActiveType type, TargettingType targettingType, int amount) {
        this.type = type;
        this.targettingType = targettingType;
        this.amount = amount;
    }

    public Active(string data) {
        var split = data.Split('(');
        amount = int.Parse(split[1].Substring(0, split[1].Length - 1));

        switch (split[0]) {
            case "attack":
                type = ActiveType.attack; targettingType = TargettingType.enemy; break;
            case "attackAll":
                type = ActiveType.attack; targettingType = TargettingType.allEnemies; break;
            case "heal":
                type = ActiveType.heal; targettingType = TargettingType.ally; break;
            case "healAll":
                type = ActiveType.heal; targettingType = TargettingType.allAllies; break;

        } 

    }
}


