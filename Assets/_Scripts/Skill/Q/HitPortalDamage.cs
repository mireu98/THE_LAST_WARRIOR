using UnityEngine;

public class HitPortalDamage : MonoBehaviour
{
    private float _damage;

    public void SetDamage(float dmg)
    {
        _damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        enemy.TakeDamage(_damage);
    }
}
