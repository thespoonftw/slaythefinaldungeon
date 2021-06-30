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

    private List<Hero> heroes = new List<Hero>();
    private List<Combatant> turnOrder;
    private int turnIndex = 0;
    private int encounterIndex = 1;
    
    public bool IsVictorious => LivingEnemies.Count == 0;
    public bool IsDefeat => LivingHeroes.Count == 0;
    public Combatant CurrentCombatant => turnOrder[turnIndex];
    public List<Combatant> LivingHeroes => turnOrder.Where(e => e.IsAlive && e.IsHero).ToList();
    public List<Combatant> LivingEnemies => turnOrder.Where(e => e.IsAlive && !e.IsHero).ToList();
    public List<Combatant> CombatantHeroes => turnOrder.Where(e => e.IsHero).ToList();

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
        SetupEncounter();
    }

    private void SetupEncounter() {
        turnIndex = -1;
        turnOrder = new List<Combatant>();
        turnOrder.Add(CreateHero(heroSpots[0], heroes[0]));
        turnOrder.Add(CreateHero(heroSpots[1], heroes[1]));
        turnOrder.Add(CreateHero(heroSpots[2], heroes[2]));
        var encounter = Data.encounters[encounterIndex];
        for (int i = 0; i < 4; i++) {
            if (encounter.enemies[i] == 0) { continue; }
            turnOrder.Add(CreateEnemy(enemySpots[i], Data.enemies[encounter.enemies[i]]));
        }
        Helper.DelayMethod(1f, StartTurn);
    }

    private void StartTurn() {     
        turnIndex++;
        if (turnIndex == turnOrder.Count) { turnIndex = 0; }
        if (!CurrentCombatant.IsAlive) { StartTurn(); return; }

        CurrentCombatant.StartOfTurnBuffs();
        if (!CurrentCombatant.IsHero) {
            PerformAction(CurrentCombatant.enemyData.action, CurrentCombatant, Helper.RandomFromList(LivingHeroes));
        } else {
            actionUI.StartActionChoice();
        }
    }

    private void EndTurn() {
        if (IsVictorious) { Helper.DelayMethod(1f, BattleWon); return; }
        if (IsDefeat) { GameOver(); return; }
        CurrentCombatant.EndOfTurnBuffs();
        StartTurn();
    }

    public void PerformAction(ActionData data, Combatant source, Combatant target = null) {
        foreach (var a in data.actives) {
            //evaluate targets
            var targets = new List<Combatant>();
            if (a.targettingType == TargettingType.Enemy || a.targettingType == TargettingType.Friendly || a.targettingType == TargettingType.Adjacent) {
                targets.Add(target);
            } else if ((a.targettingType == TargettingType.AllFriendly && CurrentCombatant.IsHero) || (a.targettingType == TargettingType.AllEnemies && !CurrentCombatant.IsHero)) {
                targets.AddRange(LivingHeroes);
            } else if ((a.targettingType == TargettingType.AllFriendly && !CurrentCombatant.IsHero) || (a.targettingType == TargettingType.AllEnemies && CurrentCombatant.IsHero)) {
                targets.AddRange(LivingEnemies);
            }
            foreach (var t in targets) {
                // do the active
                if (a.type == ActiveType.dmg) {
                    t.TakeDamage(Mathf.RoundToInt(a.amount * (CurrentCombatant.str * Mathf.Sqrt(CurrentCombatant.CombatLevel) / t.resist)));
                } else if (a.type == ActiveType.heal) {
                    t.TakeDamage(-a.amount);
                } else if (a.type == ActiveType.buff) {
                    t.ApplyBuff(new Buff(source, target, a.amount, Data.buffs[a.buff]));
                }
            }
            source.Animation.Value = 1;
            Helper.DelayMethod(1f, EndTurn);
        }
    }

    public void EquipmentStats(Hero hero, Passive passive) {
        if (passive.type == PassiveType.hp) {
            hero.stats.maxHp += passive.amount;
            hero.currentHp += passive.amount;
        }
    }

    private Combatant CreateHero(GameObject spawnPoint, Hero data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var view = go.GetComponent<CombatantView>();
        return new Combatant(view, data);
    }

    private Combatant CreateEnemy(GameObject spawnPoint, EnemyData data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var view = go.GetComponent<CombatantView>();
        return new Combatant(view, data);
    }

    private void BattleWon() {
        centreText.text = "Battle Won";
        encounterIndex++;
        if (Data.encounters.ContainsKey(encounterIndex)) {
            foreach (var h in CombatantHeroes) { h.SaveHero(); }
            foreach (var c in turnOrder) { Destroy(c.GameObject); }
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
