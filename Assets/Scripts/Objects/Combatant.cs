using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Combatant {

    public CombatantView view;
    public CharacterData data;
    public string name;
    public int str;
    public int magic;
    public float speedFactor;
    public int fireResistance;
    public int coldResistance;
    public int shockResistance;
    public int physicalResistance;
    public bool isUndead = false;

    public bool isHero = false;

    public delegate void DamageEvent(int amount, DamageType type);
    public event DamageEvent OnDamage;
    public EnemyData EnemyData => !isHero ? (EnemyData)data : null;
    public Hero HeroData => isHero ? (Hero)data : null;
    public EProperty<int> MaxHp = new EProperty<int>();
    public EProperty<int> CurrentHp = new EProperty<int>();
    public EProperty<int> Animation = new EProperty<int>(); // 1=attack, 2=damaged, 3=dead
    public bool IsAlive => CurrentHp.Value > 0;

    public GameObject GameObject => view.gameObject;
    public EList<Buff> Buffs = new EList<Buff>();

    public Combatant(CombatantView view, CharacterData data, int index) {
        // common
        this.data = data;
        this.view = view;
        MaxHp.Value = data.maxHp;
        str = data.str;
        magic = data.magic;
        speedFactor = 1f / data.speed;
        fireResistance = data.fireResistance;
        coldResistance = data.coldResistance;
        shockResistance = data.shockResistance;
        physicalResistance = data.physicalResistance;
        isUndead = data.isUndead;

        // below just for monsters
        if (!(data is EnemyData)) { return; }
        name = data.name + " " + index;
        CurrentHp.Value = data.maxHp;        
        view.Init(this);
    }

    public void TakeDamage(float amount, DamageType type) {

        var resistanceMultiplier = 1f;
        switch(type) {
            case DamageType.physical: { resistanceMultiplier = Mathf.Clamp(1 - (physicalResistance / 100f), 0.25f, 1f); break; }
            case DamageType.heal: { resistanceMultiplier = isUndead ? 1 : -1; break; }
            case DamageType.fire: { resistanceMultiplier = 1 - (fireResistance / 100f); break; }
            case DamageType.cold: { resistanceMultiplier = 1 - (coldResistance / 100f); break; }
            case DamageType.shock: { resistanceMultiplier = 1 - (shockResistance / 100f); break; }
        }
        var finalAmount = Mathf.RoundToInt(amount * resistanceMultiplier);
        OnDamage.Invoke(finalAmount, type);

        if (CurrentHp.Value - finalAmount >= MaxHp.Value) {
            CurrentHp.Value = MaxHp.Value;
        } else {
            CurrentHp.Value -= finalAmount;
        }

        if (CurrentHp.Value <= 0) {
            Animation.Value = 3;
            TurnCalculator.Instance.KillCombatant(this);
        } else {
            Animation.Value = 2;
        }
    }

    public void ApplyBuff(Buff buff) {
        var existingBuff = Buffs.List.FirstOrDefault(a => a.data.type == buff.data.type);
        if (existingBuff != null) {
            existingBuff.durationRemaining = Mathf.Max(existingBuff.durationRemaining, buff.durationRemaining);

        } else {
            Buffs.Add(buff);
            switch (buff.data.type) {
                case BuffType.pro: physicalResistance += 50; break;
                case BuffType.vul: physicalResistance -= 50; break;
                case BuffType.str: str += Mathf.RoundToInt(data.str * 0.5f); break;
                case BuffType.wea: str -= Mathf.RoundToInt(data.str * 0.333f); break;
                case BuffType.emp: magic += Mathf.RoundToInt(data.magic * 0.5f); break;
                case BuffType.dam: magic -= Mathf.RoundToInt(data.magic * 0.333f); break;
            }

        }
    }

    private void RemoveBuff(Buff buff) {
        Buffs.Remove(buff);
        switch (buff.data.type) {
            case BuffType.pro: physicalResistance -= 50; break;
            case BuffType.vul: physicalResistance += 50; break;
            case BuffType.str: str -= Mathf.RoundToInt(data.str * 0.5f); break;
            case BuffType.wea: str += Mathf.RoundToInt(data.str * 0.333f); break;
            case BuffType.emp: magic -= Mathf.RoundToInt(data.magic * 0.5f); break;
            case BuffType.dam: magic += Mathf.RoundToInt(data.magic * 0.333f); break;
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

    public int GetAttribute(ScalingAttribute attribute) {
        switch (attribute) {
            default: return 0;
            case ScalingAttribute.Strength: return str;
            case ScalingAttribute.Magic: return magic;
        }
    }
    
}
