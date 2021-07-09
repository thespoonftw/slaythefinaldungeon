using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Stats {

    public int str;
    public int maxHp;
    public int resist;
    public int speed;
    
    public Stats(CharacterData data) {
        str = data.stats.str;
        maxHp = data.stats.maxHp;
        resist = data.stats.resist;
        speed = data.stats.speed;
    }

    public Stats(int maxHp, int str, int resist, int speed) {
        this.str = str;
        this.maxHp = maxHp;
        this.resist = resist;
        this.speed = speed;
    }

    public Stats Clone() => new Stats(maxHp, str, resist, speed);

}


