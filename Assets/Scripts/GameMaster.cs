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
    private List<HeroData> activeHeroes;
    private int turnIndex = 0;
    private int encounterIndex = 1;
    
    public bool IsVictorious => LivingEnemies.Count == 0;
    public bool IsDefeat => LivingHeroes.Count == 0;
    public Combatant CurrentCharacter => turnOrder[turnIndex];
    public List<Combatant> LivingHeroes => turnOrder.Where(e => e.IsAlive && e.isHero).ToList();
    public List<Combatant> LivingEnemies => turnOrder.Where(e => e.IsAlive && !e.isHero).ToList();
    public List<Combatant> Heroes => turnOrder.Where(e => e.isHero).ToList();

    void Start() {
        var inputs = Inputs.Instance; // to initialise the inputs system
        gameObject.AddComponent<Data>().LoadData();
        activeHeroes = new List<HeroData>() { Data.heroes[1], Data.heroes[2], Data.heroes[3] };
        SetupEncounter();
    }

    private void SetupEncounter() {
        turnIndex = -1;
        turnOrder = new List<Combatant>();
        turnOrder.Add(CreateHero(heroSpots[0], activeHeroes[0]));
        turnOrder.Add(CreateHero(heroSpots[1], activeHeroes[1]));
        turnOrder.Add(CreateHero(heroSpots[2], activeHeroes[2]));
        var encounter = Data.encounters[encounterIndex];
        for (int i = 0; i < 4; i++)
        {
            if (encounter.enemies[i] == 0) { continue; }
            turnOrder.Add(CreateEnemy(enemySpots[i], Data.enemies[encounter.enemies[i]]));
        }
        Helper.DelayMethod(1f, StartNextTurn);
    }

    private void StartNextTurn() {
        if (IsVictorious) { BattleWon(); return; }
        if (IsDefeat) { GameOver(); return; }

        turnIndex++;
        if (turnIndex == turnOrder.Count) { turnIndex = 0; }
        if (!CurrentCharacter.IsAlive) { StartNextTurn(); return; }

        if (!CurrentCharacter.isHero) {
            ChooseEnemyAction();
        } else {
            actionUI.StartActionChoice();
        }
    }

    private void ChooseEnemyAction() {
        var action = new Action() {
            source = CurrentCharacter,
            damage = CurrentCharacter.str,
            target = Helper.RandomFromList(LivingHeroes)
        };
        PerformAction(action);
    }

    public void PerformAction(Action action) {
        if (action.target != null) {
            action.target.TakeDamage(action.damage);
        }
        action.source.Animation.Value = 1;
        Helper.DelayMethod(1f, StartNextTurn);
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
