using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour {

    public int currFace = 3;
    public Tile tileLoc;
    public int group;

    public Vector3 playerOffset = new Vector3(0, 0.7f, 0);
    public List<Tile> moveQueue = new List<Tile>();
    public float moveSpeed;
    
    private SpriteRenderer charSprite;
    private Animator charAnimator;

    // Use this for initialization
    void Start() {
        charSprite = GetComponent<SpriteRenderer>();
        charAnimator = GetComponent<Animator>();
        moveSpeed = 3.5f;
        group = 1;
    }

    public bool Move(List<Tile> map, Tile currTile, Tile endTile)
    {
        if (endTile.occupied == 0 || endTile.occupied == group)
        {
            moveQueue = Astar(map, currTile, endTile);
            StartCoroutine(SmoothMove(moveQueue));
            return true;
        }
        return false;
    }

    private IEnumerator SmoothMove(List<Tile> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 end = path[i].transform.position + playerOffset;
            float sqrRemaining = (transform.position - end).sqrMagnitude;
            float moveOffset = 0f;

            int newDir = getDir(tileLoc, path[i]);
            faceDir(newDir);

            int heightDiff = (path[i].height > tileLoc.height) ? path[i].height - tileLoc.height : tileLoc.height - path[i].height;
            if (heightDiff == 1)
            {
                resetAnim(charAnimator);
                if (newDir == 0 || newDir == 1)
                    charAnimator.SetBool("marchLowJumpB", true);         
                else if (newDir == 2 || newDir == 3)
                    charAnimator.SetBool("marchLowJumpF", true);
            }
            else if (heightDiff >= 2)
            {
                resetAnim(charAnimator);
                if (newDir == 0 || newDir == 1)
                    charAnimator.SetBool("marchHighJumpB", true);
                else if (newDir == 2 || newDir == 3)
                    charAnimator.SetBool("marchHighJumpF", true);
            }
             
            while (sqrRemaining > float.Epsilon)
            {
                if (heightDiff == 1)
                    moveOffset = lowJumpOffset(tileLoc, newDir);
                else if (heightDiff >= 2)
                    moveOffset = highJumpOffset(tileLoc, newDir);

                transform.position = Vector3.MoveTowards(transform.position, end + new Vector3(0, moveOffset, 0), moveSpeed * Time.deltaTime);
                sqrRemaining = (transform.position - end).sqrMagnitude;

                if (sqrRemaining < 0.4f)
                    charSprite.sortingOrder = path[i].sort + 2;

                yield return null;
            }

            resetAnim(charAnimator);
            if (newDir == 0 || newDir == 1)
                charAnimator.SetBool("marchWalkB", true);
            else if (newDir == 2 || newDir == 3)
                charAnimator.SetBool("marchWalkF", true);
                
            tileLoc = path[i];
        }
    }

    private float lowJumpOffset(Tile currTile, int direction)
    {
        if (direction == 0 || direction == 1)
        {
            float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
            return -1.7f * y * y + 1.7f * y;
        }
        else
        {
            float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
            return -1.7f * y * y + 1.7f * y;
        }
    }

    private float highJumpOffset(Tile currTile, int direction)
    {
        if (direction == 0 || direction == 1)
        {
            float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
            return -4f * y * y + 4f * y;
        }
        else
        {
            float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
            return -4f * y * y + 4f * y;
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
            resetAnim(charAnimator);
            charAnimator.SetBool("marchWalkB", true);
            currFace = dir;
        }
        else if (dir == 2 || dir == 3)
        {
            resetAnim(charAnimator);
            charAnimator.SetBool("marchWalkF", true);
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

    private void resetAnim(Animator anim)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(param.name, false);
        }
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
                    if (!closed.Contains(tile) && !open.Contains(tile) && (tile.occupied == group || tile.occupied == 0))
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