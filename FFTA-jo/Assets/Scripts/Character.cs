using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour {

    public int currFace = 3;
    public Vector3 playerOffset = new Vector3(0, 0.7f, 0);
    public List<Tile> moveQueue = new List<Tile>();
    public float moveSpeed;
    public Tile tileLoc;

    private SpriteRenderer charSprite;
    private Animator charAnimator;

    // Use this for initialization
    void Start() {
        charSprite = GetComponent<SpriteRenderer>();
        charAnimator = GetComponent<Animator>();
        moveSpeed = 3.5f;
    }

    public void Move(List<Tile> map, Tile currTile, Tile endTile)
    {
        moveQueue = Astar(map, currTile, endTile);
        StartCoroutine(SmoothMove(moveQueue));
    }

    private IEnumerator SmoothMove(List<Tile> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 end = path[i].transform.position + playerOffset;
            float sqrRemaining = (transform.position - end).sqrMagnitude;

            int newDir = getDir(tileLoc, path[i]);
            faceDir(newDir);

            while (sqrRemaining > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
                sqrRemaining = (transform.position - end).sqrMagnitude;
                yield return null;
            }

            tileLoc = path[i];
            charSprite.sortingOrder = tileLoc.sort + 2;
        }
    }

    private void faceDir(int dir)
    {
        if (((currFace == 0 || currFace == 3) && (dir == 1 || dir == 2)) || 
            ((currFace == 1 || currFace == 2) && (dir == 0 || dir == 3)))
        {
            transform.Rotate(0, 180, 0);
        }

        if (dir == 0 || dir == 1)
        {
            charAnimator.SetBool("marchWalkB", true);
            charAnimator.SetBool("marchWalkF", false);
            currFace = dir;
        }
        else if (dir == 2 || dir == 3)
        {
            charAnimator.SetBool("marchWalkF", true);
            charAnimator.SetBool("marchWalkB", false);
            currFace = dir;
        }
    }

    private int getDir(Tile start, Tile end)
    {
        for(int i = 0; i < 4; i++)
        {
            if (start.neighbors[i] == end)
                return i;
        }

        return -1;
    }


    private List<Tile> Astar(List<Tile> map, Tile start, Tile end)
    {
        XMLManager.resetMap(map);

        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();
        Tile[] neighbors = new Tile[4] { null, null, null, null };
        List<Tile> res = new List<Tile>();

        Tile current = start;
        open.Add(start);

        while (open.Count != 0 && !closed.Exists(x => x.id == end.id))
        {
            current = open[0];
            open.Remove(current);
            closed.Add(current);
            neighbors = current.neighbors;

            foreach(var tile in neighbors)
            {
                if (tile)
                {
                    if (!closed.Contains(tile) && !open.Contains(tile))
                    {
                        tile.parent = current;
                        tile.cost = 1 + tile.parent.cost;
                        open.Add(tile);
                        open = open.OrderBy(t => t.cost).ToList<Tile>();
                    }
                }
            }
        }

        // if no path
        if (!closed.Exists(x => x.id == end.id))
            return null;

        // if path exists
        res.Add(end);

        while (current.parent != start && closed.Count > 1)
        {
            current = current.parent;
            res.Add(current);
        }
        res.Add(start);
        res.Reverse();

        return res;
    }
}