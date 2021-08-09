using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnView : MonoBehaviour {

    [SerializeField] GameObject endTurn;
    [SerializeField] GameObject endTurnStay;
    [SerializeField] GameObject endTurnAdvance;

    private bool canAdvance = false;
    private bool isShown = false;

    private void Start() {
        CombatUI.Instance.IsAdvanceAllowed.OnNewValue += CanAdvance;
        CombatUI.Instance.IsHeroUIEnabled.OnNewValue += ShowHide;
    }

    private void OnDestroy() {
        CombatUI.Instance.IsAdvanceAllowed.OnNewValue += CanAdvance;
        CombatUI.Instance.IsHeroUIEnabled.OnNewValue -= ShowHide;
    }

    private void CanAdvance(bool canAdvance) {
        this.canAdvance = canAdvance;
        UpdateButtons();
    }


    private void ShowHide(bool isShown) {
        this.isShown = isShown;
        UpdateButtons();
    }

    private void UpdateButtons() {
        endTurnAdvance.SetActive(isShown && canAdvance);
        endTurnStay.SetActive(isShown && canAdvance);
        endTurn.SetActive(isShown && !canAdvance);
    }

    public void EndTurnClicked() {
        CombatUI.Instance.EndHeroTurn();
        CombatMaster.Instance.EndTurn();
    }

    public void AdvanceButtonClicked() {
        CombatUI.Instance.EndHeroTurn();
        StartCoroutine(CombatMaster.Instance.RepeatHeroesAdvance());
    }
}
