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

    public int occupied = 0;    // 0 = free, 1 = player, 2 = enemy, 3 = map object
    public int sort = 0;        // isometric sorting order: 10 * row - col
    public int glow = 0;        // 0 = none, 1 = blue, 2 = red, 3 = green

    public Tile parent = null;  // a* stuff
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

    // set the tile highlight color
    // 0 = none, 1 = blue, 2 = red, 3 = green
    public void tileHighlight(int color)
    {
        Animator tileAnim = transform.GetChild(0).gameObject.GetComponent<Animator>();

        if (color == 1)
        {
            tileAnim.SetBool("tileBlue", true);
            glow = 1;
        }
        else if (color == 2)
        {
            tileAnim.SetBool("tileBlue", false);
            tileAnim.SetBool("tileRed", true);
            glow = 2;
        }
        else if (color == 3)
        {
            tileAnim.SetBool("tileGreen", true);
            glow = 3;
        }
        else
        {
            glow = 0;
            tileAnim.SetBool("tileBlue", false);
            tileAnim.SetBool("tileRed", false);
            tileAnim.SetBool("tileGreen", false);
        }
    }

    public Character getChar(List<Character> characters)
    {
        foreach (var actor in characters)
        {
            if (actor.tileLoc == this)
                return actor;
        }

        return null;
    }

    public Enemy getEnem(List<Enemy> enemies)
    {
        foreach(var enemy in enemies)
        {
            if (enemy.tileLoc == this)
                return enemy;
        }

        return null;
    }
}
