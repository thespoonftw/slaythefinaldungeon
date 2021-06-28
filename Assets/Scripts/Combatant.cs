using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Combatant : MonoBehaviour {

    public HeroData heroData;

    public int str;
    public int spriteId;
    public bool isHero;

    public EProperty<int> MaxHp = new EProperty<int>();
    public EProperty<int> CurrentHp = new EProperty<int>();
    public EProperty<int> Animation = new EProperty<int>(); // 1=attack, 2=damaged, 3=dead
    public bool IsAlive => CurrentHp.Value > 0;
    public Equipment LHEquipment => heroData.leftHandEquipmentId != 0 ? Data.equipment[heroData.leftHandEquipmentId] : null;
    public Equipment RHEquipment => heroData.rightHandEquipmentId != 0 ? Data.equipment[heroData.rightHandEquipmentId] : null;

    public void Init(CharacterData data) {
        isHero = (data is HeroData);
        if (data is HeroData) {
            heroData = (HeroData)data;
            CurrentHp.Value = heroData.currentHp;
            MaxHp.Value = data.maxHp + heroData.bonusHp;
        } else {
            CurrentHp.Value = data.maxHp;
            MaxHp.Value = data.maxHp;
        }
        
        str = data.str;
        spriteId = data.sprite;
        GetComponent<CombatantView>().Init();
    }

    public void SaveHeroData() {
        heroData.currentHp = CurrentHp.Value;
    }

    public void TakeDamage(int amount) {
        if (CurrentHp.Value - amount >= MaxHp.Value) {
            CurrentHp.Value = MaxHp.Value;
        } else {
            CurrentHp.Value -= amount;
        }
        Animation.Value = CurrentHp.Value > 0 ? 2 : 3; // play dead if health is below zero
    }
    
}
