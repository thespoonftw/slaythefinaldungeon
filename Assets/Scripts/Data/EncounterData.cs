using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EncounterData {

    public int id;
    public int[] enemies = new int[4];

    public EncounterData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        enemies[0] = Tools.ParseDataInt(data, 1);
        enemies[1] = Tools.ParseDataInt(data, 2);
        enemies[2] = Tools.ParseDataInt(data, 3);
        enemies[3] = Tools.ParseDataInt(data, 4);
    }
}


