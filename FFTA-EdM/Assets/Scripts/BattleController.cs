using UnityEngine;
using System.Collections;
public class BattleController : StateMachine
{
    public Board board;
    void Start()
    {
        ChangeState<InitBattleState>();
    }
}
