using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer sprite;
    
    public void Init(Hero hero) {
        text.text = hero.hp + " / " + hero.hp;
        sprite.sprite = Sprites.dictionary[hero.sprite];
    }

    public void Init(Enemy enemy) {
        text.text = enemy.hp + " / " + enemy.hp;
        sprite.sprite = Sprites.dictionary[enemy.sprite];
    }
    
}
