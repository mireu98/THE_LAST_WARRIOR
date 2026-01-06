using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulEater : Enemy
{
    public GameObject FireBallPrefabs;
    public Transform FireBallSpawnPoint;
    private NavMeshAgent _agent;
    private bool isFlying = false;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        MaxHp = 7500f;
        CurrentHp = MaxHp;
        Damage = 200f;
        AttackRange = 2f;
        AttackCooldown = 1.2f;
        Exp = 100000f;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = AttackRange;
        _agent.updateRotation = true;
        _agent.updatePosition = true;
        _ani.SetBool("isAttacking", false);
        _ani.SetBool("isFlying", false);
        StartCoroutine(LetsFly());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Speed = 4f;
        if (_player == null || _agent == null) return;
        float distance = Vector3.Distance(transform.position, _player.position);
        if (!isFlying)
        {
            if (distance > AttackRange)
            {
                if (isDie) return;
                if (isHit)
                {
                    _agent.ResetPath();
                    _agent.isStopped = true;
                    Speed = 0f;
                    return;
                }
                if (isAttacking) return;
                isAttacking = false;
                _ani.SetBool("isAttacking", isAttacking);
                _agent.isStopped = false;
                _agent.speed = Speed;
                _agent.SetDestination(_player.position);
                Speed = _agent.velocity.magnitude;
            }
            else
            {
                if (isDie) return;
                if (isHit) return;
                _agent.ResetPath();
                _agent.isStopped = true;
                Speed = 0f;
                transform.LookAt(_player.position);
                isAttacking = true;
                _ani.SetBool("isAttacking", isAttacking);
            }
        }
        else
        {
            transform.LookAt(_player.position);
        }
    }

    IEnumerator ShootFireBall()
    {
        yield return new WaitForSecondsRealtime(3f);

        Vector3 dir = (_player.position - FireBallSpawnPoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        var gmobj = Instantiate(FireBallPrefabs, FireBallSpawnPoint.position, rot);
        var fireball = gmobj.GetComponent<Fireball>();
        if (fireball != null)
            fireball._owner = this;
        yield return new WaitForSecondsRealtime(3f);
    }

    IEnumerator LetsFly()
    {
        yield return new WaitForSecondsRealtime(10f);
        if (isDie) yield break;
        Speed = 0f;
        _agent.isStopped = false;
        _agent.speed = Speed;
        _agent.isStopped = true;
        _agent.ResetPath();
        isFlying = true;
        _ani.SetBool("isFlying", isFlying);
        _ani.SetTrigger("Fly");
        yield return StartCoroutine(ShootFireBall());
        isFlying = false;
        _ani.SetBool("isFlying", isFlying);
        yield return new WaitForSecondsRealtime(5f);
        isAttacking = false;
        _ani.SetBool("isAttacking", isAttacking);
        StartCoroutine(LetsFly());
    }

    public void SoulEaterAttackEnd()
    {
        if (!isAttacking) return;
        isAttacking = false;
        _ani.SetBool("isAttacking", isAttacking);
    }
}
