using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tile {

    public GameObject gameObject;
    public Combatant occupant;
    public int x;
    public int y;

    public Tile(int x, int y, GameObject gameObject) {
        this.x = x;
        this.y = y;
        this.gameObject = gameObject;
    }
}
