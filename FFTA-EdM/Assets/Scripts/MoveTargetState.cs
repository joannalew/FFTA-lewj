using UnityEngine;
using System.Collections;
public class MoveTargetState : BattleState
{
    protected override void OnMove(object sender, InfoEventArgs<int> e)
    {
        moveCursor(e.info);
    }
}