using UnityEngine;
using System.Collections;
public class InitBattleState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        board.Load();
        yield return null;
        owner.ChangeState<MoveTargetState>();
    }
}