using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : Singleton<CombatUI> {

    [SerializeField] CardView lhEquipmentCard;
    [SerializeField] CardView rhEquipmentCard;
    [SerializeField] CardView card1;
    [SerializeField] CardView card2;
    [SerializeField] CardView card3;
    [SerializeField] TextMeshProUGUI energy;

    [SerializeField] GameObject targetSelector;
    [SerializeField] GameObject activeHeroSelector;
    [SerializeField] GameObject endTurnButton;
    [SerializeField] GameObject energyImage;
    [SerializeField] GameObject tooltip;
    [SerializeField] LineRenderer cardAimerLine;
    [SerializeField] GameObject validTarget;
    [SerializeField] GameObject invalidTarget;

    private CombatMaster combatMaster;
    private CardView currentCard;
    private bool isHoldingCard = false;
    private bool isAimingCard = false;
    private Combatant currentTarget;

    private EProperty<int> energyRemaining = new EProperty<int>();
    private EProperty<int> energyMax = new EProperty<int>();

    private CombatantHero CurrentHero => combatMaster.CurrentCombatant.isHero ? (CombatantHero)combatMaster.CurrentCombatant : null;


    private void Start() {
        combatMaster = CombatMaster.Instance;
        Inputs.OnLeftMouseUp += OnLeftMouseUp;
        energyRemaining.OnUpdate += UpdateEnergyRemaining;
        energyMax.OnUpdate += UpdateEnergyRemaining;
        DisableUI();
    }

    private void OnDestroy() {
        Inputs.OnLeftMouseUp -= OnLeftMouseUp;
        energyRemaining.OnUpdate -= UpdateEnergyRemaining;
        energyMax.OnUpdate += UpdateEnergyRemaining;
    }

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

    public void MoveActive() {        
        activeHeroSelector.SetActive(true);
        activeHeroSelector.transform.position = combatMaster.CurrentCombatant.GameObject.transform.position;
    }

    public void StartTurn() {        
        CurrentHero.DrawHand(3);
        var lh = CurrentHero.HeroData.lhEquipment;
        var rh = CurrentHero.HeroData.rhEquipment;
        if (lh != null && lh.actionType != null) { lhEquipmentCard.SetContent(lh.actionType); }
        if (rh != null && rh.actionType != null) { rhEquipmentCard.SetContent(rh.actionType); }
        card1.SetContent(CurrentHero.hand[0].active);
        card2.SetContent(CurrentHero.hand[1].active);
        card3.SetContent(CurrentHero.hand[2].active);        
        energyImage.SetActive(true);
        endTurnButton.SetActive(true);
        energyRemaining.Value = CurrentHero.HeroData.maxEnergy;
        energyMax.Value = CurrentHero.HeroData.maxEnergy;        
    }

    public void EndTurnClicked() {
        DisableUI();
        CurrentHero.DiscardHand();
        combatMaster.EndTurn();
    }

    public void DisableUI() {
        lhEquipmentCard.SetContent(null);
        rhEquipmentCard.SetContent(null);
        card1.SetContent(null);
        card2.SetContent(null);
        card3.SetContent(null);
        endTurnButton.SetActive(false);
        energyImage.SetActive(false);
        tooltip.SetActive(false);
        activeHeroSelector.SetActive(false);
    }

    private void UpdateEnergyRemaining() {
        energy.text = energyRemaining.Value + " / " + energyMax.Value;
    }

    public void TryPickupCard(CardView card) {
        if (card.action.energyCost <= energyRemaining.Value) {
            SetTooltip(null);
            currentCard = card;
            if (card.action.targettingMode == ActionTarget.NoTarget) {
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
                    combatMaster.PerformAction(currentCard.action, CurrentHero);
                    energyRemaining.Value -= currentCard.action.energyCost;
                    currentCard.SetContent(null);
                }
            }

            if (isAimingCard) {
                if (currentTarget != null) {
                    combatMaster.PerformAction(currentCard.action, CurrentHero, currentTarget);
                    energyRemaining.Value -= currentCard.action.energyCost;
                    currentCard.SetContent(null);
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

    public void TargetCombatant(Combatant combatant) {
        if (!isAimingCard) { return; }
        if (combatant != null) {            
            if (
                (currentCard.action.targettingMode == ActionTarget.Any) ||
                (currentCard.action.targettingMode == ActionTarget.Target && combatant != CurrentHero) ||
                (currentCard.action.targettingMode == ActionTarget.Enemy && combatMaster.LivingMonsters.Contains(combatant)) ||
                (currentCard.action.targettingMode == ActionTarget.Friendly && combatMaster.LivingHeroes.Contains(combatant)) ||
                (currentCard.action.targettingMode == ActionTarget.OtherAlly && combatMaster.LivingHeroes.Contains(combatant) && combatant != CurrentHero)
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


    public void SetTooltip(string text) {
        if (text == null || currentCard != null) {
            tooltip.SetActive(false);
        } else {
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = text;
            tooltip.SetActive(true);
        }
    }

}
