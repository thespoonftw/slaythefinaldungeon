using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Helper : Singleton<Helper> {

    public static T RandomFromList<T>(List<T> input) {
        return input[Random.Range(0, input.Count - 1)];
    }

    private static IEnumerator WaitCoroutine(float timeInSeconds) {
        yield return new WaitForSeconds(timeInSeconds);
    }

    public static void WaitForTime(float timeInSeconds) {
        Instance.StartCoroutine(WaitCoroutine(timeInSeconds));
    }

}


