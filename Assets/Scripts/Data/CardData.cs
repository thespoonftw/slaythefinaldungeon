using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardData {

    public int id;
    public string name;
    public string description;
    public ActionData active;

    public CardData(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        description = data[2];
        active = ActionData.LoadAction(Helper.ParseDataInt(data, 3), data[4]);
    }
}


