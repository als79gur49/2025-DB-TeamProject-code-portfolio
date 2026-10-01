using UnityEngine;

public class IdleTransition : ITransition
{
    public IdleTransition(EntityStates state) : base(state)
    {

    }

    protected override bool Check(AIInput input)
    {
        return true;
    }
}
