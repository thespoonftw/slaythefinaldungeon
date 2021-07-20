using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatMaster : Singleton<CombatMaster> {

    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject tilePrefab;

    public Tile[,] tiles;

    public List<Combatant> allEnemies = new List<Combatant>();
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

    public void Setup(int encounterIndex) {
        var heroData = GameMaster.Instance.heroes;

        var encounter = Data.encounters[encounterIndex];
        tiles = new Tile[encounter.length,3];
        for (int y=0; y<3; y++) {
            for (int x=0; x<encounter.length; x++) {
                var tileGo = Instantiate(tilePrefab, new Vector3(x * 2.5f, y * 2.5f, 0.5f), Quaternion.identity, transform);
                tiles[x, y] = new Tile(x, y, tileGo);
                if (encounter.enemies[x, y] != 0) { allEnemies.Add(CreateEnemy(Data.enemies[encounter.enemies[x, y]], x, y)); }
            }
        }

        allHeroes.Add(CreateHero(heroData[0], 0, 0));
        allHeroes.Add(CreateHero(heroData[1], 0, 1));
        allHeroes.Add(CreateHero(heroData[2], 0, 2));
        activeCombatants.AddRange(allHeroes);

        turnCalculator = TurnCalculator.Instance;
        turnCalculator.Init();

        AddNewActiveEnemies();        
        Tools.DelayMethod(1f, StartTurn);
    }

    private void StartTurn() {
        CurrentCombatant = turnCalculator.TakeTurn();
        CurrentCombatant.StartOfTurnBuffs();
        CombatUI.Instance.MoveActive();
        if (!CurrentCombatant.isHero) {
            EnemyActionData enemyAction = null;
            string actionIndex = "advance";
            if (CurrentCombatant.x <= progressIndex + 3) {
                actionIndex = (CurrentCombatant.x < progressIndex + 2) ? CurrentCombatant.EnemyData.meleeActionId : CurrentCombatant.EnemyData.rangedActionId;
            }
            enemyAction = Data.enemyActions[actionIndex];
            Combatant target = null;
            if (enemyAction.aiTargetting == AiTargettingMode.random) {
                target = Tools.RandomFromList(LivingHeroes);
            } else if (enemyAction.aiTargetting == AiTargettingMode.infront) {
                target = tiles[progressIndex, CurrentCombatant.y].occupant;
            }
            Tools.DelayMethod(0.5f, () => PerformAction(enemyAction.action, CurrentCombatant, target));
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
        if (IsDefeat) {
            Tools.DelayMethod(1f, CleanUp);
            Tools.DelayMethod(1.5f, GameMaster.Instance.GameOver); 
        }
    }

    private void CleanUp() {
        CombatUI.Instance.DisableUI();
        foreach (var h in CombatantHeroes) { h.SaveHero(); }
        foreach (var c in activeCombatants) { Destroy(c.GameObject); }
    }

    public void PerformAction(ActionData data, Combatant source, Combatant target = null) {
        StartCoroutine(ActionCoroutine(data, source, target));
    }

    public IEnumerator ActionCoroutine(ActionData action, Combatant source, Combatant target = null) {
        source.Animation.Value = 1;
        //evaluate targets
       
        var randomMonster = ActiveMonsters.Count > 0 ? Tools.RandomFromList(ActiveMonsters) : null;
        var randomHero = LivingHeroes.Count > 0 ? Tools.RandomFromList(LivingHeroes) : null;
        
        foreach (var a in action.actives) {
            if (a.type == ActiveType.wait) {
                yield return new WaitForSeconds(a.amount / 1000f);
                continue;
            }

            var targets = new List<Combatant>();
            if (a.targettingMode == TargettingMode.Target) {
                targets.Add(target);
            } else if (a.targettingMode == TargettingMode.RandomEnemy && CurrentCombatant.isHero) {
                targets.Add(randomMonster);
            } else if (a.targettingMode == TargettingMode.RandomEnemy && !CurrentCombatant.isHero) {
                targets.Add(randomHero);
            } else if (a.targettingMode == TargettingMode.Self) {
                targets.Add(source);
            } else if ((a.targettingMode == TargettingMode.AllFriendly && CurrentCombatant.isHero) || (a.targettingMode == TargettingMode.AllEnemies && !CurrentCombatant.isHero)) {
                targets.AddRange(LivingHeroes);
            } else if ((a.targettingMode == TargettingMode.AllFriendly && !CurrentCombatant.isHero) || (a.targettingMode == TargettingMode.AllEnemies && CurrentCombatant.isHero)) {
                targets.AddRange(ActiveMonsters);
            } else if ((a.targettingMode == TargettingMode.AllMelee) && CurrentCombatant.isHero) {
                targets.AddRange(ActiveMonsters.Where(c => c.x <= progressIndex + 1));
            } else if (a.targettingMode == TargettingMode.AllOtherFriendly && CurrentCombatant.isHero) {
                targets.AddRange(LivingHeroes);
                targets.Remove(source);
            } else if (a.targettingMode == TargettingMode.AllOtherFriendly && !CurrentCombatant.isHero) {
                targets.AddRange(ActiveMonsters);
                targets.Remove(source);
            }
            foreach (var t in targets) {
                // nullable actives
                if (a.type == ActiveType.advance) {
                    AdvanceCombatant(CurrentCombatant);
                } else if (t == null) {
                    continue;
                // non-nullable actives
                } else if (a.type == ActiveType.dmg) {
                    t.TakeDamage(a.amount * CurrentCombatant.GetAttribute(a.ScalingAttribute) / 10f, a.damageType);
                } else if (a.type == ActiveType.heal) {
                    t.TakeDamage(a.amount * CurrentCombatant.magic / 10f, DamageType.heal);
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

    private Combatant CreateEnemy(EnemyData data, int x, int y) {
        var pos = tiles[x, y].gameObject.transform.position;
        pos.z = 0;
        var go = Instantiate(characterPrefab, pos, Quaternion.identity);
        var view = go.GetComponent<CombatantView>();
        var returner = new Combatant(view, data, x, y);
        tiles[x, y].occupant = returner;
        return returner;
    }

    public void HeroesAdvance() {
        progressIndex++;
        AdvanceCombatant(allHeroes[0]);
        AdvanceCombatant(allHeroes[1]);
        AdvanceCombatant(allHeroes[2]);        
        Camera.main.transform.parent.position = new Vector3(2.5f * progressIndex, 0, 0);
    }

    public void AdvanceCombatant(Combatant combatant) {
        var x = combatant.x;
        var y = combatant.y;
        tiles[x, y].occupant = null;
        if (combatant.isHero) {
            combatant.x = x + 1;
            tiles[x + 1, y].occupant = combatant;
            var pos = tiles[x + 1, y].gameObject.transform.position;
            pos.z = 0;
            combatant.GameObject.transform.position = pos;

        } else if (tiles[x-1,y].occupant == null) {
            combatant.x = x - 1;
            tiles[x - 1, y].occupant = combatant;
            var pos = tiles[x - 1, y].gameObject.transform.position;
            pos.z = 0;
            combatant.GameObject.transform.position = pos;
        }        
        AddNewActiveEnemies();        
    }

    private void AddNewActiveEnemies(int recursiveIndex = 0) {
        var i = ENGAGEMENT_RANGE + progressIndex + recursiveIndex - 1;
        var newEnemies = allEnemies.Where(e => e.x <= i && !activeCombatants.Contains(e));
        foreach (var e in newEnemies) { turnCalculator.AddCombatant(e); }
        activeCombatants.AddRange(newEnemies);
        if (tiles[i,0].occupant != null || tiles[i, 1].occupant != null || tiles[i, 2].occupant != null) { AddNewActiveEnemies(recursiveIndex + 1); }
    }

    public void KillEnemy(Combatant combatant) {
        turnCalculator.KillCombatant(combatant);
        tiles[combatant.x, combatant.y].occupant = null;
        CombatUI.Instance.UpdateAdvanceButton();
        Tools.DelayMethod(0.5f, () => Destroy(combatant.GameObject));        
    }

    public void KillHero(CombatantHero combatant) {
        turnCalculator.KillCombatant(combatant);
        tiles[combatant.x, combatant.y].occupant = null;
    }
}
