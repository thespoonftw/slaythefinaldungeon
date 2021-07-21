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
    emp,
    dam,
    has,
    slo,
    tau,
}

public class BuffData {

    public BuffType type;
    public string name;
    public string description;
    public int sprite;
    public bool isStartOfTurn;

    public BuffData(List<string> data) {
        type = (BuffType)Enum.Parse(typeof(BuffType), data[0]);
        name = data[1];
        description = data[2];
        sprite = Tools.ParseDataInt(data, 3);
        isStartOfTurn = data[4] == "start";
    }
}


