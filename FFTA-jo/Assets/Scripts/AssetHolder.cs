using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour {

    public Sprite[] Lutia2MapObjects;

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
