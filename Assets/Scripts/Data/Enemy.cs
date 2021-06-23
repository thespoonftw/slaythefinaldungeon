using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Enemy {

    public int id;
    public string name;
    public int sprite;
    public int hp;
    public int str;

    public Enemy(List<string> data) {
        id = int.Parse(data[0]);
        name = data[1];
        sprite = int.Parse(data[2]);
        hp = int.Parse(data[3]);
        str = int.Parse(data[4]);
    }

}


