using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum AiTargettingMode {
    none,
    random,
    infront,
}

public class EnemyActionData {

    public string id;
    public AiTargettingMode aiTargetting;
    public ActionData action;

    public EnemyActionData(List<string> data) {
        id = data[0];
        aiTargetting = GetAiTargetting(data[1]);
        var targettingMode = ActionData.GetTargettingMode(data[2]);
        var actives = data[3];
        action = ActionData.LoadAction(0, actives, id, targettingMode, "");
    }

    private AiTargettingMode GetAiTargetting(string code) {
        switch (code) {
            default: return AiTargettingMode.none;
            case "i": return AiTargettingMode.infront;
            case "r": return AiTargettingMode.random;
        }
    }
}


