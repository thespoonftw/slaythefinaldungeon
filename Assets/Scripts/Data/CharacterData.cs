using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterData {

    public int id;
    public string name;
    public int spriteId;
    public int str;
    public int magic;
    public int maxHp;
    public int speed;
    public int physicalResistance;
    public int fireResistance;
    public int coldResistance;
    public int shockResistance;
    public bool isUndead;

}

public class HeroData : CharacterData {

    public int startingLHEquipment;
    public int startingRHEquipment;
    public List<int> startingDeck;

    public HeroData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        name = data[1];
        spriteId = Tools.ParseDataInt(data, 2);
        maxHp = Tools.ParseDataInt(data, 3);
        str = Tools.ParseDataInt(data, 4);
        magic = Tools.ParseDataInt(data, 5);
        speed = Tools.ParseDataInt(data, 6);
        startingLHEquipment = Tools.ParseDataInt(data, 7);
        startingRHEquipment = Tools.ParseDataInt(data, 8);
        startingDeck = data[9].Split(' ').Select(int.Parse).ToList();
    }
}
public class EnemyData : CharacterData {

    public ActionData action;
    //public string enemyClassData;

    public EnemyData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        name = data[1];
        spriteId = Tools.ParseDataInt(data, 2);
        maxHp = Tools.ParseDataInt(data, 3);
        str = Tools.ParseDataInt(data, 4);
        magic = Tools.ParseDataInt(data, 5);
        speed = Tools.ParseDataInt(data, 6);
        var eClass = Data.enemyClasses[data[7]];
        physicalResistance = eClass.physicalResistance;
        fireResistance = eClass.fireResistance;
        coldResistance = eClass.coldResistance;
        shockResistance = eClass.shockResistance;
        isUndead = eClass.isUndead;
        action = ActionData.LoadAction(0, data[8], "attack", ActionTarget.Target);
    }
}


