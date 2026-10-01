using UnityEngine;
using UnityEngine.AI;

public class FleeState : IState
{
    private NavMeshAgent agent;

    private float fleeSpeed = 3f;
    private int fleeDistance = 10;

    public FleeState(float fleeSpeed)
    {
        this.fleeSpeed = fleeSpeed;
    }

    public void Enter(AIInput input)
    {
        agent = input.self.GetComponent<NavMeshAgent>();
        
        if(input.TargetDirection(out Vector3 targetDirection))
        {
            targetDirection *= -1 * fleeDistance;
        }

        agent.SetDestination(targetDirection + input.self.transform.position);
        agent.speed = fleeSpeed;
        agent.isStopped = false;

        input.Animation.SetRun();
        input.Animation.SetDirection(0, 1f);

        //Debug.Log("Flee Enter");
    }

    public void Execute(AIInput input)
    {
        if (input.TargetDirection(out Vector3 targetDirection))
        {
            targetDirection *= -1 * fleeDistance;
        }

        agent.SetDestination(targetDirection + input.self.transform.position);

        //Debug.Log("Flee Execute");
    }

    public void Exit(AIInput input)
    {
        agent.isStopped = true;

        //Debug.Log("Flee Exit");
    }
}
