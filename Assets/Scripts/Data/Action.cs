using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActionType {

    public List<Active> actives = new List<Active>();
    public bool RequiresEnemyTarget => actives.Any(a => a.targettingType == TargettingType.enemy);
    public bool RequiresAllyTarget => actives.Any(a => a.targettingType == TargettingType.ally);

    public static ActionType LoadAction(string data) {
        if (data == null || data == "") { return null; }
        var split = data.Split(';');
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

    public static Action BasicAttack(int power, Combatant source, Combatant target) {
        var returner = new Action();
        returner.source = source;
        returner.target = target;
        returner.actives.Add(new Active(ActiveType.attack, TargettingType.enemy, power));
        return returner;
    }



}


