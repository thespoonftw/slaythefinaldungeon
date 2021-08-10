using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hero : CharacterData {

    public int currentHp;
    public int level;
    public int maxEnergy;
    public int cunning;
    public EquipmentData lhEquipment;
    public EquipmentData rhEquipment;
    public List<CardData> deck;

    public Hero(HeroData data) {
        name = data.name;
        maxHp = data.maxHp;
        currentHp = data.maxHp;
        str = data.str;
        magic = data.magic;
        speed = data.speed;
        lhEquipment = Data.equipment[data.startingLHEquipment];
        rhEquipment = Data.equipment[data.startingRHEquipment];
        spriteId = data.spriteId;
        deck = data.startingDeck.Select(a => Data.cards[a]).ToList();
        level = 1;
        maxEnergy = 3;
        cunning = 3;
    }
}


