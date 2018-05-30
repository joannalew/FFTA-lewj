using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour {

    public Sprite GizaMap;
    public Sprite[] GizaMapObjects;
    public Sprite Lutia1Map;
    public Sprite[] Lutia1MapObjects;
    public Sprite Lutia2Map;
    public Sprite[] Lutia2MapObjects;
    public Sprite[] CharPortraits;

    private static AssetHolder _instance;

    public static AssetHolder Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }
}
