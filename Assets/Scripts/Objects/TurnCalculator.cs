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
    private Dictionary<string, Combatant> monsterNames = new Dictionary<string, Combatant>();

    public void Init() {
        turns = new List<CombatantTurn>();
        foreach (var c in CombatController.Instance.activeCombatants) { 
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

    public void AdjustSpeed(Combatant c, float oldSpeed, float newSpeed) {
        var firstTurn = turns.First(t => t.combatant == c);
        if (firstTurn == null) { Debug.LogError("Turn not found for " + c.name); return; }
        var currentTime = turns[0].time;
        var initiative = (firstTurn.time - currentTime) * (newSpeed / oldSpeed) + currentTime;
        turns.RemoveAll(t => t.combatant == c);
        AddTurns(c, initiative);
    }

    public void AddCombatant(Combatant c) {
        if (!c.isHero) {
            int i = 1;
            while (monsterNames.ContainsKey(c.name + " " + i)) {
                i++;
            }
            monsterNames.Add(c.name + " " + i, c);
            c.name = c.name + " " + i; 
        }
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
