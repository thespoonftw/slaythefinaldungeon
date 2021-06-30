using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffView : MonoBehaviour {

    [SerializeField] SpriteRenderer sprite;

    public void SetSprite(int id) {
        sprite.sprite = Data.sprites[id];
    }
}
