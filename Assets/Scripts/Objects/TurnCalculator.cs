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
        foreach (var c in CombatMaster.Instance.activeCombatants) { 
            var initiative = c.speedFactor * UnityEngine.Random.Range(0f, 1f);
            AddTurns(c, initiative);
        }
        
    }

    public Combatant GetCurrentTurn() {
        return turns[0].combatant;
    }

    public void RemoveTurn() {
        var turn = turns[0];
        var c = turns[0].combatant;
        turns.RemoveAt(0);
        turns.Add(new CombatantTurn(turn.time + c.speedFactor * NUMBER_OF_TURNS_SHOWN, c));
        turns = turns.OrderBy(t => t.time).ToList();
        OnUpdate?.Invoke(turns);
    }

    public void KillCombatant(Combatant c) {
        turns.RemoveAll(t => t.combatant == c);
        OnUpdate?.Invoke(turns);
    }

    public void ReevaluateTurns(Combatant c) {
        var initiative = turns.First(t => t.combatant == c).time;
        turns.RemoveAll(t => t.combatant == c);
        AddTurns(c, initiative);
    }

    public void AddCombatant(Combatant c) {
        var initiative = c.speedFactor * UnityEngine.Random.Range(0f, 1f) + turns[0].time;
        AddTurns(c, initiative);
    }

    private void AddTurns(Combatant c, float initiative) {
        for (int i = 0; i < NUMBER_OF_TURNS_SHOWN; i++) {
            turns.Add(new CombatantTurn(initiative + c.speedFactor * i, c));
        }
        turns = turns.OrderBy(t => t.time).ToList();
        OnUpdate?.Invoke(turns);
    }

}
