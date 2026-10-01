using UnityEngine;

public class AttackTransition : ITransition
{
    private int attackRange = 10;
    private LayerMask groundLayer = (1 << 11);

    public AttackTransition(EntityStates state, int attackRange) : base(state)
    {
        this.attackRange = attackRange;
    }

    protected override bool Check(AIInput input)
    {
        float distance = input.DistanceToTarget();

        // 범위 안에 있을 경우
        if (distance > attackRange)
        {
            return false;
        }

        // 목표 방향에 지형지물이 없을 경우
        if (Physics.Raycast(input.self.transform.position, (input.target.transform.position - input.self.transform.position).normalized, distance, groundLayer))
        {
            return false;
        }

        return true;
    }
}
