using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tools : Singleton<Tools> {

    public delegate void Action();

    public static T RandomFromList<T>(List<T> input) {
        return input[UnityEngine.Random.Range(0, input.Count)];
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

    public static T ParseEnum<T>(string input) {
        return (T)Enum.Parse(typeof(T), input);
    }

    public static void LogError(string error) {
        Debug.LogError(error);
    }

    public static int RandomSign() {
        return UnityEngine.Random.Range(0, 2) * 2 - 1;
    }

    public static void IterateBackwards<T>(List<T> items, Action<T> action) {
        for (int i = items.Count - 1; i>=0; i--) {
            action(items[i]);
        }
    }
}


