using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum BuffType {
    str,
    wea,
    pro,
    vul,
}

public class Buff {

    public BuffType type;
    public string name;
    public string description;

    public Buff(List<string> data) {
        type = (BuffType)Enum.Parse(typeof(BuffType), data[0]);
        name = data[1];
        description = data[2];
    }

    public static Dictionary<BuffType, Buff> Load(string filename) {
        var returner = new Dictionary<BuffType, Buff>();
        var data = CsvLoader.LoadFile(filename);
        foreach (var h in data) {
            var value = new Buff(h);
            BuffType key = (BuffType)Enum.Parse(typeof(BuffType), h[0]);
            returner.Add(key, value);
        }
        return returner;
    }
}


