using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActionData {

    public List<Active> actives = new List<Active>();
    public bool RequiresEnemyTarget => actives.Any(a => a.targettingType == TargettingType.Enemy);
    public bool RequiresFriendlyTarget => actives.Any(a => a.targettingType == TargettingType.Friendly);

    public static ActionData LoadAction(string data) {
        if (data == null || data == "") { return null; }
        var split = data.Split(' ');
        var returner = new ActionData();
        foreach (var s in split) { returner.actives.Add(new Active(s)); }
        return returner;
    }
}

