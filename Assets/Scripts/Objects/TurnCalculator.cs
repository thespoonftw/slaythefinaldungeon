using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatantTurn {
    public Combatant combatant;
    public float time;
    public CombatantTurn(float time, Combatant combatant) {
        this.time = time;
        this.combatant = combatant;
    }
}

public class TurnCalculator : Singleton<TurnCalculator> {

    private List<CombatantTurn> turns;
    public static int NUMBER_OF_TURNS_SHOWN = 10;
    public event Action<List<CombatantTurn>> OnUpdate;

    public void Init() {
        turns = new List<CombatantTurn>();
        foreach (var c in CombatMaster.Instance.Combatants) { 
            var initiative = c.speedFactor * UnityEngine.Random.Range(0f, 1f);
            for (int i = 0; i<NUMBER_OF_TURNS_SHOWN; i++) {
                turns.Add(new CombatantTurn(initiative + c.speedFactor * i, c));
            }            
        }
        turns = turns.OrderBy(t => t.time).ToList();
        OnUpdate?.Invoke(turns);
    }

    public Combatant TakeTurn() {
        var turn = turns[0];
        var c = turns[0].combatant;
        OnUpdate?.Invoke(turns);
        turns.RemoveAt(0);
        turns.Add(new CombatantTurn(turn.time + c.speedFactor * NUMBER_OF_TURNS_SHOWN, c));        
        return c;
    }

    public void KillCombatant(Combatant c) {
        turns.RemoveAll(t => t.combatant == c);
        OnUpdate?.Invoke(turns);
    }

}
