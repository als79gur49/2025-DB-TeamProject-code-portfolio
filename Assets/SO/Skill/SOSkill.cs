using UnityEngine;

[CreateAssetMenu(fileName = "SO_Skill_", menuName = "Skill", order = 0)]
public class SOSkill : ScriptableObject
{
    public string skillName;

    public Sprite sprite;
    public int level = 1;

    public string description;
}
