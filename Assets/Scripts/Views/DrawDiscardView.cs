using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawDiscardView : MonoBehaviour {

    [SerializeField] bool isDrawMode;
    [SerializeField] TextMeshProUGUI valueMesh;
    [SerializeField] TextMeshProUGUI labelMesh;
    [SerializeField] Image background;

    private void Start() {
        if (isDrawMode) { 
            CombatUIController.Instance.DrawSize.OnNewValue += UpdateValue;
        } else {
            CombatUIController.Instance.DiscardSize.OnNewValue += UpdateValue;
        }
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue += ShowHide;
    }

    private void OnDestroy() {
        if (isDrawMode) { 
            CombatUIController.Instance.DrawSize.OnNewValue -= UpdateValue;
        } else {
            CombatUIController.Instance.DiscardSize.OnNewValue -= UpdateValue;
        }
        CombatUIController.Instance.IsHeroUIEnabled.OnNewValue -= ShowHide;
    }

    private void UpdateValue(int amount) {
        valueMesh.text = amount.ToString();
    }

    private void ShowHide(bool isShown) {
        valueMesh.enabled = isShown;
        background.enabled = isShown;
        labelMesh.enabled = isShown;
    }
}
