using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatMaster : Singleton<CombatMaster> {

    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject tilePrefab;

    public Tile[,] tiles;

    public List<Combatant> allMonsters = new List<Combatant>();
    public List<CombatantHero> allHeroes = new List<CombatantHero>();
    public int progressIndex = 0;
    public List<Combatant> activeCombatants = new List<Combatant>();

    private TurnCalculator turnCalculator;

    private static int ENGAGEMENT_RANGE = 4;

    //public bool IsVictorious => LivingMonsters.Count == 0;
    public bool IsDefeat => LivingHeroes.Count == 0;
    
    public List<Combatant> LivingHeroes => activeCombatants.Where(e => e.IsAlive && e.isHero).ToList();
    public List<Combatant> ActiveMonsters => activeCombatants.Where(e => e.IsAlive && !e.isHero).ToList();
    public List<CombatantHero> CombatantHeroes => activeCombatants.Where(e => e.isHero).Cast<CombatantHero>().ToList();
    public Combatant CurrentCombatant { get; set; }

    private CombatUI combatUI;

    private void Awake() {
        combatUI = CombatUI.Instance;        
    }

    public void Setup(int encounterIndex) {
        var heroData = GameMaster.Instance.heroes;

        var encounter = Data.encounters[encounterIndex];
        tiles = new Tile[encounter.length,3];
        for (int y=0; y<3; y++) {
            for (int x=0; x<encounter.length; x++) {
                var tileGo = Instantiate(tilePrefab, new Vector3(x * 2.5f, y * 2.5f, 0.5f), Quaternion.identity, transform);
                tiles[x, y] = new Tile(x, y, tileGo);
                if (encounter.monsters[x, y] != 0) { allMonsters.Add(CreateMonster(Data.monsters[encounter.monsters[x, y]], x, y)); }
            }
        }

        allHeroes.Add(CreateHero(heroData[0], 0, 0));
        allHeroes.Add(CreateHero(heroData[1], 0, 1));
        allHeroes.Add(CreateHero(heroData[2], 0, 2));
        activeCombatants.AddRange(allHeroes);

        turnCalculator = TurnCalculator.Instance;
        turnCalculator.Init();

        AddNewActiveMonsters();        
        Tools.DelayMethod(1f, StartTurn);
    }

    private void StartTurn() {
        CurrentCombatant = turnCalculator.GetCurrentTurn();        
        CurrentCombatant.StartOfTurnBuffs();
        combatUI.MoveActive();
        if (!CurrentCombatant.isHero) {
            StartCoroutine(PerformMonsterAction((CombatantMonster)CurrentCombatant));
            Tools.DelayMethod(1.5f, EndTurn);
        } else {
            combatUI.StartTurn();
        }
        combatUI.UpdateAdvanceButton();
    }

    public void EndTurn() {
        combatUI.SetEnemyActionBanner(null);
        CurrentCombatant.EndOfTurnBuffs();
        turnCalculator.RemoveTurn();
        StartTurn();
    }

    private void CheckForBattleEnd() {
        if (IsDefeat) {
            Tools.DelayMethod(1f, CleanUp);
            Tools.DelayMethod(1.5f, GameMaster.Instance.GameOver); 
        }
    }

    private void CleanUp() {
        combatUI.DisableUI();
        foreach (var h in CombatantHeroes) { h.SaveHero(); }
        foreach (var c in activeCombatants) { Destroy(c.GameObject); }
    }

    public IEnumerator PerformHeroAction(HeroActionData action, Combatant source, Combatant target = null) {
        source.Animation.Value = 1;
        foreach (var a in action.actives) {
            yield return ActiveCoroutine(a, source, target);
        }
        CheckForBattleEnd();
    }

    public IEnumerator PerformMonsterAction(CombatantMonster monster) {
        monster.Animation.Value = 1;

        MonsterActionData action;
        var dist = CurrentCombatant.x - progressIndex;
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
            var infront = tiles[progressIndex, CurrentCombatant.y].occupant;
            if (activeCombatants.Contains(infront)) { target = infront; }
            else { target = Tools.RandomFromList(LivingHeroes); }
        }
        
        foreach (var a in action.actives) {
            yield return ActiveCoroutine(a, monster, target);
        }
        CheckForBattleEnd();
    }

    public IEnumerator ActiveCoroutine(Active a, Combatant source, Combatant target) {

        // non targetting actives
        if (a.type == ActiveType.wait) {
            yield return new WaitForSeconds(a.amount / 1000f);
            yield break;

        } else if (a.type == ActiveType.advance) {
            if (source.isHero) { yield return HeroesAdvanceCoroutine(); }
            else { AdvanceCombatant(CurrentCombatant); }            
            yield break;
        } else if (a.type == ActiveType.retreat) {
            if (!source.isHero) { AdvanceCombatant(CurrentCombatant, true); }
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
            targets.AddRange(ActiveMonsters.Where(m => m.x == progressIndex + 1));
        } else if (a.targettingMode == ActiveTarget.Adjacent) {
            var x = target.x;
            var y = target.y;
            if (y > 0) { targets.Add(tiles[x, y - 1].occupant); }
            if (y < 2) { targets.Add(tiles[x, y + 1].occupant); }
            if (x > 0) { targets.Add(tiles[x - 1, y].occupant); }
            if (x < tiles.GetLength(0)) { targets.Add(tiles[x + 1, y].occupant); }
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
                if (!t.isHero && tiles[t.x + 1, t.y].occupant == null) { MoveToTile(t, t.x + 1, t.y); }
            } else if (a.type == ActiveType.pull) {
                if (!t.isHero && tiles[t.x - 1, t.y].occupant == null) { MoveToTile(t, t.x - 1, t.y); }
            }
        }
    }

    public void EquipmentStats(Hero hero, Passive passive) {
        if (passive.type == PassiveType.hp) {
            hero.maxHp += passive.amount;
            hero.currentHp += passive.amount;
        } else if (passive.type == PassiveType.energy) {
            hero.maxEnergy += passive.amount;
        }
    }

    private CombatantHero CreateHero(Hero data, int x, int y) {
        var pos = tiles[x, y].gameObject.transform.position;
        pos.z = 0;
        var go = Instantiate(characterPrefab, pos, Quaternion.identity);
        var view = go.GetComponent<CombatantView>();
        var returner = new CombatantHero(view, data, x, y);
        tiles[x, y].occupant = returner;
        return returner;
    }

    private Combatant CreateMonster(MonsterData data, int x, int y) {
        var pos = tiles[x, y].gameObject.transform.position;
        pos.z = 0;
        var go = Instantiate(characterPrefab, pos, Quaternion.identity);
        var view = go.GetComponent<CombatantView>();
        var returner = new CombatantMonster(view, data, x, y);
        tiles[x, y].occupant = returner;
        return returner;
    }

    public IEnumerator RepeatHeroesAdvance() {
        yield return HeroesAdvanceCoroutine();
        while (ActiveMonsters.Count == 0) {
            yield return HeroesAdvanceCoroutine();
        }
        EndTurn();
        yield return null;
    }

    public IEnumerator HeroesAdvanceCoroutine() {
        if (!CanHeroesAdvance()) { yield break; }
        yield return new WaitForSeconds(0.5f);
        progressIndex++;
        AdvanceCombatant(allHeroes[0]);
        AdvanceCombatant(allHeroes[1]);
        AdvanceCombatant(allHeroes[2]);        
        Camera.main.transform.parent.position = new Vector3(2.5f * progressIndex, 0, 0);        
    }

    public void AdvanceCombatant(Combatant combatant, bool isRetreat = false) {
        var x = combatant.x;
        var y = combatant.y;       
        if (combatant.isHero) {
            var mod = isRetreat ? -1 : 1;
            MoveToTile(combatant, x + mod, y);

        } else {
            var mod = isRetreat ? -1 : 1;
            var randomSign = Tools.RandomSign();
            if (tiles[x - mod, y].occupant == null) {
                MoveToTile(combatant, x - mod, y);
            } else if (y == 0 && tiles[x - mod, y + 1].occupant == null) {
                MoveToTile(combatant, x - mod, y + 1);
            } else if (y == 2 && tiles[x - mod, y - 1].occupant == null) {
                MoveToTile(combatant, x - mod, y - 1);
            } else if (y == 1 && tiles[x - mod, y + randomSign].occupant == null) {
                MoveToTile(combatant, x - mod, y + randomSign);
            } else if (y == 1 && tiles[x - mod, y - randomSign].occupant == null) {
                MoveToTile(combatant, x - mod, y - randomSign);
            }
        }        
        AddNewActiveMonsters();        
    }

    private void MoveToTile(Combatant combatant, int x, int y) {
        if (tiles[x, y].occupant != null) { Debug.LogError("Tried to move onto a tile but it was blocked"); return; }
        if (!combatant.IsAlive) { return; }
        var currentX = combatant.x;
        var currentY = combatant.y;        
        combatant.x = x;
        combatant.y = y;
        tiles[x, y].occupant = combatant;
        tiles[currentX, currentY].occupant = null;
        var pos = tiles[x, y].gameObject.transform.position;
        pos.z = 0;
        combatant.GameObject.transform.position = pos;
        combatUI.UpdateAdvanceButton();
    }

    private void AddNewActiveMonsters(int recursiveIndex = 0) {
        var i = ENGAGEMENT_RANGE + progressIndex + recursiveIndex - 1;
        var newMonsters = allMonsters.Where(e => e.x <= i && !activeCombatants.Contains(e));
        foreach (var e in newMonsters) { turnCalculator.AddCombatant(e); }
        activeCombatants.AddRange(newMonsters);
        if (tiles[i,0].occupant != null || tiles[i, 1].occupant != null || tiles[i, 2].occupant != null) { AddNewActiveMonsters(recursiveIndex + 1); }
    }

    public void KillCombatant(Combatant combatant) {
        if (combatant == CurrentCombatant) { combatUI.UnparentActive(); }
        turnCalculator.KillCombatant(combatant);
        tiles[combatant.x, combatant.y].occupant = null;

        if (!combatant.isHero) {
            combatUI.UpdateAdvanceButton();
            Tools.DelayMethod(0.5f, () => Destroy(combatant.GameObject));
        }        
    }

    public bool CanHeroesAdvance() {
        var i = progressIndex + 1;
        return tiles[i, 0].occupant == null && tiles[i, 1].occupant == null && tiles[i, 2].occupant == null;
    }
}
