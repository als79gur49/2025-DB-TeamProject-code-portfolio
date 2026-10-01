using UnityEngine;

public class FleeTransition : ITransition
{
    private float fleeRange = 45;

    public FleeTransition(EntityStates state) : base(state)
    {

    }

    protected override bool Check(AIInput input)
    {
        if(input.DistanceToTarget() < fleeRange &&
            input.target.TryGetComponent<Entity>(out Entity targetEntity) &&
            input.Entity.Data.HPPercent <= 40 &&
            targetEntity.Data.HPPercent >= 80)
        {

            return true;
        }
     
       return false;
    }
}
