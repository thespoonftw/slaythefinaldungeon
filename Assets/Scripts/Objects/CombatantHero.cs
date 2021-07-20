using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatantHero : Combatant {

    public List<CardData> drawDeck;
    public List<CardData> discardPile = new List<CardData>();
    public List<CardData> hand = new List<CardData>();

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

    public void DrawHand(int handSize) {
        for (int i=0; i<handSize; i++) { DrawCard(); }
    }

    public void DiscardHand() {
        discardPile.AddRange(hand);
        hand.Clear();
    }

    public void DrawCard() {
        if (drawDeck.Count == 0) {
            drawDeck = new List<CardData>(discardPile);
            discardPile.Clear();
        }
        var randomCard = drawDeck[Random.Range(0, drawDeck.Count)];
        hand.Add(randomCard);
        drawDeck.Remove(randomCard);
    }
    
}
