using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour {
    public string charName;                 // name

    public int currhpStat;                  // hp & mp
    public int maxhpStat = 50;
    public int currmpStat;
    public int maxmpStat = 25;

    public int attackStat = 10;             // attack, defense, speed
    public int magicAtkStat = 10;
    public int defenseStat = 5;
    public int magicDefStat = 5;
    public int speedStat = 10;

    public int moveStat = 4;                // move, jump, evade
    public int jumpStat = 2;
    public int evadeStat = 50;

    public int atkRange = 1;                // attack range
    public int atkHeightLow = 1;
    public int atkHeightHigh = -2;
    System.Random randomHit;

    public int weapAtk = 25;
    public bool ko = false;

    public int id;                          // for UI portrait (assetholder)
    public int currFace = 3;
    public Tile tileLoc;
    public int group;

    public Vector3 charOffset = new Vector3(0, 0.7f, 0);
    public List<Tile> moveQueue = new List<Tile>();
    public float moveSpeed;
    
    public SpriteRenderer charSprite;
    public Animator charAnimator;
    protected GameObject shadow;                    // shadow transform movement needs fixing for high and low jumps

    protected virtual void Awake()
    {
        charSprite = GetComponent<SpriteRenderer>();
        charAnimator = GetComponent<Animator>();
        moveSpeed = 3.5f;
        group = 1;
        currhpStat = maxhpStat;
        currmpStat = maxmpStat;
        randomHit = new System.Random();
    }

    protected virtual void Start()
    {
        shadow = (GameObject)Instantiate(PrefabHolder.Instance.Shadow, tileLoc.transform.position, Quaternion.identity);
        shadow.transform.parent = transform;
    }


    public int Attack(Tile atkTile, List<Character> chars, GameObject endgameTitle)
    {
        Character target = null;
        int realDamage = 0;
        int hit = randomHit.Next(1, 101);     // random number between 1 and 100

        // face the proper direction 
        int newDir = getDir(tileLoc, atkTile);
        faceDir(newDir);

        // inflict damage
        target = atkTile.getChar(chars);
        if (hit <= calcHit(target))
            realDamage = calcDamage(target);
        target.currhpStat -= realDamage;

        checkWeak();

        // set attack animation
        if (currFace == 0 || currFace == 1)
            charAnimator.SetTrigger("marchAttackB");
        else
            charAnimator.SetTrigger("marchAttackF");

        if (realDamage != 0)
        {
            // set target hit animation
            if (target.currFace == 0 || target.currFace == 1)
                target.charAnimator.SetTrigger("marchHitB");
            else
                target.charAnimator.SetTrigger("marchHitF");
        }

        // check if target is KO'd or weakened
        target.checkKO();
        target.checkWeak();

        if(checkGameStatus(target, chars))
        {
            if (target.group == 2)
                endgameTitle.transform.GetChild(1).gameObject.SetActive(true);
            else if (target.group == 1)
                endgameTitle.transform.GetChild(2).gameObject.SetActive(true);
            endgameTitle.transform.GetChild(0).gameObject.SetActive(true);
        }

        return realDamage;
    }

    // calculate damage inflicted on target
    public int calcDamage(Character target)
    {
        float damageFloat = (attackStat - (target.defenseStat / 2)) * ((float)weapAtk / 100);
        int damage = (int)damageFloat;
        return damage;
    }

    // relative facing calculation for evade (hit chance)
    public int calcHit(Character target)
    {
        int evadeTot = 0;
        int newDir = getDir(tileLoc, target.tileLoc);

        // attack from behind (facing same direction) = evade / 4
        if (newDir == target.currFace)
            evadeTot = target.evadeStat / 4;

        // attack from front (facing opposite direction) = evade
        else if (newDir == 0 && target.currFace == 2 || newDir == 2 && target.currFace == 0 ||
            newDir == 1 && target.currFace == 3 || newDir == 3 && target.currFace == 1)
            evadeTot = target.evadeStat;

        // attack from sides = evade / 2
        else
            evadeTot = target.evadeStat / 2;

        return 100 - evadeTot;
    }

    // check game over / game won
    private bool checkGameStatus(Character target, List<Character> chars)
    {
        int checkGroup = target.group;
        bool gameOver = true;

        foreach (Character actor in chars)
        {
            if (actor.group == checkGroup && !actor.ko)
                gameOver = false;
        }

        return gameOver;
    }

    // check if character is KO'd
    private bool checkKO()
    {
        if (currhpStat <= 0)
        {
            currhpStat = 0;
            ko = true;

            resetAnim();
            if (currFace == 0 || currFace == 1)
                charAnimator.SetBool("marchKOB", true);
            else
                charAnimator.SetBool("marchKOF", true);

            shadow.SetActive(false);
            return true;
        }
        return false;
    }

    // check if character is weakened
    private bool checkWeak()
    {
        if (currhpStat <= (maxhpStat / 5) && !ko)
        {
            resetAnim();
            if (currFace == 0 || currFace == 1)
                charAnimator.SetBool("marchWeakB", true);
            else
                charAnimator.SetBool("marchWeakF", true);

            shadow.SetActive(false);
            return true;
        }
        return false;
    }

    // highlight character (grey'd out for attacks)
    public void highlight(bool status)
    {
        if (status)
            charSprite.color = new Color(0.5f, 0.5f, 0.5f, 1);
        else
            charSprite.color = new Color(1, 1, 1, 1);
    }

    // Move the character from the current tile to another tile
    public bool Move(List<Tile> map, Tile currTile, Tile endTile)
    {
        if (endTile.occupied == 0)
        {
            currTile.occupied = 0;
            endTile.occupied = group;
            moveQueue = Astar(map, currTile, endTile);
            StartCoroutine(SmoothMove(moveQueue));
            return true;
        }
        return false;
    }

    // Moves the character sprite along the given Tile path
    protected IEnumerator SmoothMove(List<Tile> path)
    {
        resetAnim();

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 end = path[i].transform.position + charOffset;
            float sqrRemaining = (transform.position - end).sqrMagnitude;
            float moveOffset = 0f;

            // face the proper direction to move to next tile
            int newDir = getDir(tileLoc, path[i]);
            faceDir(newDir);

            // calculate height difference, and choose appropriate low/high jump animation
            int heightDiff = (path[i].height > tileLoc.height) ? path[i].height - tileLoc.height : tileLoc.height - path[i].height;
            if (heightDiff == 1)
            {
                resetAnim();
                if (newDir == 0 || newDir == 1)
                    charAnimator.SetBool("marchLowJumpB", true);         
                else if (newDir == 2 || newDir == 3)
                    charAnimator.SetBool("marchLowJumpF", true);
            }
            else if (heightDiff >= 2)
            {
                resetAnim();
                if (newDir == 0 || newDir == 1)
                    charAnimator.SetBool("marchHighJumpB", true);
                else if (newDir == 2 || newDir == 3)
                    charAnimator.SetBool("marchHighJumpF", true);
            }
            
            // move until the sprite is at the final destination
            while (sqrRemaining > float.Epsilon)
            {
                path[path.Count - 1].tileHighlight(2);                  // highlight tile red while moving

                if (heightDiff == 1)
                    moveOffset = lowJumpOffset(tileLoc, newDir);
                else if (heightDiff >= 2)
                    moveOffset = highJumpOffset(tileLoc, newDir);

                transform.position = Vector3.MoveTowards(transform.position, end + new Vector3(0, moveOffset, 0), moveSpeed * Time.deltaTime);
                sqrRemaining = (transform.position - end).sqrMagnitude;

                if (sqrRemaining < 0.4f)
                {
                    charSprite.sortingOrder = path[i].sort + 3;
                    shadow.GetComponent<SpriteRenderer>().sortingOrder = path[i].sort + 2;
                }

                yield return null;
            }

            // change the sprite back to idle walking animation
            resetAnim();
            if (newDir == 0 || newDir == 1)
                charAnimator.SetBool("marchWalkB", true);
            else if (newDir == 2 || newDir == 3)
                charAnimator.SetBool("marchWalkF", true);
                
            tileLoc = path[i];
            tileLoc.tileHighlight(0);
        }

        if (!checkWeak())
        {
            if (currFace == 0 || currFace == 1)
                charAnimator.SetBool("marchWalkB", true);
            else if (currFace == 2 || currFace == 3)
                charAnimator.SetBool("marchWalkF", true);
        }
    }

    // Parabolic movement for low jump (height different = 1)
    protected float lowJumpOffset(Tile currTile, int direction)
    {
        float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
        return -1.7f * y * y + 1.7f * y;
    }

    // Parabolic movement for high jump (height difference >= 2)
    protected float highJumpOffset(Tile currTile, int direction)
    {
        float y = Mathf.Abs(transform.position.x - currTile.transform.position.x);
        return -4f * y * y + 4f * y;
    }

    // Have character face a certain direction by changing character Animation
    // 0 = left, 1 = up, 2 = right, 3 = down
    public void faceDir(int dir)
    {
        if (((currFace == 0 || currFace == 3) && (dir == 1 || dir == 2)) || 
            ((currFace == 1 || currFace == 2) && (dir == 0 || dir == 3)))
        {
            transform.Rotate(0, 180, 0);
        }

        if (dir == 0 || dir == 1)
        {
            resetAnim();
            charAnimator.SetBool("marchWalkB", true);
            currFace = dir;
        }
        else if (dir == 2 || dir == 3)
        {
            resetAnim();
            charAnimator.SetBool("marchWalkF", true);
            currFace = dir;
        }
    }

    // Returns the direction the character is currently facing
    // 0 = left, 1 = up, 2 = right, 3 = down
    protected int getDir(Tile start, Tile end)
    {
        for(int i = 0; i < 4; i++)
        {
            if (start.neighbors[i] == end)
                return i;
        }

        return -1;
    }

    // Reset Character Animation back to default state (walking, front)
    protected void resetAnim()
    {
        charAnimator.SetBool("marchWalkF", false);
        charAnimator.SetBool("marchWalkB", false);
        charAnimator.SetBool("marchLowJumpF", false);
        charAnimator.SetBool("marchLowJumpB", false);
        charAnimator.SetBool("marchHighJumpF", false);
        charAnimator.SetBool("marchHighJumpB", false);
        charAnimator.SetBool("marchWeakF", false);
        charAnimator.SetBool("marchWeakB", false);
        charAnimator.SetBool("marchKOF", false);
        charAnimator.SetBool("marchKOB", false);
    }

    // Return list of Tiles to highlight for attack
    public List<Tile> allAttack(List<Tile> map)
    {
        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();
        Tile[] neighbors = new Tile[4] { null, null, null, null };

        Tile current = tileLoc;
        open.Add(tileLoc);

        for (int i = 0; i <= atkRange; i++)
        {
            int count = open.Count;
            for (int j = 0; j < count; j++)
            {
                current = open[0];
                open.Remove(current);
                closed.Add(current);
                neighbors = current.neighbors;

                foreach (Tile tile in neighbors)
                {
                    if (tile)
                        if (!open.Contains(tile) && !closed.Contains(tile) && tile.occupied != 3 && 
                            (tile.height - current.height) <= atkHeightLow && (tile.height - current.height) >= atkHeightHigh)
                            open.Add(tile);
                }
            }

        }
        closed.Remove(tileLoc);
        return closed;
    }

    // Return list of Tiles to highlight for Move
    public List<Tile> AstarGlow(List<Tile> map)
    {
        List<Tile> poss = allMoves(map);
        List<Tile> res = new List<Tile>();
        
        foreach (Tile tile in poss)
        {
            List<Tile> path = Astar(map, tileLoc, tile);
            if (path != null)
                if (path.Count <= moveStat + 1)
                    res.Add(tile);
        }

        return res;
    }

    // Return list of all Tiles that are within "moveStat" steps away from current character location
    // Used with Astar to generate which Tiles are moveable and should be highlighted
    private List<Tile> allMoves(List<Tile> map)
    {
        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();
        Tile[] neighbors = new Tile[4] { null, null, null, null };

        Tile current = tileLoc;
        open.Add(tileLoc);

        for (int i = 0; i <= moveStat; i++)
        {
            int count = open.Count;
            for (int j = 0; j < count; j++)
            {
                current = open[0];
                open.Remove(current);
                closed.Add(current);
                neighbors = current.neighbors;

                foreach (Tile tile in neighbors)
                {
                    if (tile)
                        if (!open.Contains(tile) && !closed.Contains(tile) && (tile.occupied == 0 || tile.occupied == group))
                            open.Add(tile);
                }
            }

        }

        closed.Remove(tileLoc);
        for (int i = closed.Count - 1; i >= 0; i--)
        {
            if (closed[i].occupied != 0)
                closed.RemoveAt(i);
        }

        return closed;
    }

    // Basic Astar algorithm
    // Returns path (list of Tiles) from one Tile to another; null if no path available
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

            foreach (var tile in neighbors)
            {
                if (tile)
                {
                    if (!closed.Contains(tile) && !open.Contains(tile) &&           // if not already checked or checking
                        (tile.occupied == group || tile.occupied == 0) &&           // if tile is passable (same group)
                        (Math.Abs(tile.height - current.height) <= jumpStat))       // if tile is not too tall
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

    // set character stats (-9 if keep as is)
    public void setStats(int hp, int mp, int atk, int def, int mAtk, int mDef, int spd, int move, int jump, int evade)
    {
        if (hp != -9)
        {
            currhpStat = hp;             
            maxhpStat = hp;
        }
        if (mp != -9)
        {
            currmpStat = mp;
            maxmpStat = mp;
        }

        if (atk != -9)
            attackStat = atk;
        if (def != -9)
            defenseStat = def;
        if (mAtk != -9)
            magicAtkStat = mAtk;
        if (mDef != -9)
            magicDefStat = mDef;
        if (spd != -9)
            speedStat = spd;

        if (move != -9)
            moveStat = move;             
        if (jump != -9)
            jumpStat = jump;
        if (evade != -9)
            evadeStat = evade;
    }
}