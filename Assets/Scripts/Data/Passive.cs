using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PassiveType {
    hp,
    energy
}

public class Passive {

    public PassiveType type;
    public int amount;

    public Passive(string data) {
        var split = data.Split('(');
        type = (PassiveType)Enum.Parse(typeof(PassiveType), split[0]);
        amount = int.Parse(split[1].Substring(0, split[1].Length - 1));
    }

    public static List<Passive> LoadPassives(string data) {
        var returner = new List<Passive>();
        if (data == null || data == "") { return returner; }
        var split = data.Split(';');
        foreach (var s in split) { returner.Add(new Passive(s)); }
        return returner;
    }

}


