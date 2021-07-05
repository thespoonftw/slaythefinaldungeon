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

    private bool isTargetting = false;
    private int targetIndex = 0;
    private CombatMaster combatMaster;
    private ActionData currentAction;
    private CardView currentCard;
    private List<Combatant> availableTargets;

    private EProperty<int> energyRemaining = new EProperty<int>();
    private EProperty<int> energyMax = new EProperty<int>();


    private Combatant CurrentTarget => availableTargets[targetIndex];
    private CombatantHero CurrentHero => combatMaster.CurrentCombatant.isHero ? (CombatantHero)combatMaster.CurrentCombatant : null;
    private EquipmentData LHequipment => CurrentHero.hero.lhEquipment;
    private EquipmentData RHequipment => CurrentHero.hero.rhEquipment;


    private void Start() {
        combatMaster = CombatMaster.Instance;
        Inputs.OnEnterKey += ConfirmTargetChoice;
        Inputs.OnUpDownArrow += SwitchTargetChoice;
        Inputs.OnEscKey += CancelTargetChoice;
        energyRemaining.OnUpdate += UpdateEnergyRemaining;
        energyMax.OnUpdate += UpdateEnergyRemaining;
        DisableUI();
    }

    private void OnDestroy() {
        Inputs.OnUpDownArrow -= SwitchTargetChoice;
        Inputs.OnEnterKey -= ConfirmTargetChoice;
        Inputs.OnEscKey -= CancelTargetChoice;
        energyRemaining.OnUpdate -= UpdateEnergyRemaining;
        energyMax.OnUpdate += UpdateEnergyRemaining;
    }

    public void StartTurn() {
        CurrentHero.DrawHand(3);
        if (LHequipment != null && LHequipment.actionType != null) { lhEquipmentCard.SetContent(LHequipment.actionType); }
        if (RHequipment != null && RHequipment.actionType != null) { rhEquipmentCard.SetContent(RHequipment.actionType); }
        card1.SetContent(CurrentHero.hand[0].active);
        card2.SetContent(CurrentHero.hand[1].active);
        card3.SetContent(CurrentHero.hand[2].active);

        activeHeroSelector.SetActive(true);
        energyImage.SetActive(true);
        endTurnButton.SetActive(true);
        energyRemaining.Value = CurrentHero.hero.energy;
        energyMax.Value = CurrentHero.hero.energy;
        activeHeroSelector.transform.position = combatMaster.CurrentCombatant.GameObject.transform.position;
    }

    public void StartAction(CardView cardview, ActionData actionData) {
        if (energyRemaining.Value - actionData.energyCost < 0) { return; }
        if (isTargetting) { return; }

        if (actionData.targettingMode != ActionTargettingMode.noTarget) {
            StartTargetChoice(cardview, actionData);
        } else {
            combatMaster.PerformAction(actionData, CurrentHero);
            energyRemaining.Value -= actionData.energyCost;
            cardview.SetContent(null);
        }
    }

    private void StartTargetChoice(CardView cardview, ActionData actionData) {
        currentCard = cardview;
        currentAction = actionData;
        if (actionData.targettingMode == ActionTargettingMode.enemy) {
            availableTargets = combatMaster.LivingMonsters;
        } else if (actionData.targettingMode == ActionTargettingMode.friendly) {
            availableTargets = combatMaster.LivingHeroes;
        } else if (actionData.targettingMode == ActionTargettingMode.adjacent) {
            availableTargets = combatMaster.LivingHeroes;
            availableTargets.Remove(CurrentHero);
        }
        targetIndex = 0;
        isTargetting = true;
        targetSelector.SetActive(true);
        targetSelector.transform.position = CurrentTarget.GameObject.transform.position;

    }

    private void SwitchTargetChoice(bool isUp) {
        if (!isTargetting) { return; }
        targetIndex = isUp ? targetIndex + 1 : targetIndex - 1;
        if (targetIndex < 0) { targetIndex += availableTargets.Count; }
        if (targetIndex >= availableTargets.Count) { targetIndex -= availableTargets.Count; }
        targetSelector.transform.position = CurrentTarget.GameObject.transform.position;
    }

    private void CancelTargetChoice() { 
        if (!isTargetting) { return; }
        isTargetting = false;
        targetSelector.SetActive(false);
        StartTurn();
    }

    private void ConfirmTargetChoice() { 
        if (!isTargetting) { return; }
        currentCard.SetContent(null);
        isTargetting = false;
        targetSelector.SetActive(false);
        combatMaster.PerformAction(currentAction, CurrentHero, CurrentTarget);
        energyRemaining.Value -= currentAction.energyCost;        
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

    public void ShowTooltip(string text) {
        if (text != null) {
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = text;
            tooltip.SetActive(true);
        } else {
            tooltip.SetActive(false);
        }
    }

}
