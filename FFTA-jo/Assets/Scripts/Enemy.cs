using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
        //base.setStat("group", 2);
        group = 2;
    }

    //IN PROGRESS - executes enemy's turn - displays spaces to move to, chooses target, moves to appropriate space, attacks if appropriate
    protected void executeTurnAI(Enemy curEnemy, List<Tile> map, List<Player> playerList)
    {
        
        //determine possible spaces to move to
        List<Tile> potentialSpaces = potentialSpacesToMoveTo(map, curEnemy);

        //display spaces with blue animation

        //determine target
        Player target = determineTarget(curEnemy, map, playerList);
        // determine tile to move to if target = null

        //move to target or as close as possible
        List<Tile> totalPathToTarget = Astar(map, curEnemy.tileLoc, target.tileLoc);

        //creates List of intersection between the complete path to the target and the possible spaces to move to
        List<Tile> executablePathToTarget = new List<Tile>();
        foreach (var totalPath in totalPathToTarget)
        {
            foreach (var potential in potentialSpaces)
            {
                if (totalPath.id == potential.id)
                    executablePathToTarget.Add(totalPath);
            }
        }
        //if target is within range, remove the target's tile from the path to be traveled
        if (totalPathToTarget[totalPathToTarget.Count - 1].id == executablePathToTarget[executablePathToTarget.Count - 1].id)
            executablePathToTarget.RemoveAt(executablePathToTarget.Count - 1);

        StartCoroutine(SmoothMove(executablePathToTarget));

        //remove blue animation

        //attack target if possible
    }


    // determines which character to attack based on which character it can inflict highest percentage of damage against
    protected Player determineTarget(Enemy curEnemy, List<Tile> map, List<Player> playerList)
    {
        Player target = null;
        double damage;
        double maxDamage = -5;

        foreach (var player in playerList)
        {
            if (player.alive) 
            {
                damage = (player.attackStat - player.defenseStat / 2.0) / player.hpStat; //tentative equation until attack script is complete
                if (damage >= 1)
                    damage = 1; //will kill the character

                if (damage == maxDamage)
                {
                    target = closestTarget(curEnemy, map, target, player);
                }
                else if (damage > maxDamage && (Astar(map, curEnemy.tileLoc, player.tileLoc) != null))
                {
                    maxDamage = damage;
                    target = player;
                }
            }
        }

        return target;
    }



    protected Character closestTarget(Character curChar, List<Tile> map, Character c1, Character c2)
    {
        List<Tile> p1 = Astar(map, curChar.curLocation, c1.curLocation);
        List<Tile> p2 = Astar(map, curChar.curLocation, c2.curLocation);

        if (p1 == null && p2 == null)
            return null;

        if (p1 == null)
            return c2;

        if (p2 == null)
            return c1;

        if (p1.Count > p2.Count)
            return c2;
        return c1;
    }


}
