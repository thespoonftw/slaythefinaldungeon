using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Encounter {

    public int id;
    public int[] enemies = new int[4];

    public Encounter(List<string> data) {
        id = int.Parse(data[0]);
        enemies[0] = int.Parse(data[1]);
        if (data.Count > 2) { enemies[1] = int.Parse(data[2]); }
        if (data.Count > 3) { enemies[2] = int.Parse(data[3]); }
        if (data.Count > 4) { enemies[3] = int.Parse(data[4]); }
    }
}


