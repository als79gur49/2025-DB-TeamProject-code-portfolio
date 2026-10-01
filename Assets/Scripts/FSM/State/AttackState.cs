using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AttackState : IState
{
    private float rotateMultiple = 4.0f;

    public void Enter(AIInput input)
    {
        input.Animation.SetIdle();
        input.Animation.SetDirection(0, 0);
        //Debug.Log("Attack Enter");
    }

    public void Execute(AIInput input)
    {
        Vector3 direction = (input.target.transform.position - input.self.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        input.self.transform.rotation = Quaternion.Lerp(input.self.transform.rotation, targetRotation, Time.deltaTime * rotateMultiple);

        if(IsAngleSimilar(input.self.transform.forward, direction, 5f))
        {
            Attack(input);
        }
        
        //Debug.Log("Attack Execute");
    }

    public void Exit(AIInput input)
    {
        //Debug.Log("Attack Exit");
    }

    private void Attack(AIInput input)
    {
        if(input.self.TryGetComponent<IAttack>(out IAttack attack))
        {
            attack.Attack();
        }
    }

    private bool IsAngleSimilar(Vector3 self, Vector3 target, float toleranceDegree = 10f)
    {
        if(self == Vector3.zero || target == Vector3.zero)
        {
            return false;
        }

        return Vector3.Angle(self, target) < toleranceDegree;
    }
}
