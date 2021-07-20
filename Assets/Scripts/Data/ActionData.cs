using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ActionTarget {
    NoTarget,
    Melee,
    Touch,
    Ranged,
    Any,
    Enemy,
    OtherAlly,
    Friendly,
}

public class ActionData {

    public string name;
    public string tooltip;
    public int energyCost;
    public List<Active> actives = new List<Active>();
    public ActionTarget targettingMode;    

    public static ActionData LoadAction(int energyCost, string data, string name, ActionTarget targettingMode, string tooltip = "") {
        if (data == null || data == "") { return null; }
        var returner = new ActionData();
        returner.name = name;
        returner.energyCost = energyCost;
        returner.tooltip = tooltip;
        string[] splitStrings = { ") ", ")" };
        var split = data.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in split) { returner.actives.Add(new Active(s)); }
        returner.targettingMode = targettingMode;
        return returner;
    }

    public static ActionTarget GetTargettingMode(string s) {
        switch (s) {
            case "m": return ActionTarget.Melee;
            case "t": return ActionTarget.Touch;
            case "r": return ActionTarget.Ranged;
            case "e": return ActionTarget.Enemy;
            case "o": return ActionTarget.OtherAlly;
            case "f": return ActionTarget.Friendly;
            case "a": return ActionTarget.Any;
            default: return ActionTarget.NoTarget;
        }
    }
}

