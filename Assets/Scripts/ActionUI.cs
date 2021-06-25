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
    [SerializeField] GameObject enemySelector;
    [SerializeField] GameObject activeHeroSelector;

    private bool isActionChoiceEnabled = false;
    private bool isTargetChoiceEnabled = false;
    private int actionChoice = 0;
    private int enemyChoice = 0;
    private GameMaster gameMaster;

    private Combatant CurrentTarget => gameMaster.LivingEnemies[enemyChoice];

    private void Start() {
        gameMaster = GameMaster.Instance;
        Inputs.OnLeftArrow += SwitchActionChoice;
        Inputs.OnRightArrow += SwitchActionChoice;
        Inputs.OnEnterKey += ConfirmActionChoice;
        Inputs.OnEnterKey += ConfirmTargetChoice;
        Inputs.OnUpDownArrow += SwitchTargetChoice;
        Inputs.OnEscKey += CancelTargetChoice;
        actionOneText.enabled = false;
        actionTwoText.enabled = false;
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
        actionOneText.enabled = true;
        actionTwoText.enabled = true;
        actionOneSelect.enabled = true;
        actionTwoSelect.enabled = false;
        activeHeroSelector.SetActive(true);
        activeHeroSelector.transform.position = gameMaster.CurrentCharacter.transform.position;
        actionChoice = 0;
    }

    private void SwitchActionChoice() {
        if (!isActionChoiceEnabled) { return; }
        actionChoice = (actionChoice == 0) ? 1 : 0;
        actionOneSelect.enabled = (actionChoice == 0);
        actionTwoSelect.enabled = (actionChoice == 1);
    }

    private void ConfirmActionChoice() {
        if (!isActionChoiceEnabled) { return; }
        isActionChoiceEnabled = false;
        actionOneText.enabled = false;
        actionTwoText.enabled = false;
        actionOneSelect.enabled = false;
        actionTwoSelect.enabled = false;
        if (actionChoice == 0) {
            Helper.DelayMethod(0.1f, StartTargetChoice);
        } else {
            gameMaster.PerformAction(new Action() {
                damage = -10,
                source = gameMaster.CurrentCharacter,
                target = gameMaster.CurrentCharacter
            });
            activeHeroSelector.SetActive(false);
        }
    }

    private void StartTargetChoice() {
        enemyChoice = 0; // need to set this to first living enemy
        isTargetChoiceEnabled = true;
        enemySelector.SetActive(true);
        enemySelector.transform.position = CurrentTarget.transform.position;
    }

    private void SwitchTargetChoice(bool isUp) {
        if (!isTargetChoiceEnabled) { return; }
        enemyChoice = isUp ? enemyChoice + 1 : enemyChoice - 1;
        if (enemyChoice < 0) { enemyChoice += gameMaster.LivingEnemies.Count; }
        if (enemyChoice >= gameMaster.LivingEnemies.Count) { enemyChoice -= gameMaster.LivingEnemies.Count; }
        enemySelector.transform.position = CurrentTarget.transform.position;
    }

    private void CancelTargetChoice() { 
        if (!isTargetChoiceEnabled) { return; }
        isTargetChoiceEnabled = false;
        enemySelector.SetActive(false);
        StartActionChoice();
    }

    private void ConfirmTargetChoice() { 
        if (!isTargetChoiceEnabled) { return; }
        isTargetChoiceEnabled = false;
        enemySelector.SetActive(false);
        gameMaster.PerformAction(new Action() {
            damage = gameMaster.CurrentCharacter.str,
            source = gameMaster.CurrentCharacter,
            target = CurrentTarget
        });
        activeHeroSelector.SetActive(false);
    }




}
