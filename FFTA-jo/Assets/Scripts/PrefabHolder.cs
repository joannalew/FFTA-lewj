using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour {
    public GameObject Tile;
    public GameObject CursorBase;
    public GameObject CursorTop;
    public GameObject Shadow;

    public GameObject battleUI;
    public GameObject leftUI;
    public GameObject rightUI;
    public GameObject sysUI;
    public GameObject damageUI;

    public GameObject[] Characters;


    private static PrefabHolder _instance;

    public static PrefabHolder Instance
    {
        get{ return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }

}
