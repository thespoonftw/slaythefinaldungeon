using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffView : MonoBehaviour {

    [SerializeField] SpriteRenderer sprite;

    private Buff buff;

    public void Init(Buff buff) {
        this.buff = buff;
        sprite.sprite = Data.sprites[buff.data.sprite];
    }

    private void OnMouseEnter() {
        CombatUI.Instance.SetTooltip(buff.data.description);
    }

    private void OnMouseExit() {
        CombatUI.Instance.SetTooltip(null);
    }
}
