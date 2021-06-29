using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EquipmentData {

    public int id;  
    public string name;
    public string description;
    public int energyCost;
    public ActionData actionType;
    public List<Passive> passives;

    public EquipmentData(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        description = data[2];
        actionType = ActionData.LoadAction(Helper.ParseDataInt(data, 3), data[4]);
        passives = Passive.LoadPassives(data[5]);
    }
}


