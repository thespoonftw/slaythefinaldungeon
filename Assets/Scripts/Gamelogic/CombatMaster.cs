using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatMaster : Singleton<CombatMaster> {

    [SerializeField] List<GameObject> heroSpots;
    [SerializeField] List<GameObject> enemySpots;
    [SerializeField] GameObject characterPrefab;

    public bool IsVictorious => LivingMonsters.Count == 0;
    public bool IsDefeat => LivingHeroes.Count == 0;
    public List<Combatant> Combatants { get; set; }
    public List<Combatant> LivingHeroes => Combatants.Where(e => e.IsAlive && e.isHero).ToList();
    public List<Combatant> LivingMonsters => Combatants.Where(e => e.IsAlive && !e.isHero).ToList();
    public List<CombatantHero> CombatantHeroes => Combatants.Where(e => e.isHero).Cast<CombatantHero>().ToList();
    public Combatant CurrentCombatant { get; set; }

    public void Setup(int encounterIndex) {
        var heroes = GameMaster.Instance.heroes;        
        Combatants = new List<Combatant>();
        Combatants.Add(CreateHero(heroSpots[0], heroes[0]));
        Combatants.Add(CreateHero(heroSpots[1], heroes[1]));
        Combatants.Add(CreateHero(heroSpots[2], heroes[2]));
        var encounter = Data.encounters[encounterIndex];
        for (int i = 0; i < 4; i++) {
            if (encounter.enemies[i] == 0) { continue; }
            Combatants.Add(CreateEnemy(enemySpots[i], Data.enemies[encounter.enemies[i]], i+1));
        }
        TurnCalculator.Instance.Init();
        Tools.DelayMethod(1f, StartTurn);
    }

    private void StartTurn() {
        CurrentCombatant = TurnCalculator.Instance.TakeTurn();
        CurrentCombatant.StartOfTurnBuffs();
        if (!CurrentCombatant.isHero) {
            Tools.DelayMethod(0.5f, () => PerformAction(CurrentCombatant.enemyData.action, CurrentCombatant));
            Tools.DelayMethod(1.5f, EndTurn);
        } else {
            CombatUI.Instance.StartTurn();
        }
    }

    public void EndTurn() {        
        CurrentCombatant.EndOfTurnBuffs();
        StartTurn();
    }

    private void CheckForBattleEnd() {
        if (IsVictorious) {
            Tools.DelayMethod(1f, CleanUp);
            Tools.DelayMethod(1.5f, GameMaster.Instance.BattleWon);
        } else if (IsDefeat) {
            Tools.DelayMethod(1f, CleanUp);
            Tools.DelayMethod(1.5f, GameMaster.Instance.GameOver); 
        }
    }

    private void CleanUp() {
        CombatUI.Instance.DisableUI();
        foreach (var h in CombatantHeroes) { h.SaveHero(); }
        foreach (var c in Combatants) { Destroy(c.GameObject); }
    }

    public void PerformAction(ActionData data, Combatant source, Combatant target = null) {
        StartCoroutine(ActionCoroutine(data, source, target));
    }

    public IEnumerator ActionCoroutine(ActionData data, Combatant source, Combatant target = null) {
        source.Animation.Value = 1;
        //evaluate targets
       
        var randomMonster = Tools.RandomFromList(LivingMonsters);
        var randomHero = Tools.RandomFromList(LivingHeroes);
        
        foreach (var a in data.actives) {
            if (a.type == ActiveType.wait) {
                yield return new WaitForSeconds(a.amount / 1000f);
                continue;
            }

            var targets = new List<Combatant>();
            if (a.targettingType == TargettingType.Enemy || a.targettingType == TargettingType.Friendly || a.targettingType == TargettingType.Adjacent) {
                targets.Add(target);
            } else if (a.targettingType == TargettingType.RandomEnemy && CurrentCombatant.isHero) {
                targets.Add(randomMonster);
            } else if (a.targettingType == TargettingType.RandomEnemy && !CurrentCombatant.isHero) {
                targets.Add(randomHero);
            } else if (a.targettingType == TargettingType.Self) {
                targets.Add(source);
            } else if ((a.targettingType == TargettingType.AllFriendly && CurrentCombatant.isHero) || (a.targettingType == TargettingType.AllEnemies && !CurrentCombatant.isHero)) {
                targets.AddRange(LivingHeroes);
            } else if ((a.targettingType == TargettingType.AllFriendly && !CurrentCombatant.isHero) || (a.targettingType == TargettingType.AllEnemies && CurrentCombatant.isHero)) {
                targets.AddRange(LivingMonsters);
            } else if (a.targettingType == TargettingType.AllAdjacent && CurrentCombatant.isHero) {
                targets.AddRange(LivingHeroes);
                targets.Remove(source);
            } else if (a.targettingType == TargettingType.AllAdjacent && !CurrentCombatant.isHero) {
                targets.AddRange(LivingMonsters);
                targets.Remove(source);
            }
            foreach (var t in targets) {
                // do the active
                if (a.type == ActiveType.dmg) {
                    var dmg = Mathf.RoundToInt(a.amount * (CurrentCombatant.str * Mathf.Sqrt(CurrentCombatant.CombatLevel) / t.resist));
                    Debug.Log(a.amount + " * " + CurrentCombatant.str + " * SQRT(" + CurrentCombatant.CombatLevel + ") / " + t.resist + " = " + dmg);
                    t.TakeDamage(dmg);
                } else if (a.type == ActiveType.heal) {
                    t.TakeDamage(-a.amount);
                } else if (a.type == ActiveType.buff) {
                    t.ApplyBuff(new Buff(source, target, a.amount, Data.buffs[a.buff]));
                }
            }
        }
        CheckForBattleEnd();
        yield return null;
    }

    public void EquipmentStats(Hero hero, Passive passive) {
        if (passive.type == PassiveType.hp) {
            hero.stats.maxHp += passive.amount;
            hero.currentHp += passive.amount;
        } else if (passive.type == PassiveType.energy) {
            hero.energy += passive.amount;
        }
    }

    private Combatant CreateHero(GameObject spawnPoint, Hero data) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var view = go.GetComponent<CombatantView>();
        return new CombatantHero(view, data);
    }

    private Combatant CreateEnemy(GameObject spawnPoint, EnemyData data, int index) {
        var go = Instantiate(characterPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        var view = go.GetComponent<CombatantView>();
        return new Combatant(view, data, index);
    }
}
