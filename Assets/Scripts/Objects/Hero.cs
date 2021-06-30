using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hero {

    public int currentHp;
    public int spriteId;
    public int level;
    public Stats stats;
    public EquipmentData lhEquipment;
    public EquipmentData rhEquipment;
    public List<CardData> deck;

    public Hero(HeroData data) {
        lhEquipment = Data.equipment[data.startingLHEquipment];
        rhEquipment = Data.equipment[data.startingRHEquipment];
        spriteId = data.sprite;
        stats = new Stats(data);
        currentHp = stats.maxHp;
        deck = data.startingDeck.Select(a => Data.cards[a]).ToList();
    }
}


