using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    public IdleState()
    {
    }

    public void Enter(AIInput input)
    {
        input.Animation.SetIdle();
        input.Animation.SetDirection(0, 0);

        //Debug.Log("Idle Enter");
    }

    public void Execute(AIInput input)
    {
        //Debug.Log("Idle Execute");
    }

    public void Exit(AIInput input)
    {
        //Debug.Log("Idle Exit");
    }

}
