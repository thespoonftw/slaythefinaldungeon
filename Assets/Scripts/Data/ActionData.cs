using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ActionTargettingMode {
    noTarget,
    enemy,
    friendly,
    adjacent,
}


public class ActionData {

    public string name;
    public string tooltip;
    public int energyCost;
    public List<Active> actives = new List<Active>();
    public ActionTargettingMode targettingMode;

    public static ActionData LoadAction(int energyCost, string data, string name, string tooltip = "") {
        if (data == null || data == "") { return null; }
        var split = data.Split(' ');
        var returner = new ActionData();
        returner.name = name;
        returner.energyCost = energyCost;
        returner.tooltip = tooltip;
        foreach (var s in split) { returner.actives.Add(new Active(s)); }
        returner.targettingMode = getTargettingMode(returner.actives);
        return returner;
    }

    private static ActionTargettingMode getTargettingMode(List<Active> actives) {
        if (actives.Any(a => a.targettingType == TargettingType.Enemy)) { return ActionTargettingMode.enemy; }
        if (actives.Any(a => a.targettingType == TargettingType.Friendly)) { return ActionTargettingMode.friendly; }
        if (actives.Any(a => a.targettingType == TargettingType.Adjacent)) { return ActionTargettingMode.adjacent; }
        return ActionTargettingMode.noTarget;
    }
}

