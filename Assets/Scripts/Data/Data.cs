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

    public void LoadData() {
        heroes = CsvLoader.LoadAsType<HeroData>("Heroes");
        enemies = CsvLoader.LoadAsType<EnemyData>("Enemies");
        encounters = CsvLoader.LoadAsType<EncounterData>("Encounters");
        equipment = CsvLoader.LoadAsType<EquipmentData>("Equipment");
        buffs = BuffData.Load("Buffs");
        cards = CsvLoader.LoadAsType<CardData>("Cards");
        sprites = gameObject.AddComponent<SpriteLoader>().Load();
    }
}
