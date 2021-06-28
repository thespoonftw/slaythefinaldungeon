using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CsvLoader {

    public static List<List<string>> LoadFile(string filename) {
        var returner = new List<List<string>>();
        var filepath = Application.streamingAssetsPath + "/Data/" + filename + ".csv";
        var filedata = System.IO.File.ReadAllText(filepath);
        var chars = new Char[2] { "\r"[0], "\n"[0] };
        var lines = filedata.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i<lines.Length; i++) {
            returner.Add(lines[i].Split(',').ToList());
        }
        return returner;
    }
    public static Dictionary<int, T> LoadAsType<T>(string filename) {
        var returner = new Dictionary<int, T>();
        var data = LoadFile(filename);
        foreach (var h in data) {
            var value = (T)Activator.CreateInstance(typeof(T), h);
            var key = int.Parse(h[0]);
            returner.Add(key, value);
        }
        return returner;
    }
}


