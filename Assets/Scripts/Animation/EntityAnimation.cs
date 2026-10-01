using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private string speed = "Speed_f";
    private string death = "Death_b";
    private string deathType = "DeathType_int";

    private string dirX = "DirectionX";
    private string dirY = "DirectionY";
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void Setup(Animator animator, NavMeshAgent agent)
    {
        this.animator = animator;
        this.agent = agent;
    }

    public void Death()
    {
        animator.SetInteger(deathType, Random.Range(1, 3));
        animator.SetBool(death, true);
    }

    public void SetRun()
    {
        SetSpeed(0.99f);
    }
    public void SetWalk()
    {
        SetSpeed(0.49f);
    }
    public void SetIdle()
    {
        SetSpeed(0);
    }

    private void SetSpeed(float moveSpeed)
    {
        animator.SetFloat(speed, moveSpeed);
    }
    public void SetDirection(float x, float y)
    {
        animator.SetFloat(dirX, x);
        animator.SetFloat(dirY, y);
    }
    public void SetForwardLoop()
    {
        animator.speed = 1;
    }
    public void SetBackwardLoop()
    {
        animator.speed = -1;
    }
}
