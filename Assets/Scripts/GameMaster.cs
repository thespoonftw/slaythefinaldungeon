using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    [SerializeField] List<GameObject> heroSpots;
    [SerializeField] List<GameObject> enemySpots;
    [SerializeField] GameObject characterPrefab;



    void Start() {

        var heroes = CsvLoader.LoadAsType<Hero>("Heroes");
        var enemies = CsvLoader.LoadAsType<Enemy>("Enemies");
        var encounters = CsvLoader.LoadAsType<Encounter>("Encounters");
        Sprites.Init();

        CreateHero(heroSpots[0], heroes[1]);
        CreateHero(heroSpots[1], heroes[2]);
        CreateHero(heroSpots[2], heroes[3]);

        var encounter = encounters[1];
        for (int i=0; i<4; i++) {
            if (encounter.enemies[i] == 0) { continue; }
            CreateEnemy(enemySpots[i], enemies[encounter.enemies[i]]);
        }
    }

    private void CreateHero(GameObject spawnPoint, Hero data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        go.GetComponent<Character>().Init(data);
    }

    private void CreateEnemy(GameObject spawnPoint, Enemy data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        go.GetComponent<Character>().Init(data);
    }

}
