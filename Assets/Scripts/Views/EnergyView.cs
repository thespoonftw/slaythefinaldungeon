using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image background;

    private void Start() {
        CombatUIController.Instance.EnergyRemaining.OnUpdate += UpdateEnergyRemaining;
        CombatUIController.Instance.EnergyMax.OnUpdate += UpdateEnergyRemaining;
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue += ShowHide;
    }

    private void OnDestroy() {
        CombatUIController.Instance.EnergyRemaining.OnUpdate -= UpdateEnergyRemaining;
        CombatUIController.Instance.EnergyMax.OnUpdate += UpdateEnergyRemaining;
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue -= ShowHide;
    }

    private void UpdateEnergyRemaining() {
        textMesh.text = CombatUIController.Instance.EnergyRemaining.Value + " / " + CombatUIController.Instance.EnergyMax.Value;
    }

    private void ShowHide(bool isShown) {
        textMesh.enabled = isShown;
        background.enabled = isShown;
    }
}
