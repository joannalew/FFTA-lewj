using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Turn
{
    public Character actor;
    public bool hasCharMoved;
    public bool hasCharActed;
    public bool lockMove;
    Tile startTile;
    private int startDir;
    public void Change(Character current)
    {
        actor = current;
        hasCharMoved = false;
        hasCharActed = false;
        lockMove = false;
        startTile = actor.tileLoc;
        startDir = actor.currFace;
    }
    public void UndoMove()
    {
        hasCharMoved = false;
        actor.transform.position = startTile.transform.position + actor.charOffset;
        actor.faceDir(startDir);
    }
}
