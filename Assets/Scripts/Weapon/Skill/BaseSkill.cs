using UnityEngine;

public class BaseSkill : MonoBehaviour, ILevelup
{
    protected Entity owner;

    [SerializeField]
    private SOSkill origin_data;
    protected SOSkill data;
    public SOSkill Data=>data;

    public virtual void Setup(Entity owner, SOSkill data = null)
    {
        this.owner = owner;

        this.data = (data == null) ? Instantiate(origin_data) : (data);        
    }

    public virtual void LevelUp()
    {
        data.level += 1;
    }
}
