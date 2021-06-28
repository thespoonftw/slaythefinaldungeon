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

public class HeroData : CharacterData {

    public int currentHp;
    public int bonusHp;
    public int leftHandEquipmentId;
    public int rightHandEquipmentId;

    public HeroData(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        sprite = Helper.ParseDataInt(data, 2);
        currentHp = Helper.ParseDataInt(data, 3);
        maxHp = Helper.ParseDataInt(data, 3);
        str = Helper.ParseDataInt(data, 4);
        leftHandEquipmentId = Helper.ParseDataInt(data, 5);
        rightHandEquipmentId = Helper.ParseDataInt(data, 6);
    }
}
public class EnemyData : CharacterData {

    public EnemyData(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        sprite = Helper.ParseDataInt(data, 2);
        maxHp = Helper.ParseDataInt(data, 3);
        str = Helper.ParseDataInt(data, 4);
    }
}


