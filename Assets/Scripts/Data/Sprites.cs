﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Sprites : MonoBehaviour {

    public static Dictionary<int, Sprite> dictionary;
    private static Sprites instance;

    public static void Init() {
        var go = new GameObject("Sprites");
        instance = go.AddComponent<Sprites>();
        instance.Load();
    }

    private void Load() {
        dictionary = new Dictionary<int, Sprite>();
        var path = Application.streamingAssetsPath + "/Sprites";
        var files = Directory.GetFiles(path);
        foreach (var f in files) {
            if (f.EndsWith(".meta")) { continue; }
            StartCoroutine("LoadSprite", f);
        }
    }


    private IEnumerator LoadSprite(string path) {
        var backslash = new char[] { '\\' };
        var split1 = path.Split(backslash, StringSplitOptions.RemoveEmptyEntries)[1];
        var split2 = split1.Split(' ')[0];
        var id = int.Parse(split2);
        var wwwPath = "file://" + path;
        var www = new WWW(wwwPath);
        var texture = www.texture;
        var sprite =  Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        dictionary.Add(id, sprite);
        yield return sprite;
    }
}

