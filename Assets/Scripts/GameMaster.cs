using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    [SerializeField] List<GameObject> heroSpots;
    [SerializeField] List<GameObject> enemySpots;
    [SerializeField] GameObject characterPrefab;
    [SerializeField] TextMeshProUGUI centreText;

    private List<Character> turnOrder;
    private List<Character> enemies;
    private List<Character> heroes;
    private int turnIndex = 0;
    private float timeBetweenTurns = 1;
    private int encounterIndex = 1;
    

    public bool IsVictorious => !enemies.Any(e => e.IsAlive);
    public bool IsDefeat => !heroes.Any(h => h.IsAlive);
    public Character CurrentCharacter => turnOrder[turnIndex];

    void Start() {
        gameObject.AddComponent<Data>().LoadData();
        SetupEncounter();
        StartNextTurn();
    }

    private void StartNextTurn() {
        if (IsVictorious) { BattleWon(); return; }
        if (IsDefeat) { GameOver(); return; }

        turnIndex++;
        if (turnIndex == turnOrder.Count) { turnIndex = 0; }
        if (!CurrentCharacter.IsAlive) { StartNextTurn(); return; }

        if (enemies.Contains(CurrentCharacter)) {
            ChooseEnemyAction();
        } else {
            ChooseHeroAction(); // placeholder
        }
    }

    private void ChooseHeroAction() { // currently automated
        var validTargets = enemies.Where(c => c.IsAlive).ToList();
        var action = new Action()
        {
            source = CurrentCharacter,
            damage = CurrentCharacter.str,
            target = Helper.RandomFromList(validTargets)
        };
        PerformAction(action);
    }

    private void ChooseEnemyAction() {
        var validTargets = heroes.Where(c => c.IsAlive).ToList();
        var action = new Action() {
            source = CurrentCharacter,
            damage = CurrentCharacter.str,
            target = Helper.RandomFromList(validTargets)
        };
        PerformAction(action);
    }

    private void PerformAction(Action action) {
        if (action.target != null) {
            action.target.TakeDamage(action.damage);
        }
        action.source.Animation.Value = 1;
        Helper.DelayAction(timeBetweenTurns, StartNextTurn);
    }

    private void SetupEncounter() {
        turnIndex = -1;
        heroes = new List<Character>();
        enemies = new List<Character>();
        turnOrder = new List<Character>();
        turnOrder.Add(CreateHero(heroSpots[0], Data.heroes[1]));
        turnOrder.Add(CreateHero(heroSpots[1], Data.heroes[2]));
        turnOrder.Add(CreateHero(heroSpots[2], Data.heroes[3]));
        var encounter = Data.encounters[encounterIndex];
        for (int i = 0; i < 4; i++) {
            if (encounter.enemies[i] == 0) { continue; }
            turnOrder.Add(CreateEnemy(enemySpots[i], Data.enemies[encounter.enemies[i]]));
        }
    }

    private Character CreateHero(GameObject spawnPoint, Hero data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var character = go.GetComponent<Character>();
        character.Init(data);
        heroes.Add(character);
        return character;
    }

    private Character CreateEnemy(GameObject spawnPoint, Enemy data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var character = go.GetComponent<Character>();
        character.Init(data);
        enemies.Add(character);
        return character;
    }

    private void BattleWon() {
        centreText.text = "Battle Won";
        encounterIndex++;
        if (Data.encounters.ContainsKey(encounterIndex)) {
            Helper.DelayAction(2f, SetupEncounter);
            Helper.DelayAction(2f, () => centreText.text = "");
        } else {
            GameOver();
        }        
    }

    private void GameOver() {
        centreText.text = "Game Over";
    }

}
