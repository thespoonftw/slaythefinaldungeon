using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer sprite;

    public ActionData action { get; set; }
    public Vector3 originalPosition { get; set; }

    public void SetContent(ActionData action) {
        this.action = action;
        if (action == null) {
            text.text = "";
            text.enabled = false;
            sprite.enabled = false;
            GetComponent<Collider>().enabled = false;
        } else {
            text.text = "[" + action.energyCost + "] " + action.name;
            text.enabled = true;
            sprite.enabled = true;
            GetComponent<Collider>().enabled = true;
        }
    }

    private void OnMouseEnter() {
        CombatUI.Instance.SetTooltip(action.tooltip);
    }

    private void OnMouseExit() {
        CombatUI.Instance.SetTooltip(null);
    }

    private void OnMouseDown() {
        originalPosition = transform.position;
        CombatUI.Instance.TryPickupCard(this);
    }
}
