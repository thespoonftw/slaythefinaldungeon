using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Buff {

    public Combatant source;
    public Combatant target;
    public int durationRemaining;
    public BuffData data;
    public BuffType Type => data.type;

    public Buff(Combatant source, Combatant target, int duration, BuffData data) {
        this.source = source;
        this.target = target;
        this.durationRemaining = duration;
        this.data = data;
    }
}


