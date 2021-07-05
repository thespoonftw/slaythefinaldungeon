using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;

    private ActionData action;

    public void SetContent(ActionData action) {
        this.action = action;
        if (action == null) {
            text.text = "";
            text.enabled = false;
            GetComponent<Image>().enabled = false;
        } else {
            text.text = "[" + action.energyCost + "] " + action.name;
            text.enabled = true;
            GetComponent<Image>().enabled = true;
        }
    }

    public void EventMouseDown() {

    }

    public void EventMouseUp() {
        CombatUI.Instance.StartAction(this, action);
    }

    public void EventMouseEnter() {
        CombatUI.Instance.ShowTooltip(action.tooltip);
    }

    public void EventMouseExit() {
        CombatUI.Instance.ShowTooltip(null);
    }

}
