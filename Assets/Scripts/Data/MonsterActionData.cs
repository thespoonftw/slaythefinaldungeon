using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MonsterActionTarget {
    NoTarget,
    RandomHero,
    Infront,
    LowestHero,
    HighestHero,
    RandomMonster,
}

public class MonsterActionData {

    public string id;
    public MonsterActionTarget targettingMode;
    public List<Active> actives = new List<Active>();

    public MonsterActionData(List<string> data) {
        id = data[0];
        targettingMode = GetAiTargetting(data[1]);
        actives = Active.GetActives(data[2]);
    }

    private MonsterActionTarget GetAiTargetting(string s) {
        switch (s) {
            case "n": return MonsterActionTarget.NoTarget;
            case "i": return MonsterActionTarget.Infront;
            case "r": return MonsterActionTarget.RandomHero;
            case "l": return MonsterActionTarget.LowestHero;
            case "h": return MonsterActionTarget.HighestHero;
            case "m": return MonsterActionTarget.RandomMonster;
            default: Tools.LogError("Unknown Hero Action Target " + s); return MonsterActionTarget.NoTarget;
        }
    }
}


