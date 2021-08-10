using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PassiveType {
    hp,
    energy,
    strength,
    magic,
    speed,
    cunning,
    physical,
    fire,
    cold,
    shock,
}

public class Passive {

    public PassiveType type;
    public int amount;

    public Passive(string data) {
        var split = data.Split('(');
        var parameters = split[1].Split(' ');
        type = (PassiveType)Enum.Parse(typeof(PassiveType), split[0]);
        amount = int.Parse(parameters[0]);
    }

    public static List<Passive> LoadPassives(string data) {
        var returner = new List<Passive>();
        if (data == null || data == "") { return returner; }
        string[] splitStrings = { ") ", ")" };
        var split = data.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in split) { returner.Add(new Passive(s)); }
        return returner;
    }

}


