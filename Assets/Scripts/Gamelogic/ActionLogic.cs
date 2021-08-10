using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ActionLogic {

    private Combatant CurrentCombatant => combatMaster.CurrentCombatant;
    private int ProgressIndex => combatMaster.progressIndex;
    private List<Combatant> LivingHeroes => combatMaster.LivingHeroes;
    private List<Combatant> ActiveMonsters => combatMaster.ActiveMonsters;
    private List<CombatantHero> CombatantHeroes => combatMaster.CombatantHeroes;
    private List<Combatant> ActiveCombatants => combatMaster.activeCombatants;
    public List<Combatant> AllMonsters => combatMaster.allMonsters;
    public List<CombatantHero> AllHeroes => combatMaster.allHeroes;

    public Tile[,] Tiles => combatMaster.tiles;

    private CombatController combatMaster;
    private CombatUIController combatUI;

    public ActionLogic() {
        combatMaster = CombatController.Instance;
        combatUI = CombatUIController.Instance;
    }        

    public IEnumerator PerformMonsterAction(CombatantMonster monster) {
        monster.Animation.Value = 1;

        MonsterActionData action;
        var dist = CurrentCombatant.x - ProgressIndex;
        if (dist > 3) {
            action = Data.monsterActions["advance"];
        } else if (dist > 1 && dist <= 3) {
            action = monster.CurrentRangedAction;
            monster.IncrementRangedIndex();
        } else {
            action = monster.CurrentMeleeAction;
            monster.IncrementMeleeIndex();
        }
        combatUI.SetEnemyActionBanner(action.id);

        Combatant target = null;
        if (monster.HasBuff(BuffType.tau)) {
            target = monster.GetBuff(BuffType.tau).source;
        } else if (action.targettingMode == MonsterActionTarget.RandomHero) {
            target = Tools.RandomFromList(LivingHeroes);
        } else if (action.targettingMode == MonsterActionTarget.HighestHero) {
            target = LivingHeroes.OrderByDescending(x => x.CurrentHp).First();
        } else if (action.targettingMode == MonsterActionTarget.LowestHero) {
            target = LivingHeroes.OrderByDescending(x => x.CurrentHp).Last();
        } else if (action.targettingMode == MonsterActionTarget.RandomMonster) {
            target = Tools.RandomFromList(ActiveMonsters);
        } else if (action.targettingMode == MonsterActionTarget.Infront) {
            var infront = Tiles[ProgressIndex, CurrentCombatant.y].occupant;
            if (ActiveCombatants.Contains(infront)) { target = infront; }
            else { target = Tools.RandomFromList(LivingHeroes); }
        }
        
        foreach (var a in action.actives) {
            yield return ActiveCoroutine(a, monster, target);
        }
        combatMaster.CheckForBattleEnd();
    }

    public IEnumerator ActiveCoroutine(Active a, Combatant source, Combatant target) {

        // non targetting actives
        if (a.type == ActiveType.wait) {
            yield return new WaitForSeconds(a.amount / 1000f);
            yield break;

        } else if (a.type == ActiveType.advance) {
            if (source.isHero) { yield return combatMaster.HeroesAdvanceCoroutine(); }
            else { MonsterMoveAction(CurrentCombatant, true); }            
            yield break;
        } else if (a.type == ActiveType.retreat) {
            if (!source.isHero) { MonsterMoveAction(CurrentCombatant, false); }
            yield break;
        }

        var targets = new List<Combatant>();
        if (a.targettingMode == ActiveTarget.Target) {
            targets.Add(target);
        } else if (a.targettingMode == ActiveTarget.Self) {
            targets.Add(source);
        } else if (a.targettingMode == ActiveTarget.AllHeroes) {
            targets.AddRange(LivingHeroes);
        } else if (a.targettingMode == ActiveTarget.AllMonsters) {
            targets.AddRange(ActiveMonsters);
        } else if (a.targettingMode == ActiveTarget.MonstersRow1) {
            targets.AddRange(ActiveMonsters.Where(m => m.x == ProgressIndex + 1));
        } else if (a.targettingMode == ActiveTarget.Adjacent) {
            var x = target.x;
            var y = target.y;
            if (y > 0) { targets.Add(Tiles[x, y - 1].occupant); }
            if (y < 2) { targets.Add(Tiles[x, y + 1].occupant); }
            if (x > 0) { targets.Add(Tiles[x - 1, y].occupant); }
            if (x < Tiles.GetLength(0)) { targets.Add(Tiles[x + 1, y].occupant); }
            targets.RemoveAll(i => i == null);
        }

        foreach (var t in targets) {
            if (a.type == ActiveType.dmg) {
                t.TakeDamage(a.amount * CurrentCombatant.GetAttribute(a.ScalingAttribute) / 10f, a.damageType);
            } else if (a.type == ActiveType.heal) {
                t.TakeDamage(a.amount * CurrentCombatant.magic / 10f, DamageType.heal);
            } else if (a.type == ActiveType.buff) {
                t.ApplyBuff(new Buff(source, target, a.amount, Data.buffs[a.buff]));
            } else if (a.type == ActiveType.push) {
                if (!t.isHero && Tiles[t.x + 1, t.y].occupant == null) { MoveToTile(t, t.x + 1, t.y); }
            } else if (a.type == ActiveType.pull) {
                if (!t.isHero && Tiles[t.x - 1, t.y].occupant == null) { MoveToTile(t, t.x - 1, t.y); }
            }
        }
    }        

    public void MonsterMoveAction(Combatant combatant, bool isForward) {
        var x = combatant.x;
        var y = combatant.y;       
        var mod = isForward ? 1 : -1;
        var randomSign = Tools.RandomSign();
        if (Tiles[x - mod, y].occupant == null) {
            MoveToTile(combatant, x - mod, y);
        } else if (y == 0 && Tiles[x - mod, y + 1].occupant == null) {
            MoveToTile(combatant, x - mod, y + 1);
        } else if (y == 2 && Tiles[x - mod, y - 1].occupant == null) {
            MoveToTile(combatant, x - mod, y - 1);
        } else if (y == 1 && Tiles[x - mod, y + randomSign].occupant == null) {
            MoveToTile(combatant, x - mod, y + randomSign);
        } else if (y == 1 && Tiles[x - mod, y - randomSign].occupant == null) {
            MoveToTile(combatant, x - mod, y - randomSign);
        }               
    }

    public void MoveToTile(Combatant combatant, int x, int y) {
        if (Tiles[x, y].occupant != null) { Debug.LogError("Tried to move onto a tile but it was blocked"); return; }
        if (!combatant.IsAlive) { return; }
        var currentX = combatant.x;
        var currentY = combatant.y;        
        combatant.x = x;
        combatant.y = y;
        Tiles[x, y].occupant = combatant;
        Tiles[currentX, currentY].occupant = null;
        var pos = Tiles[x, y].gameObject.transform.position;
        pos.z = 0;
        combatant.GameObject.transform.position = pos;
        combatUI.IsAdvanceAllowed.Value = combatMaster.CanHeroesAdvance;
    }
}
