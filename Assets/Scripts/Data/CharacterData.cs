using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterData {

    public int id;
    public string name;
    public int sprite;
    public Stats stats;

}

public class HeroData : CharacterData {

    public int startingLHEquipment;
    public int startingRHEquipment;
    public List<int> startingDeck;

    public HeroData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        name = data[1];
        sprite = Tools.ParseDataInt(data, 2);
        stats = new Stats(
            Tools.ParseDataInt(data, 3),
            Tools.ParseDataInt(data, 4),
            Tools.ParseDataInt(data, 5)
        );
        startingLHEquipment = Tools.ParseDataInt(data, 6);
        startingRHEquipment = Tools.ParseDataInt(data, 7);
        startingDeck = data[8].Split(' ').Select(int.Parse).ToList();
    }
}
public class EnemyData : CharacterData {

    public ActionData action;

    public EnemyData(List<string> data) {
        id = Tools.ParseDataInt(data, 0);
        name = data[1];
        sprite = Tools.ParseDataInt(data, 2);
        stats = new Stats(
            Tools.ParseDataInt(data, 3),
            Tools.ParseDataInt(data, 4),
            Tools.ParseDataInt(data, 5)
        );
        action = ActionData.LoadAction(0, data[6], "attack");
    }
}


