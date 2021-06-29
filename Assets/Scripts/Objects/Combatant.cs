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
    public Hero hero;
    public EnemyData enemyData;

    public EProperty<int> MaxHp = new EProperty<int>();
    public EProperty<int> CurrentHp = new EProperty<int>();
    public EProperty<int> Animation = new EProperty<int>(); // 1=attack, 2=damaged, 3=dead
    public bool IsAlive => CurrentHp.Value > 0;
    public bool IsHero => (hero != null);
    public GameObject GameObject => view.gameObject;
    public List<Buff> buffs = new List<Buff>();

    public Combatant(CombatantView view, Hero hero) {
        this.view = view;
        this.hero = hero;
        view.Init();
        CurrentHp.Value = hero.currentHp;
        spriteId = hero.spriteId;
        LoadStats(hero.stats);
    }

    public Combatant(CombatantView view, EnemyData enemyData) {
        this.view = view;
        this.enemyData = enemyData;
        view.Init();
        CurrentHp.Value = enemyData.stats.maxHp;
        spriteId = enemyData.sprite;
        LoadStats(new Stats(enemyData));
    }

    private void LoadStats(Stats stats) {
        baseStats = stats.Clone();
        MaxHp.Value = stats.maxHp;
        str = stats.str;
        resist = stats.resist;
    }

    public void SaveHero() {
        hero.currentHp = CurrentHp.Value;
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
        var existingBuff = buffs.First(a => a.data.type == buff.data.type);
        if (existingBuff != null) {
            existingBuff.durationRemaining = Mathf.Max(existingBuff.durationRemaining, buff.durationRemaining);

        } else {
            buffs.Add(buff);
            switch (buff.data.type) {
                case BuffType.pro: resist += Mathf.RoundToInt(baseStats.resist * BuffData.PRO_MOD); break;
                case BuffType.vul: resist -= Mathf.RoundToInt(baseStats.resist * BuffData.VUL_MOD); break;
                case BuffType.str: str += Mathf.RoundToInt(baseStats.resist * BuffData.STR_MOD); break;
                case BuffType.wea: str -= Mathf.RoundToInt(baseStats.resist * BuffData.WEA_MOD); break;
            }

        }
    }

    private void RemoveBuff(Buff buff) {
        buffs.Remove(buff);

        switch (buff.data.type) {
            case BuffType.pro: resist -= Mathf.RoundToInt(baseStats.resist * BuffData.PRO_MOD); break;
            case BuffType.vul: resist += Mathf.RoundToInt(baseStats.resist * BuffData.VUL_MOD); break;
            case BuffType.str: str -= Mathf.RoundToInt(baseStats.resist * BuffData.STR_MOD); break;
            case BuffType.wea: str += Mathf.RoundToInt(baseStats.resist * BuffData.WEA_MOD); break;
        }
    }

    public void StartOfTurnBuffs() { 
        foreach (var b in buffs.ToArray()) {
            b.durationRemaining--;
            if (b.durationRemaining <= 0) { RemoveBuff(b); }
        }
    }

    public void EndOfTurnBuffs() {
        foreach (var b in buffs.ToArray()) {
            if (b.durationRemaining <= 0) { RemoveBuff(b); }
        }
    }
    
}
