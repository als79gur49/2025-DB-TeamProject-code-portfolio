using UnityEngine;

public class ChaseTransition : ITransition
{
    private int chaseRange = 50;

    public ChaseTransition(EntityStates state, int chaseRange) : base(state)
    {
        this.chaseRange = chaseRange;
    }

    protected override bool Check(AIInput input)
    {
        if(input.DistanceToTarget() <= chaseRange)
        {
            return true;
        }

        return false;
    }

}
