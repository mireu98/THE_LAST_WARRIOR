using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackHitbox : MonoBehaviour
{
    private BoxCollider _col;
    public ParticleSystem HitEffect;
    private float _dmg;
    public float Dmg { get => _dmg; set => _dmg = value; }

    private void Awake()
    {
        HitEffect.Stop();
    }

    void Start()
    {
        Dmg = GameManager.Instance.PlayerDmg;
        _col = GetComponent<BoxCollider>();
    }
    public void Enable()
    {
        _col.enabled = true;
    }

    public void Disable()
    {
        HitEffect.Stop();
        _col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        HitEffect.Play();
        enemy.TakeDamage(Dmg);
    }
}
