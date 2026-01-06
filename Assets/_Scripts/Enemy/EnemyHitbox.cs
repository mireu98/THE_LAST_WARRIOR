using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Enemy _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_owner == null) return;

        _owner.DamageToPlayer(); 
    }
}
