using UnityEngine;

[CreateAssetMenu(fileName = "SO_Projectile_", menuName = "Projectile", order = 0)]
public class SOProjectile : ScriptableObject
{
    public string projectileName;

    public Sprite sprite;

    public float damage = 10f;
    public float speed = 50f;
    public float range = 500f;
    public float duration = 10f;
    public float attackRate = 1f;

    public float size = 0.3f;
}
