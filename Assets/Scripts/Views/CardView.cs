using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;

    private ActionData action;
    private bool isHeld;
    private float yOfNoReturn = 100;
    private Vector3 originalPosition;

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

    private void Update() {
        if (!isHeld) { return; }        
        transform.position = Input.mousePosition;
    }

    public void EventMouseDown() {
        isHeld = true;
        originalPosition = transform.position;
    }

    public void EventMouseUp() {
        isHeld = false;
        if (transform.position.y > 100) {
            CombatUI.Instance.StartAction(this, action);
        } else {
            transform.position = originalPosition;
        }
    }

    public void EventMouseEnter() {
        CombatUI.Instance.ShowTooltip(action.tooltip);
    }

    public void EventMouseExit() {
        CombatUI.Instance.ShowTooltip(null);
    }

}
