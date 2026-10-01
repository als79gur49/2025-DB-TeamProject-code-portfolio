using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Player : Entity
{
    [SerializeField]
    LevelupStorage levelupStorage;

    private NavMeshAgent agent;
    private Dictionary<KeyCode, Vector3> arrowVector;
    private LayerMask groundLayer = 1 << 8;

    private Vector3 inputVector;
    private Vector3 mouseDirection;
    private Vector3 mouseWorldPosition;
    private float directionX;
    private float directionY;
    // 죽음 상태 플래그
    private bool _isDead = false;

    public void Setup(
        EntityInfo info,
        EntityData data,
        DamagePopupManager damagePopupManager,
        KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner,
        SkillIconManager skillIconManager)
    {
        levelupStorage.Setup(skillIconManager, this, firePoint);

        base.Setup(info, data, damagePopupManager, killLogManager, scoreBlockSpawner);
    }

    protected override void Setup()
    {
        base.Setup();

        agent = GetComponent<NavMeshAgent>();

        arrowVector = new Dictionary<KeyCode, Vector3>()
        {
            {KeyCode.UpArrow, Vector3.forward},
            {KeyCode.RightArrow, Vector3.right},
            {KeyCode.DownArrow, Vector3.back},
            {KeyCode.LeftArrow, Vector3.left}
        };

        levelupStorage.AddLevelupable(Weapon);
        //StartWeapon을 levelupStorage에 적용
    }

    private void Update()
    {
        if (IsDead && !_isDead) // 처음 죽었을 때만
        {
            _isDead = true;
            animation.Death();
            GetComponent<BoxCollider>().enabled = false;
            onDeath?.Invoke();
            return;
        }

        if (_isDead) // 이미 죽었다면 아무것도 하지 않음
        {
            return;
        }

        Attack();

        KeyboardInput();
        CalculateMouseDirection();
        LookMouse();
        CalculateAnimDirection();
        Move();
        Animation();
    }

    private void KeyboardInput()
    {
        inputVector = Vector3.zero;

        foreach (var pair in arrowVector)
        {
            if (Input.GetKey(pair.Key))
            {
                inputVector += pair.Value;
            }
        }
    }

    private void CalculateMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            mouseWorldPosition = hit.point;
        }
        else
        {
            // ����ĳ��Ʈ�� �����ϸ� ��鿡 ����
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                mouseWorldPosition = ray.GetPoint(distance);
            }
        }

        mouseDirection = (mouseWorldPosition - transform.position).normalized;
        mouseDirection.y = 0;
    }

    private void CalculateAnimDirection()
    {
        if (inputVector.magnitude < 0.1f)
        {
            directionX = 0;
            directionY = 0;
        }
        else
        {
            float angle = GetAngle360(mouseDirection, inputVector);
            float angleRadian = angle * Mathf.Deg2Rad;

            directionX = Mathf.Sin(angleRadian);
            directionY = Mathf.Cos(angleRadian);
        }
    }

    private float GetAngle360(Vector3 from, Vector3 to)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);

        // 0~ 360 ������ ��ȯ
        if (signedAngle < 0)
        {
            signedAngle += 360f;
        }

        return signedAngle;
    }

    private void LookMouse()
    {
        if (mouseDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(mouseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void Move()
    {
        if (inputVector == Vector3.zero) // idle
        {
            agent.isStopped = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // run
        {
            agent.isStopped = false;
            agent.SetDestination(transform.position + inputVector);
            agent.speed = 2.5f;
        }
        else // walk
        {
            agent.isStopped = false;
            agent.SetDestination(transform.position + inputVector);
            agent.speed = 1.3f;
        }
    }

    private void Animation()
    {
        // �ִϸ��̼� ���� ����
        animation.SetDirection(directionX, directionY);

        if (inputVector == Vector3.zero) // idle
        {
            animation.SetIdle();
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // run
        {
            animation.SetRun();
        }
        else // walk
        {
            animation.SetWalk();
        }
    }

    protected override void levelup()
    {
        // hp회복
        data.AddHp(30);
        onTakeDamage?.Invoke(data.HP, data.MaxHp);
        // 스킬 창 띄우기
        //levelupStorage.Levelupable[0].LevelUp();  
        levelupStorage.Levelup();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 origin = transform.position;

            // ���콺 ���� ǥ�� (������)
            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, mouseDirection * 2f);

            // �Է� ���� ǥ�� (�Ķ���)
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, inputVector * 2f);
        }
    }
}