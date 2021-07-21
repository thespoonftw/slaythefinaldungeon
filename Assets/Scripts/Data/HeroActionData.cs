using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum HeroActionTarget {
    NoTarget,
    Range1,
    Range2,
    Range3,
}

public class HeroActionData {

    public string name;
    public string tooltip;
    public int energyCost;
    public List<Active> actives;
    public HeroActionTarget targettingMode;

    public HeroActionData(int energyCost, string data, string name, HeroActionTarget targettingMode, string tooltip = "") {
        this.name = name;
        this.energyCost = energyCost;
        this.tooltip = tooltip;
        actives = Active.GetActives(data);
        this.targettingMode = targettingMode;
    }

    // nullable constructor
    public static HeroActionData LoadAction(int energyCost, string data, string name, HeroActionTarget targettingMode, string tooltip = "") {
        if (data == null || data == "") { return null; }
        return new HeroActionData(energyCost, data, name, targettingMode, tooltip);
    }

    public static HeroActionTarget GetTargettingMode(string s) {
        switch (s) {
            case "": return HeroActionTarget.NoTarget;
            case "0": return HeroActionTarget.NoTarget;
            case "1": return HeroActionTarget.Range1;
            case "2": return HeroActionTarget.Range2;
            case "3": return HeroActionTarget.Range3;
            default: Tools.LogError("Unknown Hero Action Target " + s); return HeroActionTarget.NoTarget;
        }
    }
}

