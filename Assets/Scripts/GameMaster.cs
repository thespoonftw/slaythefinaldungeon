using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameMaster : Singleton<GameMaster> {

    [SerializeField] List<GameObject> heroSpots;
    [SerializeField] List<GameObject> enemySpots;
    [SerializeField] GameObject characterPrefab;
    [SerializeField] TextMeshProUGUI centreText;
    [SerializeField] ActionUI actionUI;

    private List<Combatant> turnOrder;
    private int turnIndex = 0;
    private int encounterIndex = 1;
    
    public bool IsVictorious => LivingEnemies.Count == 0;
    public bool IsDefeat => LivingHeroes.Count == 0;
    public Combatant CurrentCombatant => turnOrder[turnIndex];
    public List<Combatant> LivingHeroes => turnOrder.Where(e => e.IsAlive && e.isHero).ToList();
    public List<Combatant> LivingEnemies => turnOrder.Where(e => e.IsAlive && !e.isHero).ToList();
    public List<Combatant> Heroes => turnOrder.Where(e => e.isHero).ToList();

    void Start() {
        var inputs = Inputs.Instance; // to initialise the inputs system
        gameObject.AddComponent<Data>().LoadData();
        foreach (var h in Data.heroes.Values) {
            if (h.leftHandEquipmentId != 0) { Data.equipment[h.leftHandEquipmentId].passives.ForEach(p => ApplyPassive(h, p)); }
            if (h.rightHandEquipmentId != 0) { Data.equipment[h.rightHandEquipmentId].passives.ForEach(p => ApplyPassive(h, p)); }
        }
        SetupEncounter();
    }

    private void SetupEncounter() {
        turnIndex = -1;
        turnOrder = new List<Combatant>();
        turnOrder.Add(CreateHero(heroSpots[0], Data.heroes[1]));
        turnOrder.Add(CreateHero(heroSpots[1], Data.heroes[2]));
        turnOrder.Add(CreateHero(heroSpots[2], Data.heroes[3]));
        var encounter = Data.encounters[encounterIndex];
        for (int i = 0; i < 4; i++) {
            if (encounter.enemies[i] == 0) { continue; }
            turnOrder.Add(CreateEnemy(enemySpots[i], Data.enemies[encounter.enemies[i]]));
        }
        Helper.DelayMethod(1f, StartNextTurn);
    }

    private void StartNextTurn() {
        if (IsVictorious) { Helper.DelayMethod(1f, BattleWon); return; }
        if (IsDefeat) { GameOver(); return; }

        turnIndex++;
        if (turnIndex == turnOrder.Count) { turnIndex = 0; }
        if (!CurrentCombatant.IsAlive) { StartNextTurn(); return; }

        if (!CurrentCombatant.isHero) {
            PerformAction(Action.BasicAttack(2, CurrentCombatant, Helper.RandomFromList(LivingHeroes)));
        } else {
            actionUI.StartActionChoice();
        }
    }

    public void PerformAction(Action action) {
        foreach (var a in action.actives) {
            //evaluate targets
            var targets = new List<Combatant>();
            if (a.targettingType == TargettingType.enemy || a.targettingType == TargettingType.ally) {
                targets.Add(action.target);
            } else if ((a.targettingType == TargettingType.allAllies && CurrentCombatant.isHero) || (a.targettingType == TargettingType.allEnemies && !CurrentCombatant.isHero)) {
                targets.AddRange(LivingHeroes);
            } else if ((a.targettingType == TargettingType.allAllies && !CurrentCombatant.isHero) || (a.targettingType == TargettingType.allEnemies && CurrentCombatant.isHero)) {
                targets.AddRange(LivingEnemies);
            }
            foreach (var t in targets) {
                // do the active
                if (a.type == ActiveType.attack) {
                    t.TakeDamage(a.amount * CurrentCombatant.str);
                } else if (a.type == ActiveType.heal) {
                    t.TakeDamage(-a.amount * CurrentCombatant.str);
                }
            }
            action.source.Animation.Value = 1;
            Helper.DelayMethod(1f, StartNextTurn);
        }
    }

    public void ApplyPassive(HeroData heroData, Passive passive) {
        if (passive.type == PassiveType.hp) {
            heroData.bonusHp += passive.amount;
            heroData.currentHp += passive.amount;
        }
    }

    private Combatant CreateHero(GameObject spawnPoint, HeroData data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var character = go.GetComponent<Combatant>();
        character.Init(data);
        return character;
    }

    private Combatant CreateEnemy(GameObject spawnPoint, EnemyData data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var character = go.GetComponent<Combatant>();
        character.Init(data);
        return character;
    }

    private void BattleWon() {
        centreText.text = "Battle Won";
        encounterIndex++;
        if (Data.encounters.ContainsKey(encounterIndex)) {
            foreach (var h in Heroes) { h.SaveHeroData(); }
            foreach (var c in turnOrder) { Destroy(c.gameObject); }
            Helper.DelayMethod(2f, SetupEncounter);
            Helper.DelayMethod(2f, () => centreText.text = "");
        } else {
            GameOver();
        }        
    }

    private void GameOver() {
        centreText.text = "Game Over";
    }

}
