using UnityEngine;

public abstract class ITransition
{
    private EntityStates state;

    public ITransition(EntityStates state)
    {
        this.state = state;
    }

    protected abstract bool Check(AIInput input);
    public bool CheckTransition(AIInput input)
    {
        bool result = Check(input);

        return result;
    }
}
