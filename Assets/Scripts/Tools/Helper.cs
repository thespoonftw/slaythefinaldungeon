using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Helper : Singleton<Helper> {

    public delegate void Action();

    public static T RandomFromList<T>(List<T> input) {
        return input[Random.Range(0, input.Count)];
    }

    private static IEnumerator WaitCoroutine(float timeInSeconds, Action action) {
        yield return new WaitForSeconds(timeInSeconds);
        action.Invoke();
    }

    public static void DelayMethod(float timeInSeconds, Action action) {
        Instance.StartCoroutine(WaitCoroutine(timeInSeconds, action));        
    }

    public static int ParseDataInt(List<string> data, int index) {
        if (data.Count < index) { return 0; }
        if (data[index] == null || data[index] == "") { return 0; }
        else { return int.Parse(data[index]); }
    }

}


