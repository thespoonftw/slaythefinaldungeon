using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    public static Dictionary<int, HeroData> heroes;
    public static Dictionary<int, EnemyData> enemies;
    public static Dictionary<int, Encounter> encounters;
    public static Dictionary<int, Sprite> sprites;
    public static Dictionary<int, Equipment> equipment;
    public static Dictionary<BuffType, Buff> buffs;
    public static Dictionary<int, Card> cards;

    public void LoadData() {
        heroes = CsvLoader.LoadAsType<HeroData>("Heroes");
        enemies = CsvLoader.LoadAsType<EnemyData>("Enemies");
        encounters = CsvLoader.LoadAsType<Encounter>("Encounters");
        equipment = CsvLoader.LoadAsType<Equipment>("Equipment");
        buffs = Buff.Load("Buffs");
        cards = CsvLoader.LoadAsType<Card>("Cards");
        sprites = gameObject.AddComponent<SpriteLoader>().Load();
    }
}
