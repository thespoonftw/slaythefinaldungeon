using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer sprite;

    public Character Model { get; set; }

    public void Init() {
        Model = GetComponent<Character>();
        text.text = Model.currentHp + " / " + Model.maxHp;
        sprite.sprite = Data.sprites[Model.spriteId];
    }
    
}
