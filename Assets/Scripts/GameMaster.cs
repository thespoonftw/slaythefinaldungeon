using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    [SerializeField] List<GameObject> heroSpots;
    [SerializeField] List<GameObject> enemySpots;
    [SerializeField] GameObject characterPrefab;

    private List<Character> turnOrder;
    private List<Character> enemies;
    private List<Character> heroes;
    private int turnIndex = 0;
    private float timeBetweenTurns = 1;
    

    public bool IsVictorious => !enemies.Any(e => e.IsAlive);
    public bool IsDefeat => !heroes.Any(h => h.IsAlive);
    public Character CurrentCharacter => turnOrder[turnIndex];

    void Start() {
        gameObject.AddComponent<Data>().LoadData();
        SetupEncounter(1);
        StartNextTurn();
    }

    private void StartNextTurn() {
        if (IsVictorious) { Debug.Log("Victory!"); return; }
        if (IsDefeat) { Debug.Log("Defeat!"); return; }

        turnIndex++;
        if (turnIndex == turnOrder.Count) { turnIndex = 0; }
        if (CurrentCharacter.IsAlive) { StartNextTurn(); return; }

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
        action.source.TakeAction();
        Helper.WaitForTime(timeBetweenTurns);
        StartNextTurn();
    }

    private void SetupEncounter(int encounterId) {
        turnIndex = -1;
        turnOrder = new List<Character>();
        turnOrder.Add(CreateHero(heroSpots[0], Data.heroes[1]));
        turnOrder.Add(CreateHero(heroSpots[1], Data.heroes[2]));
        turnOrder.Add(CreateHero(heroSpots[2], Data.heroes[3]));
        var encounter = Data.encounters[encounterId];
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

}
