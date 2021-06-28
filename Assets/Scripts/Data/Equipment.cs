using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Equipment {

    public int id;
    public string name;
    public string description;
    public ActionType actionType;
    public List<Passive> passives;

    public Equipment(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        description = data[2];
        actionType = ActionType.LoadAction(data[3]);
        passives = Passive.LoadPassives(data[4]);
    }
}


