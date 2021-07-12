using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameMaster : Singleton<GameMaster> {

    [SerializeField] TextMeshProUGUI centreText;

    public List<Hero> heroes = new List<Hero>();
    private int encounterIndex = 1;

    void Start() {
        var inputs = Inputs.Instance; // to initialise the inputs system
        gameObject.AddComponent<Data>().LoadData();

        foreach (var data in Data.heroes.Values) {
            if (data == null) { continue; }
            var hero = new Hero(data);
            heroes.Add(hero);
            if (hero.lhEquipment != null) { hero.lhEquipment.passives.ForEach(p => EquipmentStats(hero, p)); }
            if (hero.rhEquipment != null) { hero.rhEquipment.passives.ForEach(p => EquipmentStats(hero, p)); }
        }
        CombatMaster.Instance.Setup(encounterIndex);
    }

    public void EquipmentStats(Hero hero, Passive passive) {
        if (passive.type == PassiveType.hp) {
            hero.maxHp += passive.amount;
            hero.currentHp += passive.amount;
        } else if (passive.type == PassiveType.energy) {
            hero.maxEnergy += passive.amount;
        }
    }

    public void BattleWon() {
        CombatUI.Instance.DisableUI();
        centreText.text = "Battle Won";
        encounterIndex++;
        if (Data.encounters.ContainsKey(encounterIndex)) {

            Tools.DelayMethod(2f, () => CombatMaster.Instance.Setup(encounterIndex));
            Tools.DelayMethod(2f, () => centreText.text = "");
        } else {
            GameOver();
        }        
    }

    public void GameOver() {
        CombatUI.Instance.DisableUI();
        centreText.text = "Game Over";
    }

}
