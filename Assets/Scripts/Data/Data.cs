using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    public static Dictionary<int, HeroData> heroes;
    public static Dictionary<int, EnemyData> enemies;
    public static Dictionary<int, EncounterData> encounters;
    public static Dictionary<int, Sprite> sprites;
    public static Dictionary<int, EquipmentData> equipment;
    public static Dictionary<BuffType, BuffData> buffs;
    public static Dictionary<int, CardData> cards;
    public static Dictionary<string, EnemyClassData> enemyClasses;

    public void LoadData() {
        heroes = CsvLoader.LoadInt<HeroData>("Heroes");
        enemyClasses = CsvLoader.LoadString<EnemyClassData>("EnemyClass"); // must load before enemies
        enemies = CsvLoader.LoadInt<EnemyData>("Enemies");
        encounters = CsvLoader.LoadInt<EncounterData>("Encounters");
        equipment = CsvLoader.LoadInt<EquipmentData>("Equipment");
        buffs = CsvLoader.LoadBuffs("Buffs");
        cards = CsvLoader.LoadInt<CardData>("Cards");        
        sprites = gameObject.AddComponent<SpriteLoader>().Load();        
        Debug.Log("Streaming Assets fully loaded");
    }
}
