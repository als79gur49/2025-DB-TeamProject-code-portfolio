using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public void TakeDamage(float amount, Entity enemy, string weaponName);

    public bool IsDead { get;}
}
