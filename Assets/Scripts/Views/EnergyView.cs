using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image background;

    private void Start() {
        CombatUI.Instance.EnergyRemaining.OnUpdate += UpdateEnergyRemaining;
        CombatUI.Instance.EnergyMax.OnUpdate += UpdateEnergyRemaining;
        CombatUI.Instance.IsHeroUIEnabled.OnNewValue += ShowHide;
    }

    private void OnDestroy() {
        CombatUI.Instance.EnergyRemaining.OnUpdate -= UpdateEnergyRemaining;
        CombatUI.Instance.EnergyMax.OnUpdate += UpdateEnergyRemaining;
        CombatUI.Instance.IsHeroUIEnabled.OnNewValue -= ShowHide;
    }

    private void UpdateEnergyRemaining() {
        textMesh.text = CombatUI.Instance.EnergyRemaining.Value + " / " + CombatUI.Instance.EnergyMax.Value;
    }

    private void ShowHide(bool isShown) {
        textMesh.enabled = false;
        background.enabled = false;
    }
}
