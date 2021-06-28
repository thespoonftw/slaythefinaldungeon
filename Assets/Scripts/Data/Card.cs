using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Card {

    public int id;
    public string name;
    public string description;
    public int cost;
    public ActionType active;

    public Card(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        description = data[2];
        cost = Helper.ParseDataInt(data, 3);
        active = ActionType.LoadAction(data[4]);
    }
}


