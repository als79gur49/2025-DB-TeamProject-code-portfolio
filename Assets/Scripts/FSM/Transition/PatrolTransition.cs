using UnityEngine;

public class PatrolTransition : ITransition
{

    public PatrolTransition(EntityStates state) : base(state)
    {

    }

    protected override bool Check(AIInput input)
    {
        return true;
    }

}
