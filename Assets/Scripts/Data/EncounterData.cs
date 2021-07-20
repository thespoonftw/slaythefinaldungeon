using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EncounterData {

    public int id;
    public int length;
    public int[,] enemies;

    public EncounterData(List<string> line1, List<string> line2, List<string> line3, List<string> line4) {
        id = Tools.ParseDataInt(line1, 0);
        length = line2.Count;
        var row1 = parseRow(line2);
        var row2 = parseRow(line3);
        var row3 = parseRow(line4);

        enemies = new int[length, 3];
        for (int i=0; i<length; i++) {
            enemies[i, 0] = row1[i];
            enemies[i, 1] = row2[i];
            enemies[i, 2] = row3[i];
        }
    }

    public int[] parseRow(List<string> line) {
        var list = new List<int>();
        foreach (var s in line) {
            if (int.TryParse(s, out int i)) {
                list.Add(i);
            } else {
                list.Add(0);
            }
        }
        return list.ToArray();
    }
}


