using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;


public static class XMLManager
{
    public static void LoadMap(List<Tile> map, GameObject mapObject)
    {
        XDocument xmlDoc = XDocument.Load("Assets/Resources/giza1.xml");
        IEnumerable<XElement> tiles = xmlDoc.Descendants("tile");

        foreach (var tile in tiles)
        {
            Tile myTile = ((GameObject)Transform.Instantiate(PrefabHolder.Instance.Tile)).GetComponent<Tile>();

            myTile.id = int.Parse(tile.Attribute("id").Value);
            myTile.height = int.Parse(tile.Element("height").Value);
            myTile.row = int.Parse(tile.Element("row").Value);
            myTile.col = int.Parse(tile.Element("column").Value);
            myTile.hasObj = int.Parse(tile.Element("object").Value) != 0;
            myTile.neighbors = new Tile[4] { null, null, null, null };

            float x = (myTile.row + myTile.col) * 1.0f;
            float y = (myTile.row + myTile.height - myTile.col) * 0.5f;
            float z = (myTile.row * 1.0f) - (myTile.col * 0.1f);
            myTile.transform.position = new Vector3(x, y, z);
            myTile.transform.parent = mapObject.transform;

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
}