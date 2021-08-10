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
        CombatUIController.Instance.IsAdvanceAllowed.OnNewValue += CanAdvance;
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue += ShowHide;
    }

    private void OnDestroy() {
        CombatUIController.Instance.IsAdvanceAllowed.OnNewValue += CanAdvance;
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue -= ShowHide;
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
        CombatUIController.Instance.EndHeroTurn();
        CombatController.Instance.EndTurn();
    }

    public void AdvanceButtonClicked() {
        CombatUIController.Instance.EndHeroTurn();
        StartCoroutine(CombatController.Instance.RepeatHeroesAdvance());
    }
}
