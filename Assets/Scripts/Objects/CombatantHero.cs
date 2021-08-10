using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatantHero : Combatant {

    public List<CardData> drawDeck;
    public List<CardData> discardPile = new List<CardData>();    

    public CombatantHero(CombatantView view, Hero data, int x, int y) : base(view, data, x, y) {
        name = data.name;
        drawDeck = new List<CardData>(data.deck);
        CurrentHp.Value = data.currentHp;
        view.Init(this);
        isHero = true;
    }

    public void SaveHero() {
        HeroData.currentHp = CurrentHp.Value;
    }    
}
