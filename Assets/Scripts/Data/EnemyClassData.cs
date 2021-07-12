using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyClassData {

    public string id;
    public bool isUndead;
    public int physicalResistance;
    public int fireResistance;
    public int coldResistance;
    public int shockResistance;

    public EnemyClassData(List<string> data) {
        id = data[0];
        isUndead = data[1] == "Y";
        physicalResistance = Tools.ParseDataInt(data, 2);
        fireResistance = Tools.ParseDataInt(data, 2);
        coldResistance = Tools.ParseDataInt(data, 2);
        shockResistance = Tools.ParseDataInt(data, 2);
    }
}


