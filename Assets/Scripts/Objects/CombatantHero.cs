using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatantHero : Combatant {

    public Hero hero;

    public override int CombatLevel => hero.level;

    public List<CardData> drawDeck;
    public List<CardData> discardPile = new List<CardData>();
    public List<CardData> hand = new List<CardData>();

    public CombatantHero(CombatantView view, Hero hero) : base(view, null, 0) {
        this.hero = hero;
        name = hero.name;
        drawDeck = new List<CardData>(hero.deck);
        CurrentHp.Value = hero.currentHp;
        spriteId = hero.spriteId;
        LoadStats(hero.stats);
        view.Init(this);
        isHero = true;
    }

    public void SaveHero() {
        hero.currentHp = CurrentHp.Value;
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
