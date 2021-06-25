using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterData {

    public int id;
    public string name;
    public int sprite;
    public int maxHp;
    public int str;
}

public class EnemyData : CharacterData {

    public EnemyData(List<string> data) {
        id = int.Parse(data[0]);
        name = data[1];
        sprite = int.Parse(data[2]);
        maxHp = int.Parse(data[3]);
        str = int.Parse(data[4]);
    }
}

public class HeroData : CharacterData {

    public int currentHp;

    public HeroData(List<string> data) {
        id = int.Parse(data[0]);
        name = data[1];
        sprite = int.Parse(data[2]);
        currentHp = int.Parse(data[3]);
        maxHp = int.Parse(data[3]);
        str = int.Parse(data[4]);
    }
}


