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
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        sprite = Helper.ParseDataInt(data, 2);
        stats = new Stats(
            Helper.ParseDataInt(data, 3),
            Helper.ParseDataInt(data, 4),
            Helper.ParseDataInt(data, 5)
        );
        startingLHEquipment = Helper.ParseDataInt(data, 6);
        startingRHEquipment = Helper.ParseDataInt(data, 7);
        startingDeck = data[8].Split(' ').Select(int.Parse).ToList();
    }
}
public class EnemyData : CharacterData {

    public ActionData action;

    public EnemyData(List<string> data) {
        id = Helper.ParseDataInt(data, 0);
        name = data[1];
        sprite = Helper.ParseDataInt(data, 2);
        stats = new Stats(
            Helper.ParseDataInt(data, 3),
            Helper.ParseDataInt(data, 4),
            Helper.ParseDataInt(data, 5)
        );
        action = ActionData.LoadAction(data[6]);
    }
}


