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
        CombatController.Instance.Setup(encounterIndex);
    }

    public void EquipmentStats(Hero hero, Passive passive) {
        switch (passive.type) {
            case PassiveType.hp:
                hero.maxHp += passive.amount;
                hero.currentHp += passive.amount;
                break;
            case PassiveType.energy:
                hero.maxEnergy += passive.amount;
                break;
            case PassiveType.strength:
                hero.str += passive.amount;
                break;
            case PassiveType.magic:
                hero.magic += passive.amount;
                break;
            case PassiveType.speed:
                hero.speed += passive.amount;
                break;
            case PassiveType.cunning:
                hero.cunning += passive.amount;
                break;
            case PassiveType.physical:
                hero.physicalResistance += passive.amount;
                break;
            case PassiveType.fire:
                hero.fireResistance += passive.amount;
                break;
            case PassiveType.cold:
                hero.coldResistance += passive.amount;
                break;
            case PassiveType.shock:
                hero.shockResistance += passive.amount;
                break; 
        }
    }

    public void GameOver() {
        CombatUIController.Instance.EndHeroTurn();
        SetCentreText("Game Over");
    }

    public void SetCentreText(string text) {
        centreText.text = text;
    }

}
