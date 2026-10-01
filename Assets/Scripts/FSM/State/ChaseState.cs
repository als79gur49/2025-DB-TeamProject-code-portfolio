using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private NavMeshAgent agent;
    private GameObject target;

    private float chaseSpeed = 3.5f;

    public ChaseState(float chaseSpeed)
    {
        this.chaseSpeed = chaseSpeed;
    }

    public void Enter(AIInput input)
    {
        agent = input.self.GetComponent<NavMeshAgent>();
        target = input.target;

        agent.SetDestination(target.transform.position);
        agent.speed = chaseSpeed;
        agent.isStopped = false;

        input.Animation.SetRun();
        input.Animation.SetDirection(0, 1);

        //Debug.Log("Chase Enter");
    }

    public void Execute(AIInput input)
    {
        agent.SetDestination(target.transform.position);

        //Debug.Log("Chase Execute");
    }

    public void Exit(AIInput input)
    {
        agent.isStopped = true;

        //Debug.Log("Chase Exit");
    }
}
