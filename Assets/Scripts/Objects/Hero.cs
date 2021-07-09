using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hero {

    public string name;
    public int currentHp;
    public int spriteId;
    public int level;
    public int energy;
    public Stats stats;
    public EquipmentData lhEquipment;
    public EquipmentData rhEquipment;
    public List<CardData> deck;

    public Hero(HeroData data) {
        name = data.name;
        lhEquipment = Data.equipment[data.startingLHEquipment];
        rhEquipment = Data.equipment[data.startingRHEquipment];
        spriteId = data.sprite;
        stats = new Stats(data);
        currentHp = stats.maxHp;
        deck = data.startingDeck.Select(a => Data.cards[a]).ToList();
        level = 1;
        energy = 3;
    }
}


