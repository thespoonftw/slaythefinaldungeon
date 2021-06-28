using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActionType {

    public List<Active> actives = new List<Active>();
    public bool RequiresEnemyTarget => actives.Any(a => a.targettingType == TargettingType.Enemy);
    public bool RequiresFriendlyTarget => actives.Any(a => a.targettingType == TargettingType.Friendly);

    public static ActionType LoadAction(string data) {
        if (data == null || data == "") { return null; }
        var split = data.Split(' ');
        var returner = new ActionType();
        foreach (var s in split) { returner.actives.Add(new Active(s)); }
        return returner;
    }

    public Action CreateAction(Combatant source, Combatant target = null) {
        var returner = new Action();
        returner.actives = actives;
        returner.source = source;
        returner.target = target;
        return returner;
    }
}

public class Action : ActionType {

    public Combatant source;
    public Combatant target;

}


