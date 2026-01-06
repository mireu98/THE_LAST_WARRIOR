using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nightmare : Enemy
{
    private NavMeshAgent _agent;
    private int _comboCnt = 0;
    private bool bAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        MaxHp = 5000f;
        CurrentHp = MaxHp;
        Damage = 200f;
        AttackRange = 3f;
        AttackCooldown = 1.2f;
        Exp = 1500f;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = AttackRange;
        _agent.updateRotation = true;
        _agent.updatePosition = true;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Speed = 5f;
        if (_player == null || _agent == null) return;
        bAttacking = _ani.GetBool("isAttacking");
        float distance = Vector3.Distance(transform.position, _player.position);

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
            if (bAttacking) return;
            _agent.isStopped = false;
            _agent.speed = Speed;
            _agent.SetDestination(_player.position);
            Speed = _agent.velocity.magnitude;
        }
        else
        {
            if (isDie) return;
            _agent.ResetPath();
            _agent.isStopped = true;
            Speed = 0f;
            if (isHit) return;
            transform.LookAt(_player.position);
            bAttacking = true;
            _ani.SetBool("isAttacking", true);
        }
    }

    //애니메이션 이벤트
    public void NightmareAttackEnd()
    {
        if (!bAttacking) return;

        _comboCnt = (_comboCnt + 1) % 3;
        if (Vector3.Distance(transform.position, _player.position) <= AttackRange)
        {
            _ani.SetInteger("ComboCount", _comboCnt);
        }
        else
        {
            bAttacking = false;
            _ani.SetBool("isAttacking", false);
            _comboCnt = 0;
            _ani.SetInteger("ComboCount", _comboCnt);
        }
    }

}
