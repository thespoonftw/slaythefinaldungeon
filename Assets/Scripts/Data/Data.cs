using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    public static Dictionary<int, HeroData> heroes;
    public static Dictionary<int, MonsterData> monsters;
    public static Dictionary<int, EncounterData> encounters;
    public static Dictionary<int, Sprite> sprites;
    public static Dictionary<int, EquipmentData> equipment;
    public static Dictionary<BuffType, BuffData> buffs;
    public static Dictionary<int, CardData> cards;
    public static Dictionary<string, MonsterClassData> monsterClasses;
    public static Dictionary<string, MonsterActionData> monsterActions;

    public void LoadData() {
        heroes = CsvLoader.LoadInt<HeroData>("Heroes");
        monsterClasses = CsvLoader.LoadString<MonsterClassData>("MonsterClass"); // must load before monsters
        monsterActions = CsvLoader.LoadString<MonsterActionData>("MonsterActions"); // must load before monsters
        monsters = CsvLoader.LoadInt<MonsterData>("Monsters");
        encounters = CsvLoader.LoadEncounters("Encounters");
        equipment = CsvLoader.LoadInt<EquipmentData>("Equipment");
        buffs = CsvLoader.LoadBuffs("Buffs");
        cards = CsvLoader.LoadInt<CardData>("Cards");        
        sprites = gameObject.AddComponent<SpriteLoader>().Load();        
        Debug.Log("Streaming Assets fully loaded");
    }
}
