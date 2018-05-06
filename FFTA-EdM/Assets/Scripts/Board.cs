using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public List<Tile> map = new List<Tile>();
    protected BattleController owner;
    public GameObject mapObject;
    public GameObject objObject;
    public string str;

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }


    // Use this for initialization
    public void Load()
    {
        GameObject instance_1 = Instantiate(mapObject) as GameObject;
        GameObject instance_2 = Instantiate(objObject) as GameObject;
        XMLManager.LoadMap(map, mapObject, str);
    }
}
