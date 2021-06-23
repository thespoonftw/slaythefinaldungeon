using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    public static Dictionary<int, Hero> heroes;
    public static Dictionary<int, Enemy> enemies;
    public static Dictionary<int, Encounter> encounters;
    public static Dictionary<int, Sprite> sprites;

    public void LoadData() {
        heroes = CsvLoader.LoadAsType<Hero>("Heroes");
        enemies = CsvLoader.LoadAsType<Enemy>("Enemies");
        encounters = CsvLoader.LoadAsType<Encounter>("Encounters");
        sprites = gameObject.AddComponent<SpriteLoader>().Load();
    }
}
