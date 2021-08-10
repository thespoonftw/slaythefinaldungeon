using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer sprite;

    private CombatantHero hero;
    private HandController cardsView;

    public HeroActionData action { get; set; }
    public Vector3 originalPosition { get; set; }

    public void Init(HandController cardsView, HeroActionData action, CombatantHero hero = null) {
        this.cardsView = cardsView;
        this.action = action;
        if (action == null) {
            text.text = "";
            text.enabled = false;
            sprite.enabled = false;
            GetComponent<Collider>().enabled = false;
        } else {
            this.hero = hero;
            text.text = "[" + action.energyCost + "] " + action.name;
            text.enabled = true;
            sprite.enabled = true;
            GetComponent<Collider>().enabled = true;
        }
    }

    private string GetCardDescription() {
        var returner = action.tooltip;
        while (returner.Contains("#")) {
            var hash = returner.IndexOf('#'); 
            var act = 1;
            var offset = 0;
            if (hash < returner.Length - 1 && char.IsDigit(returner[hash + 1])) {
                act = (int)char.GetNumericValue(returner[hash + 1]);
                offset = 1;
            }
            var active = action.actives[act - 1];
            var dmg = Mathf.RoundToInt(active.amount * hero.GetAttribute(active.ScalingAttribute) / 10f);
            returner = returner.Substring(0, hash) + dmg + returner.Substring(hash + 1 + offset);
        }        
        return returner;
    }

    private void OnMouseEnter() {
        CombatUIController.Instance.SetTooltip(GetCardDescription());
    }

    private void OnMouseExit() {
        CombatUIController.Instance.SetTooltip(null);
    }

    private void OnMouseDown() {
        originalPosition = transform.position;
        cardsView.TryPickupCard(this);
    }
}
