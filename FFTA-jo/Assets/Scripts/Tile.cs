using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int id;
    public int height = 0;
    public int row;
    public int col;
    public bool hasObj = false;
    public Tile[] neighbors = new Tile[4] { null, null, null, null };  // left, up, right, down
    public int sort = 0;

    public Tile parent = null;
    public int cost = -1;

    public Tile(int i, int ht, int r, int c, bool obj, Tile[] nbr)
    {
        id = i;
        height = ht;
        row = r;
        col = c;
        hasObj = obj;
        neighbors = nbr;
    }
}
