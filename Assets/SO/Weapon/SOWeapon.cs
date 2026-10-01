using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon_", menuName = "Weapon", order = 0)]
public class SOWeapon: ScriptableObject
{
    public string weaponName;

    public Sprite sprite;
    public int level = 1;

    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float durationMultiplier = 1f;
    public float attackRateMultiplier = 1f;

    public float sizeMultiplier = 1f;
}
