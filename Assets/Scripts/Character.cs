using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour {

    public CharacterView View { get; set; }

    public int str;
    public int maxHp;
    public int currentHp;
    public int spriteId;

    public bool IsAlive => currentHp > 0;
    
    public void Init(CharacterData data) {        
        maxHp = data.hp;
        currentHp = data.hp;        
        str = data.str;
        View = GetComponent<CharacterView>();
        View.Init();
    }

    public void TakeDamage(int amount) {
        currentHp -= amount;
    }

    public void TakeAction() {
        // make the sprite wiggle or something
    }

    
}
