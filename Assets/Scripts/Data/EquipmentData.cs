using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EquipmentData {

    public int id;  
    public string name;
    public string description;
    public HeroActionData action;
    public List<Passive> passives;

    public EquipmentData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        name = data[1];
        description = data[2];
        var cost = Tools.ParseDataInt(data, 3);
        var targetting = HeroActionData.GetTargettingMode(data[4]);
        var actives = data[5];
        action = HeroActionData.LoadAction(cost, actives, name, targetting, description);
        passives = Passive.LoadPassives(data[6]);
    }
}


