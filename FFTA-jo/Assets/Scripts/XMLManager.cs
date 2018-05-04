﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;


public static class XMLManager
{
    public static void LoadMap(int level, List<Tile> map, GameObject mapObject)
    {
        if (level == 1)
            mapObject.GetComponent<SpriteRenderer>().sprite = AssetHolder.Instance.GizaMap;
        else if (level == 2)
            mapObject.GetComponent<SpriteRenderer>().sprite = AssetHolder.Instance.Lutia1Map;
        else
            mapObject.GetComponent<SpriteRenderer>().sprite = AssetHolder.Instance.Lutia2Map;

        string filepath;
        if (level == 1)
            filepath = "Assets/Resources/giza1.xml";
        else if (level == 2)
            filepath = "Assets/Resources/lutia1.xml";
        else
            filepath = "Assets/Resources/lutia2.xml";

        XDocument xmlDoc = XDocument.Load(filepath);
        IEnumerable<XElement> tiles = xmlDoc.Descendants("tile");
        int objectCount = 0;

        foreach (var tile in tiles)
        {
            Tile myTile = ((GameObject)Transform.Instantiate(PrefabHolder.Instance.Tile)).GetComponent<Tile>();

            // translate XML doc data
            myTile.id = int.Parse(tile.Attribute("id").Value);
            myTile.height = int.Parse(tile.Element("height").Value);
            myTile.row = int.Parse(tile.Element("row").Value);
            myTile.col = int.Parse(tile.Element("column").Value);
            myTile.hasObj = int.Parse(tile.Element("object").Value) != 0;
            myTile.neighbors = new Tile[4] { null, null, null, null };

            myTile.occupied = int.Parse(tile.Element("occupy").Value);
            myTile.sort = 4 * (1000 - (myTile.row * 10 - myTile.col));       // 0 = tile, 1 = cursor, 2 = shadow, 3 = player

            // for A* pathfinding
            myTile.parent = null;
            myTile.cost = -1;

            // actual position in Unity
            float x = (myTile.row + myTile.col) * 1.0f;
            float y = (myTile.row + myTile.height - myTile.col) * 0.5f;
            float z = 0;
            myTile.transform.position = new Vector3(x, y, z);
            myTile.transform.parent = mapObject.transform;

            // subsprite rendering
            if (myTile.hasObj)
            {
                SpriteRenderer mySprite = myTile.GetComponent<SpriteRenderer>();

                if (level == 1)
                    mySprite.sprite = AssetHolder.Instance.GizaMapObjects[objectCount];
                else if (level == 2)
                    mySprite.sprite = AssetHolder.Instance.Lutia1MapObjects[objectCount];
                else
                    mySprite.sprite = AssetHolder.Instance.Lutia2MapObjects[objectCount];

                mySprite.sortingOrder = myTile.sort;
                objectCount++;
            }

            map.Add(myTile);
        }
        
        foreach (var tile in tiles)
        {
            int id = int.Parse(tile.Attribute("id").Value);

            if (int.Parse(tile.Element("left").Value) != -9)
                map[id].neighbors[0] = map[int.Parse(tile.Element("left").Value)];

            if (int.Parse(tile.Element("up").Value) != -9)
                map[id].neighbors[1] = map[int.Parse(tile.Element("up").Value)];

            if (int.Parse(tile.Element("right").Value) != -9)
                map[id].neighbors[2] = map[int.Parse(tile.Element("right").Value)];

            if (int.Parse(tile.Element("down").Value) != -9)
                map[id].neighbors[3] = map[int.Parse(tile.Element("down").Value)]; 
        }
    }

    public static void resetMap(List<Tile> map)
    {
        foreach (var tile in map)
        {
            tile.parent = null;
            tile.cost = -1;
        }
    }
}