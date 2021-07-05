using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Combatant {

    public int str;
    public int resist;
    public int spriteId;
    public Stats baseStats;
    
    public CombatantView view;
    public EnemyData enemyData;

    public bool isHero = false;

    public virtual int CombatLevel => Mathf.FloorToInt(enemyData.stats.str / 3f);
    public EProperty<int> MaxHp = new EProperty<int>();
    public EProperty<int> CurrentHp = new EProperty<int>();
    public EProperty<int> Animation = new EProperty<int>(); // 1=attack, 2=damaged, 3=dead
    public bool IsAlive => CurrentHp.Value > 0;

    public GameObject GameObject => view.gameObject;
    public EList<Buff> Buffs = new EList<Buff>();

    public Combatant(CombatantView view, EnemyData enemyData) {
        this.view = view;
        if (enemyData == null) { return; }
        this.enemyData = enemyData;
        CurrentHp.Value = enemyData.stats.maxHp;
        spriteId = enemyData.sprite;
        LoadStats(new Stats(enemyData));
        view.Init(this);
    }

    private void LoadStats(Stats stats) {
        baseStats = stats.Clone();
        MaxHp.Value = stats.maxHp;
        str = stats.str;
        resist = stats.resist;
    }

    public void TakeDamage(int amount) {
        if (CurrentHp.Value - amount >= MaxHp.Value) {
            CurrentHp.Value = MaxHp.Value;
        } else {
            CurrentHp.Value -= amount;
        }
        Animation.Value = CurrentHp.Value > 0 ? 2 : 3; // play dead if health is below zero
    }

    public void ApplyBuff(Buff buff) {
        var existingBuff = Buffs.List.FirstOrDefault(a => a.data.type == buff.data.type);
        if (existingBuff != null) {
            existingBuff.durationRemaining = Mathf.Max(existingBuff.durationRemaining, buff.durationRemaining);

        } else {
            Buffs.Add(buff);
            switch (buff.data.type) {
                case BuffType.pro: resist += Mathf.RoundToInt(baseStats.resist * BuffData.PRO_MOD); break;
                case BuffType.vul: resist -= Mathf.RoundToInt(baseStats.resist * BuffData.VUL_MOD); break;
                case BuffType.str: str += Mathf.RoundToInt(baseStats.resist * BuffData.STR_MOD); break;
                case BuffType.wea: str -= Mathf.RoundToInt(baseStats.resist * BuffData.WEA_MOD); break;
            }

        }
    }

    private void RemoveBuff(Buff buff) {
        Buffs.Remove(buff);

        switch (buff.data.type) {
            case BuffType.pro: resist -= Mathf.RoundToInt(baseStats.resist * BuffData.PRO_MOD); break;
            case BuffType.vul: resist += Mathf.RoundToInt(baseStats.resist * BuffData.VUL_MOD); break;
            case BuffType.str: str -= Mathf.RoundToInt(baseStats.resist * BuffData.STR_MOD); break;
            case BuffType.wea: str += Mathf.RoundToInt(baseStats.resist * BuffData.WEA_MOD); break;
        }
    }

    public void StartOfTurnBuffs() { 
        foreach (var b in Buffs.List) {
            if (b.data.isStartOfTurn) {
                b.durationRemaining--;
                if (b.durationRemaining <= 0) { RemoveBuff(b); }
            }
        }
    }

    public void EndOfTurnBuffs() {
        foreach (var b in Buffs.List) {
            if (!b.data.isStartOfTurn) {
                b.durationRemaining--;
                if (b.durationRemaining <= 0) { RemoveBuff(b); }
            }
        }
    }
    
}
