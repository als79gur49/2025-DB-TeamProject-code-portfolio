using UnityEngine;

public class AIInput : MonoBehaviour
{
    public GameObject self;
    public GameObject target;

    private EntityAnimation animation;
    private Entity entity;
    public EntityAnimation Animation => animation;
    public Entity Entity => entity;

    [SerializeField]
    private float detectionRange = 200f;

    [SerializeField]
    private LayerMask targetLayer = (1 << 8); // AttackCollider

    private void Update()
    {
        UpdateTarget();
    }

    public void SetAnimation(EntityAnimation animation)
    {
        this.animation = animation;
    }
    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }
    private void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject == self)
            {
                continue;
            }
            if (!hit.gameObject.TryGetComponent<Entity>(out Entity t))
            { 
                continue;
            }

            float distance = Vector3.Distance(self.transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit.gameObject;
            }
        }

        target = closest; // 가장 가까운 타겟을 설정
    }

    public float DistanceToTarget()
    {
        if (self == null || target == null)
        {
            return float.MaxValue;
        }

        return Vector3.Distance(target.transform.position, self.transform.position);
    }

    public bool TargetDirection(out Vector3 targetDirection)
    {
        targetDirection = Vector3.zero;

        if(target == null)
        {
            return false;
        }

        Vector3 direction = (target.transform.position - self.transform.position);
        targetDirection = direction.normalized;

        return true;
    }
}
