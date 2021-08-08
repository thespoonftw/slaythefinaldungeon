using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatantMonster : Combatant {

    private int meleeActionIndex;
    private int rangedActionIndex;
    private MonsterData monsterData;

    public MonsterActionData CurrentMeleeAction => monsterData.meleeActions[meleeActionIndex];

    public MonsterActionData CurrentRangedAction => monsterData.rangedActions[rangedActionIndex];

    public CombatantMonster(CombatantView view, MonsterData data, int x, int y) : base(view, data, x, y) {
        name = data.name;
        CurrentHp.Value = data.maxHp;
        this.monsterData = data;
        meleeActionIndex = Random.Range(0, data.meleeActions.Count);
        rangedActionIndex = Random.Range(0, data.rangedActions.Count);
        view.Init(this);
    }

    public void IncrementMeleeIndex() {
        meleeActionIndex += 1;
        if (meleeActionIndex >= monsterData.meleeActions.Count) { meleeActionIndex -= monsterData.meleeActions.Count; }
    }

    public void IncrementRangedIndex() {
        rangedActionIndex += 1;
        if (rangedActionIndex >= monsterData.rangedActions.Count) { rangedActionIndex -= monsterData.rangedActions.Count; }
    }
}
