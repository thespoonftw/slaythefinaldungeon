using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MonsterClassData {

    public string id;
    public bool isUndead;
    public int physicalResistance;
    public int fireResistance;
    public int coldResistance;
    public int shockResistance;

    public MonsterClassData(List<string> data) {
        id = data[0];
        isUndead = data[1] == "Y";
        physicalResistance = Tools.ParseDataInt(data, 2);
        fireResistance = Tools.ParseDataInt(data, 3);
        coldResistance = Tools.ParseDataInt(data, 4);
        shockResistance = Tools.ParseDataInt(data, 5);
    }
}


