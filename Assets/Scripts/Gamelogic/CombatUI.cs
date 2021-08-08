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
    [SerializeField] GameObject enemyActionBanner;
    
    [SerializeField] GameObject energyImage;
    [SerializeField] GameObject tooltip;
    [SerializeField] LineRenderer cardAimerLine;
    [SerializeField] GameObject validTarget;
    [SerializeField] GameObject invalidTarget;

    [SerializeField] GameObject endTurnButton;
    [SerializeField] GameObject advanceButton;
    [SerializeField] GameObject holdButton;

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

    public void UnparentActive() {
        activeHeroSelector.transform.parent = null;
    }

    public void MoveActive() {        
        activeHeroSelector.SetActive(true);
        activeHeroSelector.transform.position = combatMaster.CurrentCombatant.GameObject.transform.position;
        activeHeroSelector.transform.parent = combatMaster.CurrentCombatant.GameObject.transform;
    }

    public void StartTurn() {        
        CurrentHero.DrawHand(3);
        var lh = CurrentHero.HeroData.lhEquipment;
        var rh = CurrentHero.HeroData.rhEquipment;
        if (lh != null && lh.action != null) { lhEquipmentCard.SetContent(lh.action, CurrentHero); }
        if (rh != null && rh.action != null) { rhEquipmentCard.SetContent(rh.action, CurrentHero); }
        card1.SetContent(CurrentHero.hand[0].action, CurrentHero);
        card2.SetContent(CurrentHero.hand[1].action, CurrentHero);
        card3.SetContent(CurrentHero.hand[2].action, CurrentHero);        
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

    public void AdvanceButtonClicked() {
        DisableUI();
        CurrentHero.DiscardHand();
        StartCoroutine(combatMaster.RepeatHeroesAdvance());        
    }

    public void UpdateAdvanceButton() {
        if (!combatMaster.CurrentCombatant.isHero) {
            advanceButton.SetActive(false);
            holdButton.SetActive(false);
            endTurnButton.SetActive(false);

        } else if (combatMaster.CanHeroesAdvance()) {
            advanceButton.SetActive(true);
            holdButton.SetActive(true);
            endTurnButton.SetActive(false);

        } else {
            advanceButton.SetActive(false);
            holdButton.SetActive(false);
            endTurnButton.SetActive(true);
        }       
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
        advanceButton.SetActive(false);
        holdButton.SetActive(false);
    }

    private void UpdateEnergyRemaining() {
        energy.text = energyRemaining.Value + " / " + energyMax.Value;
    }

    public void TryPickupCard(CardView card) {
        if (card.action.energyCost <= energyRemaining.Value) {
            SetTooltip(null);
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
                    StartCoroutine(combatMaster.PerformHeroAction(currentCard.action, CurrentHero));
                    energyRemaining.Value -= currentCard.action.energyCost;
                    currentCard.SetContent(null);
                }
            }

            if (isAimingCard) {
                if (currentTarget != null) {
                    StartCoroutine(combatMaster.PerformHeroAction(currentCard.action, CurrentHero, currentTarget));
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


    public void SetTooltip(string text) {
        if (text == null || currentCard != null) {
            tooltip.SetActive(false);
        } else {
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = text;
            tooltip.SetActive(true);
        }
    }

    public void SetEnemyActionBanner(string text) {
        if (text == null || text == "") {
            enemyActionBanner.SetActive(false);
        } else {
            enemyActionBanner.SetActive(true);
            enemyActionBanner.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }
    }

}
