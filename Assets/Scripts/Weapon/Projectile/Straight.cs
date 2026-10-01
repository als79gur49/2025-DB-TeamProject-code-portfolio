using UnityEngine;

public class Straight : Projectile
{
    public override void Setup(GameObject owner, float damage = 1, float speed = 1, float range = 1, float duration = 1, float attackRate = 1, float size = 1)
    {
        base.Setup(owner, damage, speed, range, duration, attackRate, size);
    }

    protected override void Move()
    {
        transform.position += transform.forward * (Data.speed * Time.deltaTime);
    }
}
