using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnOrderView : MonoBehaviour {

    [SerializeField] List<TextMeshProUGUI> turnCards;

    public void Start() {
        TurnCalculator.Instance.OnUpdate += UpdateTurns;
    }

    private void OnDestroy() {
        TurnCalculator.Instance.OnUpdate -= UpdateTurns;
    }

    private void UpdateTurns(List<CombatantTurn> turns) {
        for (int i=0; i<TurnCalculator.NUMBER_OF_TURNS_SHOWN; i++) {
            turnCards[i].text = turns[i].combatant.name;
        }
    }
}
