using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BuffType {
    str,
    wea,
    pro,
    vul,
}

public class BuffData {

    public static float STR_MOD = 0.5f;
    public static float WEA_MOD = 0.33333f;
    public static float PRO_MOD = 0.5f;
    public static float VUL_MOD = 0.33333f;

    public BuffType type;
    public string name;
    public string description;

    public BuffData(List<string> data) {
        type = (BuffType)Enum.Parse(typeof(BuffType), data[0]);
        name = data[1];
        description = data[2];
    }
}


