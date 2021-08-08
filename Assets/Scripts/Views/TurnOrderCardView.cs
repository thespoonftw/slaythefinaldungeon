using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderCardView : MonoBehaviour {

    private Combatant combatant;
    private int cardIndex;
    private TextMeshProUGUI textMesh;

    public void Awake() {
        TurnCalculator.Instance.OnUpdate += UpdateTurns;
        cardIndex = int.Parse(gameObject.name.Split(' ')[1]);
        textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy() {
        if (TurnCalculator.Instance != null) TurnCalculator.Instance.OnUpdate -= UpdateTurns;
    }

    private void UpdateTurns(List<CombatantTurn> turns) {
        textMesh.text = turns[cardIndex].combatant.name;
        //turnCards[i].text = turns[i].combatant.name + " " + turns[i].time;
        combatant = turns[cardIndex].combatant;
    }

    public void MouseEnter() {
        CombatUI.Instance.TurnOrderHighlight(combatant);
        GetComponent<Image>().color = Color.green;
    }

    public void MouseExit() {
        CombatUI.Instance.TurnOrderHighlight(null);
        GetComponent<Image>().color = Color.white;
    }
}
