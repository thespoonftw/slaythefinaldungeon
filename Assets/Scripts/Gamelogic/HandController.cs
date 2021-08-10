using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandController : Singleton<HandController> {

    [SerializeField] GameObject equip1;
    [SerializeField] GameObject equip2;
    [SerializeField] GameObject hand1;
    [SerializeField] GameObject hand2;
    [SerializeField] GameObject hand3;

    [SerializeField] LineRenderer cardAimerLine;
    [SerializeField] GameObject validTarget;
    [SerializeField] GameObject invalidTarget;
    [SerializeField] GameObject cardPrefab;

    private CombatController combatMaster;
    private CombatUIController combatUI;
    private CardView currentCard;
    private bool isHoldingCard = false;
    private bool isAimingCard = false;
    private Combatant currentTarget;
    public List<CardData> hand = new List<CardData>();
    private CombatantHero hero;

    public List<CardData> DrawDeck => hero.drawDeck;
    public List<CardData> DiscardPile => hero.discardPile;

    private void Start() {
        combatMaster = CombatController.Instance;
        combatUI = CombatUIController.Instance;
        Inputs.OnLeftMouseUp += OnLeftMouseUp;
        combatUI.HoveredCombatant.OnNewValue += TargetCombatant;
    }

    private void OnDestroy() {
        Inputs.OnLeftMouseUp -= OnLeftMouseUp;
        combatUI.HoveredCombatant.OnNewValue -= TargetCombatant;

    }

    public void StartTurn(CombatantHero hero, int handSize) {
        this.hero = hero;
        var lh = hero.HeroData.lhEquipment;
        var rh = hero.HeroData.rhEquipment;
        if (lh != null && lh.action != null) { CreateCard(equip1, lh.action); }
        if (rh != null && rh.action != null) { CreateCard(equip2, rh.action); }
        for (int i = 0; i < handSize; i++) { DrawCard(); }
    }

    private void CreateCard(GameObject location, HeroActionData action) {
        var go = Instantiate(cardPrefab, location.transform.position, Quaternion.identity, location.transform);
        var view = go.GetComponent<CardView>();
        view.Init(this, action, hero);
    }

    public void DiscardHand() {
        DiscardPile.AddRange(hand);
        hand.Clear();
    }

    public void DrawCard() {
        if (DrawDeck.Count == 0) {
            hero.drawDeck = new List<CardData>(DiscardPile);
            DiscardPile.Clear();
        }
        var randomCard = DrawDeck[Random.Range(0, DrawDeck.Count)];
        hand.Add(randomCard);
        DrawDeck.Remove(randomCard);
    }

    /*
    public void EndHeroTurn() {
        CurrentHero.DiscardHand();
        IsHeroUIEnabled.Value = false;
        lhEquipmentCard.SetContent(null);
        rhEquipmentCard.SetContent(null);
        card1.SetContent(null);
        card2.SetContent(null);
        card3.SetContent(null);
        tooltip.SetActive(false);
        activeHeroSelector.SetActive(false);
    }
    */

    private void Update() {
        if (currentCard != null && isHoldingCard) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -1;
            currentCard.transform.position = pos;
        }        
        if (currentCard != null && isAimingCard) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -1;
            cardAimerLine.SetPosition(0, currentCard.transform.position);
            cardAimerLine.SetPosition(1, pos);
        }
    }

    public void TryPickupCard(CardView card) {
        if (card.action.energyCost <= combatUI.EnergyRemaining.Value) {
            //SetTooltip(null);
            currentCard = card;
            if (card.action.targettingMode == HeroActionTarget.NoTarget) {
                isHoldingCard = true;
            } else {
                isAimingCard = true;
                cardAimerLine.enabled = true;
            }
        }
    }

    private void OnLeftMouseUp() {
        if (currentCard != null) {    
            
            if (isHoldingCard) {
                currentCard.transform.position = currentCard.originalPosition;
                if (Input.mousePosition.y > 100) {
                    combatUI.UseCard(currentCard.action, null);
                    currentCard.Init(this, null);
                }
            }

            if (isAimingCard) {
                if (currentTarget != null) {
                    combatUI.UseCard(currentCard.action, currentTarget);
                    currentCard.Init(this, null);
                }
            }
            cardAimerLine.enabled = false;
            currentCard = null;
            isHoldingCard = false;
            isAimingCard = false;
            currentTarget = null;
            validTarget.SetActive(false);
            invalidTarget.SetActive(false);
        }
        
    }

    private void TargetCombatant(Combatant combatant) {
        if (!isAimingCard) { return; }
        if (combatant != null) {
            var dist = combatant.x - combatMaster.progressIndex;
            if (
                (currentCard.action.targettingMode == HeroActionTarget.Range3 && dist <= 3) ||
                (currentCard.action.targettingMode == HeroActionTarget.Range2 && dist <= 2) ||
                (currentCard.action.targettingMode == HeroActionTarget.Range1 && dist <= 1)
                ) {
                validTarget.SetActive(true);
                validTarget.transform.position = combatant.GameObject.transform.position + new Vector3(0, 0, -1);
                currentTarget = combatant;
            } else {
                invalidTarget.SetActive(true);
                invalidTarget.transform.position = combatant.GameObject.transform.position + new Vector3(0, 0, -1);
            }
        } else {
            validTarget.SetActive(false);
            invalidTarget.SetActive(false);
            currentTarget = null;
        }
    }

    public void TurnOrderHighlight(Combatant combatant) {
        if (combatant != null) {
            validTarget.SetActive(true);
            validTarget.transform.position = combatant.GameObject.transform.position + new Vector3(0, 0, -1);
        } else {
            validTarget.SetActive(false);
        }        
    }
}
