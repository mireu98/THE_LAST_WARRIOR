using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAttackHitboxDmg : MonoBehaviour
{
    private WeaponHitbox _owner;

    void Start()
    {
        _owner = GetComponentInParent<WeaponHitbox>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;
        _owner.HitEffect.Play();
        enemy.TakeDamage(_owner.Dmg);
    }
}
