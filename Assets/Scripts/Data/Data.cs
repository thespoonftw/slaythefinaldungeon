using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    public static Dictionary<int, HeroData> heroes;
    public static Dictionary<int, EnemyData> enemies;
    public static Dictionary<int, Encounter> encounters;
    public static Dictionary<int, Sprite> sprites;

    public void LoadData() {
        heroes = CsvLoader.LoadAsType<HeroData>("Heroes");
        enemies = CsvLoader.LoadAsType<EnemyData>("Enemies");
        encounters = CsvLoader.LoadAsType<Encounter>("Encounters");
        sprites = gameObject.AddComponent<SpriteLoader>().Load();
    }
}
