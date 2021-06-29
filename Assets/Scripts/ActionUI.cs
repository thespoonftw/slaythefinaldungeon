using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour {

    [SerializeField] TextMeshProUGUI actionOneText;
    [SerializeField] TextMeshProUGUI actionTwoText;
    [SerializeField] Image actionOneSelect;
    [SerializeField] Image actionTwoSelect;
    [SerializeField] GameObject targetSelector;
    [SerializeField] GameObject activeHeroSelector;

    private bool isActionChoiceEnabled = false;
    private bool isTargetChoiceEnabled = false;
    private bool isLeftAction = true;
    private int targetIndex = 0;
    private bool isActionOneEnabled = false;
    private bool isActionTwoEnabled = false;
    private bool isTargettingEnemies = false;
    private GameMaster gameMaster;

    private List<Combatant> AvailableTargets => isTargettingEnemies ? gameMaster.LivingEnemies : gameMaster.LivingHeroes;
    private Combatant CurrentTarget => AvailableTargets[targetIndex];
    private Combatant CurrentCombatant => gameMaster.CurrentCombatant;
    private EquipmentData LHequipment => CurrentCombatant.hero.lhEquipment;
    private EquipmentData RHequipment => CurrentCombatant.hero.rhEquipment;


    private void Start() {
        gameMaster = GameMaster.Instance;
        Inputs.OnLeftArrow += SwitchActionChoice;
        Inputs.OnRightArrow += SwitchActionChoice;
        Inputs.OnEnterKey += ConfirmActionChoice;
        Inputs.OnEnterKey += ConfirmTargetChoice;
        Inputs.OnUpDownArrow += SwitchTargetChoice;
        Inputs.OnEscKey += CancelTargetChoice;
        actionOneText.text = "";
        actionTwoText.text = "";
        actionOneSelect.enabled = false;
        actionTwoSelect.enabled = false;
    }

    private void OnDestroy() {
        Inputs.OnLeftArrow -= SwitchActionChoice;
        Inputs.OnRightArrow -= SwitchActionChoice;
        Inputs.OnEnterKey -= ConfirmActionChoice;
        Inputs.OnUpDownArrow -= SwitchTargetChoice;
        Inputs.OnEnterKey -= ConfirmTargetChoice;
        Inputs.OnEscKey -= CancelTargetChoice;
    }

    public void StartActionChoice() {
        isActionChoiceEnabled = true;
        SetEquipmentText(true);
        SetEquipmentText(false);
        actionOneSelect.enabled = true;
        actionTwoSelect.enabled = false;
        activeHeroSelector.SetActive(true);
        activeHeroSelector.transform.position = gameMaster.CurrentCombatant.GameObject.transform.position;
        isLeftAction = true;
    }

    private void SetEquipmentText(bool isLeft) {
        bool isAction = false;
        var equipment = isLeft ? LHequipment : RHequipment;
        var text = isLeft ? actionOneText : actionTwoText;
        if (equipment == null) { 
            text.text = ""; 
        } else {
            text.text = equipment.name;
            isAction = equipment.actionType != null;
            text.color = isAction ? Color.white : Color.gray;
        }
        if (isLeft) { isActionOneEnabled = isAction; } else { isActionTwoEnabled = isAction; }
    }

    private void SwitchActionChoice() {
        if (!isActionChoiceEnabled) { return; }
        if (!isActionOneEnabled || !isActionTwoEnabled) { return; }
        isLeftAction = !isLeftAction;
        actionOneSelect.enabled = isLeftAction;
        actionTwoSelect.enabled = !isLeftAction;
    }

    private void ConfirmActionChoice() {
        if (!isActionChoiceEnabled) { return; }
        isActionChoiceEnabled = false;
        actionOneText.text = "";
        actionTwoText.text = "";
        actionOneSelect.enabled = false;
        actionTwoSelect.enabled = false;
        var actionData = isLeftAction ? LHequipment.actionType : RHequipment.actionType;
        if (actionData.RequiresEnemyTarget) {
            Helper.DelayMethod(0.1f, () => StartTargetChoice(true));
        } else if (actionData.RequiresFriendlyTarget) {
            Helper.DelayMethod(0.1f, () => StartTargetChoice(false));
        } else {
            gameMaster.PerformAction(actionData, CurrentCombatant);
            activeHeroSelector.SetActive(false);
        }
    }

    private void StartTargetChoice(bool isTargettingEnemies) {
        this.isTargettingEnemies = isTargettingEnemies;
        targetIndex = 0; // need to set this to first living enemy
        isTargetChoiceEnabled = true;
        targetSelector.SetActive(true);
        targetSelector.transform.position = CurrentTarget.GameObject.transform.position;
    }

    private void SwitchTargetChoice(bool isUp) {
        if (!isTargetChoiceEnabled) { return; }
        targetIndex = isUp ? targetIndex + 1 : targetIndex - 1;
        if (targetIndex < 0) { targetIndex += AvailableTargets.Count; }
        if (targetIndex >= AvailableTargets.Count) { targetIndex -= AvailableTargets.Count; }
        targetSelector.transform.position = CurrentTarget.GameObject.transform.position;
    }

    private void CancelTargetChoice() { 
        if (!isTargetChoiceEnabled) { return; }
        isTargetChoiceEnabled = false;
        targetSelector.SetActive(false);
        StartActionChoice();
    }

    private void ConfirmTargetChoice() { 
        if (!isTargetChoiceEnabled) { return; }
        isTargetChoiceEnabled = false;
        targetSelector.SetActive(false);
        var actionData = isLeftAction ? LHequipment.actionType : RHequipment.actionType;
        gameMaster.PerformAction(actionData, CurrentCombatant, CurrentTarget);
        activeHeroSelector.SetActive(false);
    }




}
