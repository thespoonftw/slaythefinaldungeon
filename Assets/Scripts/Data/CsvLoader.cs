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
        try {
            var filedata = System.IO.File.ReadAllText(filepath);
            var chars = new Char[2] { "\r"[0], "\n"[0] };
            var lines = filedata.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++) {
                returner.Add(lines[i].Split(',').ToList());
            }
            return returner;
        } catch {
            GameMaster.Instance.SetCentreText("Unable to read " + filename + ". Is it open or missing?");
            return null;
        }        
    }
    public static Dictionary<int, T> LoadInt<T>(string filename) {
        Debug.Log("Attempting to load " + filename + " ...");
        var returner = new Dictionary<int, T>();
        var data = LoadFile(filename);
        for (int i = 0; i < data.Count; i++) {
            try {
                var value = (T)Activator.CreateInstance(typeof(T), data[i]);
                var key = int.Parse(data[i][0]);
                returner.Add(key, value);
            } catch {
                GameMaster.Instance.SetCentreText("Problem in " + filename + " line " + (i+2));
            }            
        }
        returner.Add(0, default);
        return returner;
    }

    public static Dictionary<string, T> LoadString<T>(string filename) {
        Debug.Log("Attempting to load " + filename + " ...");
        var returner = new Dictionary<string, T>();
        var data = LoadFile(filename);
        for (int i = 0; i < data.Count; i++) {
            var value = (T)Activator.CreateInstance(typeof(T), data[i]);
            var key = data[i][0];
            returner.Add(key, value);
        }
        return returner;
    }

    public static Dictionary<int, EncounterData> LoadEncounters(string filename) {
        Debug.Log("Attempting to load " + filename + " ...");
        var returner = new Dictionary<int, EncounterData>();
        var data = LoadFile(filename);
        int i = 0;
        while (i < data.Count) {
            var encounter = new EncounterData(data[i], data[i + 1], data[i + 2], data[i + 3]);
            returner.Add(encounter.id, encounter);
            i += 4;
        }
        return returner;
    }

    public static Dictionary<BuffType, BuffData> LoadBuffs(string filename) {
        Debug.Log("Attempting to load " + filename + " ...");
        var returner = new Dictionary<BuffType, BuffData>();
        var data = CsvLoader.LoadFile(filename);
        for (int i = 0; i < data.Count; i++) {
            var value = new BuffData(data[i]);
            BuffType key = (BuffType)Enum.Parse(typeof(BuffType), data[i][0]);
            returner.Add(key, value);
        }
        return returner;
    }
}


