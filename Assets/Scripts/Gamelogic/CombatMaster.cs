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

    public bool IsDefeat => LivingHeroes.Count == 0;
    
    public List<Combatant> LivingHeroes => activeCombatants.Where(e => e.IsAlive && e.isHero).ToList();
    public List<Combatant> ActiveMonsters => activeCombatants.Where(e => e.IsAlive && !e.isHero).ToList();
    public List<CombatantHero> CombatantHeroes => activeCombatants.Where(e => e.isHero).Cast<CombatantHero>().ToList();
    public Combatant CurrentCombatant { get; set; }
    public bool CanHeroesAdvance => tiles[progressIndex + 1, 0].occupant == null && tiles[progressIndex + 1, 1].occupant == null && tiles[progressIndex + 1, 2].occupant == null;

    private CombatUI combatUI;
    private ActionLogic actionLogic;

    private void Awake() {
        combatUI = CombatUI.Instance;
        actionLogic = new ActionLogic();
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
            StartCoroutine(actionLogic.PerformMonsterAction((CombatantMonster)CurrentCombatant));
            Tools.DelayMethod(1.5f, EndTurn);
        } else {
            combatUI.StartHeroTurn();
        }
    }

    public void EndTurn() {
        combatUI.SetEnemyActionBanner(null);
        CurrentCombatant.EndOfTurnBuffs();
        turnCalculator.RemoveTurn();
        StartTurn();
    }

    public void CheckForBattleEnd() {
        if (IsDefeat) {
            Tools.DelayMethod(1f, CleanUp);
            Tools.DelayMethod(1.5f, GameMaster.Instance.GameOver); 
        }
    }

    private void CleanUp() {
        combatUI.EndHeroTurn();
        foreach (var h in CombatantHeroes) { h.SaveHero(); }
        foreach (var c in activeCombatants) { Destroy(c.GameObject); }
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

    public void AddNewActiveMonsters(int recursiveIndex = 0) {
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
            combatUI.IsAdvanceAllowed.Value = CanHeroesAdvance;
            Tools.DelayMethod(0.5f, () => Destroy(combatant.GameObject));
        }        
    }

    public IEnumerator PerformHeroAction(HeroActionData action, Combatant source, Combatant target = null) {
        source.Animation.Value = 1;
        foreach (var a in action.actives) {
            yield return actionLogic.ActiveCoroutine(a, source, target);
        }
        CheckForBattleEnd();
    }   

    public IEnumerator HeroesAdvanceCoroutine() {
        if (!CanHeroesAdvance) { yield break; }
        yield return new WaitForSeconds(0.5f);
        progressIndex++;
        HeroAdvance(allHeroes[0]);
        HeroAdvance(allHeroes[1]);
        HeroAdvance(allHeroes[2]);        
        Camera.main.transform.parent.position = new Vector3(2.5f * progressIndex, 0, 0);        
    }

    public void HeroAdvance(Combatant hero) {
        var x = hero.x;
        var y = hero.y;
        actionLogic.MoveToTile(hero, x + 1, y);
        AddNewActiveMonsters();
    }

    public IEnumerator RepeatHeroesAdvance() {
        yield return HeroesAdvanceCoroutine();
        while (ActiveMonsters.Count == 0) {
            yield return HeroesAdvanceCoroutine();
        }
        EndTurn();
        yield return null;
    }
}
