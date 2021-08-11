using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIController : Singleton<CombatUIController> {

    [SerializeField] HandController cardsView;
    [SerializeField] GameObject activeHeroSelector;
    [SerializeField] GameObject enemyActionBanner;    
    [SerializeField] GameObject tooltip;
    [SerializeField] GameObject turnOrderHighlight;

    private CombatController combatController;
    private HandController handController;

    public EProperty<bool> IsHeroUIEnabled = new EProperty<bool>();
    public EProperty<bool> IsAdvanceAllowed = new EProperty<bool>();
    public EProperty<int> EnergyRemaining = new EProperty<int>();
    public EProperty<int> EnergyMax = new EProperty<int>();
    public EProperty<int> DrawSize = new EProperty<int>();
    public EProperty<int> DiscardSize = new EProperty<int>();
    public EProperty<Combatant> HoveredCombatant = new EProperty<Combatant>();

    private CombatantHero CurrentHero => combatController.CurrentCombatant.isHero ? (CombatantHero)combatController.CurrentCombatant : null;


    private void Start() {
        combatController = CombatController.Instance;
        handController = HandController.Instance;
        tooltip.SetActive(false);
    }

    public void UseCard(HeroActionData action, Combatant target) {
        StartCoroutine(combatController.PerformHeroAction(action, CurrentHero, target));
        EnergyRemaining.Value -= action.energyCost;
    }

    public void UnparentActive() {
        activeHeroSelector.transform.parent = null;
    }

    public void MoveActive() {        
        activeHeroSelector.SetActive(true);
        activeHeroSelector.transform.position = combatController.CurrentCombatant.GameObject.transform.position;
        activeHeroSelector.transform.parent = combatController.CurrentCombatant.GameObject.transform;
    }

    public void StartHeroTurn() {
        handController.StartHandTurn(CurrentHero, CurrentHero.HeroData.cunning);
        IsHeroUIEnabled.Value = true;
        IsAdvanceAllowed.Value = combatController.CanHeroesAdvance;
        EnergyRemaining.Value = CurrentHero.HeroData.maxEnergy;
        EnergyMax.Value = CurrentHero.HeroData.maxEnergy;        
    }

    public void EndHeroTurn() {
        handController.EndHandTurn();
        IsHeroUIEnabled.Value = false;
        tooltip.SetActive(false);
        activeHeroSelector.SetActive(false);
    }

    public void TurnOrderHighlight(Combatant combatant) {
        if (combatant != null) {
            turnOrderHighlight.SetActive(true);
            turnOrderHighlight.transform.position = combatant.GameObject.transform.position + new Vector3(0, 0, -1);
        } else {
            turnOrderHighlight.SetActive(false);
        }
    }


    public void SetTooltip(string text) {
        //if (text == null || currentCard != null) {
        if (text == null) {
            tooltip.SetActive(false);
        } else {
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = text;
            tooltip.SetActive(true);
        }
    }

    public void SetEnemyActionBanner(string text) {
        if (text == null || text == "") {
            enemyActionBanner.SetActive(false);
        } else {
            enemyActionBanner.SetActive(true);
            enemyActionBanner.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }
    }

}
