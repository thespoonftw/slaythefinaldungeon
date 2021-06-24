using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Helper : Singleton<Helper> {

    public delegate void Action();

    public static T RandomFromList<T>(List<T> input) {
        return input[Random.Range(0, input.Count - 1)];
    }

    private static IEnumerator WaitCoroutine(float timeInSeconds, Action action) {
        yield return new WaitForSeconds(timeInSeconds);
        action.Invoke();
    }

    public static void DelayAction(float timeInSeconds, Action action) {
        Instance.StartCoroutine(WaitCoroutine(timeInSeconds, action));        
    }

}


