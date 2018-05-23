using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Character : MonoBehaviour {
    public int moveStat = 4;
    public int jumpStat = 2;
    public int hpStat = 25;
    public int attackStat = 5;
    public int atkRange = 10;
    public int atkHeightLow = 1;
    public int atkHeightHigh = -2;

    public int currFace = 3;
    public Tile tileLoc;
    public int group;

    public Vector3 charOffset = new Vector3(0, 0.7f, 0);
    public List<Tile> moveQueue = new List<Tile>();
    public float moveSpeed;
    
    public SpriteRenderer charSprite;
    protected Animator charAnimator;
    protected GameObject shadow;                    // shadow transform movement needs fixing for high and low jumps

    protected virtual void Awake()
    {
        charSprite = GetComponent<SpriteRenderer>();
        charAnimator = GetComponent<Animator>();
        moveSpeed = 3.5f;
        group = 1;
    }

    protected virtual void Start()
    {
        shadow = (GameObject)Instantiate(PrefabHolder.Instance.Shadow, tileLoc.transform.position, Quaternion.identity);
        shadow.transform.parent = transform;
    }
    
    public void Attack(Tile atkTile, List<Character> chars, List<Enemy> enems)
    {
        Character player = null;
        Enemy enemy = null;

        // face the proper direction to move to next tile
        int newDir = getDir(tileLoc, atkTile);
        faceDir(newDir);

        if (atkTile.occupied == 1)
        {
            player = atkTile.getChar(chars);
            player.hpStat -= attackStat;
            if (currFace == 0 || currFace == 1)
                charAnimator.SetTrigger("marchAttackB");
            else
                charAnimator.SetTrigger("marchAttackF");

            if (player.currFace == 0 || player.currFace == 1)
                player.GetComponent<Animator>().SetTrigger("marchHitB");
            else
                player.GetComponent<Animator>().SetTrigger("marchHitF");
        }
        else if (atkTile.occupied == 2)
        {
            enemy = atkTile.getEnem(enems);
            enemy.hpStat -= attackStat;

            if (currFace == 0 || currFace == 1)
                charAnimator.SetTrigger("marchAttackB");
            else
                charAnimator.SetTrigger("marchAttackF");

            if (enemy.currFace == 0 || enemy.currFace == 1)
                enemy.GetComponent<Animator>().SetTrigger("marchHitB");
            else
                enemy.GetComponent<Animator>().SetTrigger("marchHitF");
        }
    }
    
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
    public IEnumerator SmoothMove(List<Tile> path)
    {
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
            resetAnim(charAnimator);
            if (newDir == 0 || newDir == 1)
                charAnimator.SetBool("marchWalkB", true);
            else if (newDir == 2 || newDir == 3)
                charAnimator.SetBool("marchWalkF", true);
                
            tileLoc = path[i];
            tileLoc.tileHighlight(0);
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
    protected void resetAnim(Animator anim)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(param.name, false);
        }
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
    protected List<Tile> allMoves(List<Tile> map)
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
                        if (!open.Contains(tile) && !closed.Contains(tile) && tile.occupied == 0)
                            open.Add(tile);
                }
            }

        }
        closed.Remove(tileLoc);
        return closed;
    }

    // Basic Astar algorithm
    // Returns path (list of Tiles) from one Tile to another; null if no path available
    public List<Tile> Astar(List<Tile> map, Tile start, Tile end)
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

    // set a Character parameter to another value
    // (useful if not all parameters public)
    public void setStat<T>(string statName, T value)
    {
        FieldInfo field = GetType().GetField(statName);
        field.SetValue(this, value);
    }





    public List<Tile> PotentialSpacesToMoveTo(List<Tile> map)
    {
        List<Tile> potentialSpaces = new List<Tile>();
        List<Tile> spaceOccupiedBySameType = new List<Tile>();
        Queue<Tile> toCheck = new Queue<Tile>();
        Tile[] neighbors = new Tile[4] { null, null, null, null };
        Tile start = tileLoc;
        Tile cur;

        XMLManager.resetMap(map);

        start.cost = 0;
        toCheck.Enqueue(start);
        potentialSpaces.Add(start);

        while (toCheck.Count != 0)
        {
            cur = toCheck.Dequeue();
            neighbors = cur.neighbors;

            foreach (var tile in neighbors)
            {
                if (tile != null)
                {
                    if (tile.cost == -1 || (cur.cost + 1 < tile.cost))
                        tile.cost = cur.cost + 1;

                    //tile is added to list if it is in range depending on the character's move stat, is not occupied, is not already in the list, it is accessible based on character's jump stat, and it's not the starting tile
                    if ((tile.cost <= moveStat) && (tile.occupied == 0 || tile.occupied == group) &&
                        !potentialSpaces.Contains(tile) && (Math.Abs(cur.height - tile.height) <= jumpStat) &&
                        !spaceOccupiedBySameType.Contains(tile))
                    {
                        if (tile.occupied == 2)
                        { //if space is occupied by other enemy, can't move there, but should still examine neighbors
                            toCheck.Enqueue(tile);
                            spaceOccupiedBySameType.Add(tile);
                        }
                        else
                        {
                            potentialSpaces.Add(tile);
                            toCheck.Enqueue(tile);
                        }
                    }
                }
            }
        }
        return potentialSpaces;
    }




    public IEnumerator ExecuteEnemyTurnAI(Character curChar, List<Tile> map, List<Character> charaList)
    {   //determine possible spaces to move to
        List<Tile> potentialSpaces = curChar.PotentialSpacesToMoveTo(map);

        //display spaces with blue animation, pause for 1 second to see animation
        foreach (Tile space in potentialSpaces)
            space.tileHighlight(1);
        yield return new WaitForSeconds(1.1f);

        //determine target (will always return a character)
        Character target = DetermineTarget(curChar, map, charaList, potentialSpaces);

        //get path to target to compare to moveable spaces
        List<Tile> totalPathToTarget = Astar(map, curChar.tileLoc, target.tileLoc);
        
        
        List<Tile> executablePath = new List<Tile>();
        Tile destinationTile;

        //if there's no path to the target, move to a space at random
        if (totalPathToTarget.Count == 0)
        {
            int destNum = UnityEngine.Random.Range(0, potentialSpaces.Count - 1);
            destinationTile = potentialSpaces[destNum];
            executablePath = Astar(map, curChar.tileLoc, destinationTile);
        }
        else //creates List of intersection between the complete path to the target and the possible spaces to move to
        {
             foreach (var totalPath in totalPathToTarget)
             {
                  foreach (var potential in potentialSpaces)
                  {
                     if (totalPath.id == potential.id)
                         executablePath.Add(totalPath);
                  }
             }
            destinationTile = executablePath[executablePath.Count() - 1];
        }
       
        
        //glow final tile in executable path (destination tile) orange
        destinationTile.tileHighlight(2);
        yield return new WaitForSeconds(.3f);


        //remove blue glow on potential spaces
        foreach (Tile space in potentialSpaces)
            space.tileHighlight(0);

        //move to new tile 
        StartCoroutine(curChar.SmoothMove(executablePath));
        yield return new WaitForSeconds(3.0f);


        //attack target if possible
        //green glow tiles in attack range - determine targets in attack range
        List<Tile> attackRange = curChar.TilesInAttackRange(map);
        bool playerInAttackRange = false;
        //see if any players on on tiles in attack range
        foreach(var player in charaList)
        {
            if(attackRange.Contains(player.tileLoc))
            {
                playerInAttackRange = true;
                break;
            }
        }

        if (playerInAttackRange)
        {
            foreach (Tile space in attackRange)
                space.tileHighlight(3);
            yield return new WaitForSeconds(1.0f);

            //if target is in attack range, attack target
            foreach (Tile space in attackRange)
                space.tileHighlight(0);
        }
    }



    public static List<Tile> GetReachableEnemyLocations(List<Tile> moveableSpaces, Character chara)
    {
        List<Tile> reachableEnemyLocations = new List<Tile>();
        foreach (var tile in moveableSpaces)
        {
            foreach (var neighbor in tile.neighbors)
            {
                if (neighbor != null && (Math.Abs(tile.height - neighbor.height) <= chara.atkHeightLow)
                    && !reachableEnemyLocations.Contains(neighbor))
                {
                    reachableEnemyLocations.Add(neighbor);
                }
            }
        }
        return reachableEnemyLocations;
    }


    public static Character DetermineTarget(Character curChar, List<Tile> map, List<Character> charaList, List<Tile> moveableSpaces)
    {
        int randNum;

        Character target = null;
        double damage;
        double maxDamage = 10000;
        List<Character> possibleTargets = new List<Character>();

        //get list of tiles where an enemy could be reached
        List<Tile> reachableEnemyLocations = GetReachableEnemyLocations(moveableSpaces, curChar);

        //see if any target is in range
        foreach (var chara in charaList)
        {
            if (chara.group == 1) //if character in list is player character - tentative, since may organize to have separate character lists for player characters and enemies, in which case we'll just pass the player list as the parameter
            {
                if (reachableEnemyLocations.Contains(chara.tileLoc))
                {
                    possibleTargets.Add(chara);
                }
            }
        }

        if (possibleTargets.Count == 1)
            return possibleTargets[0];
        else if (possibleTargets.Count == 0)
        {
            foreach (var chara in charaList)
            {
                if (chara.group == 1)
                    possibleTargets.Add(chara);
            }
        }


        foreach (var chara in possibleTargets)
        {
            damage = (chara.hpStat - 5.0) / chara.hpStat; //tentative equation until attack script is complete
            if (damage == maxDamage)
            {
                randNum = UnityEngine.Random.Range(1, 2);
                if (randNum % 2 == 0)
                    target = chara;
            }
            else if (damage < maxDamage)
            {
                maxDamage = damage;
                target = chara;
            }
        }

        return target;
    }

    protected List<Tile> TilesInAttackRange(List<Tile> map)
    {
        List<Tile> tilesInAttackRange = new List<Tile>();
        Queue<Tile> toCheck = new Queue<Tile>();
        Tile[] neighbors = new Tile[4] { null, null, null, null };
        Tile start = tileLoc;
        Tile cur;

        XMLManager.resetMap(map);

        start.cost = 0;
        toCheck.Enqueue(start);
        tilesInAttackRange.Add(start);

        while (toCheck.Count != 0)
        {
            cur = toCheck.Dequeue();
            neighbors = cur.neighbors;

            foreach (var tile in neighbors)
            {
                if (tile != null)
                {
                    if (tile.cost == -1 || (cur.cost + 1 < tile.cost))
                        tile.cost = cur.cost + 1;
                    if(tile.cost <= atkRange && !tilesInAttackRange.Contains(tile)
                        && Math.Abs(cur.height - tile.height) <= atkHeightLow)
                    {
                        tilesInAttackRange.Add(tile);
                        toCheck.Enqueue(tile);
                    }
                }    
            }
        }
        return tilesInAttackRange;
    }

}