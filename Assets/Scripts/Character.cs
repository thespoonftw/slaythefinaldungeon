using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour {

    public CharacterView View { get; set; }

    public int str;
    public int spriteId;
    public bool isHero;

    public EProperty<int> MaxHp = new EProperty<int>();
    public EProperty<int> CurrentHp = new EProperty<int>();
    public EProperty<int> Animation = new EProperty<int>(); // 1=attack, 2=damaged, 3=dead
    public bool IsAlive => CurrentHp.Value > 0;
    
    public void Init(CharacterData data) {
        isHero = (data is Hero);
        MaxHp.Value = data.hp;
        CurrentHp.Value = data.hp;        
        str = data.str;
        spriteId = data.sprite;
        View = GetComponent<CharacterView>();
        View.Init();
    }

    public void TakeDamage(int amount) {
        CurrentHp.Value -= amount;
        Animation.Value = CurrentHp.Value > 0 ? 2 : 3;
    }
    
}
