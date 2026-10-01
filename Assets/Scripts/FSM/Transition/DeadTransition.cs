using UnityEngine;

public class DeadTransition : ITransition
{
    public DeadTransition(EntityStates state) : base(state)
    {

    }

    protected override bool Check(AIInput input)
    {
        return input.Entity.IsDead;
    }
}
