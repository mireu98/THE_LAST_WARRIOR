using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private float _dmg;
    public float Dmg { get => _dmg; private set => _dmg = value; }

    private Coroutine _tickDamage;
    private HashSet<Enemy> _targets = new HashSet<Enemy>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            Dmg = GameManager.Instance.PlayerDmg * 2f;
        }
    }

    private void OnDisable()
    {
        if (_tickDamage != null)
        {
            StopCoroutine(_tickDamage);
            _tickDamage = null;
        }
        _targets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        _targets.Add(enemy);

        if (_tickDamage == null)
            _tickDamage = StartCoroutine(TickDamage());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        _targets.Remove(enemy);

        if (_targets.Count == 0 && _tickDamage != null)
        {
            StopCoroutine(_tickDamage);
            _tickDamage = null;
        }
    }

    private IEnumerator TickDamage()
    {
        var wait = new WaitForSecondsRealtime(0.5f);

        while (true)
        {
            var snapshot = _targets.ToArray();

            foreach (var enemy in snapshot)
            {
                if (enemy == null || enemy.isDie)
                {
                    _targets.Remove(enemy);
                    continue;
                }

                enemy.TakeDamage(Dmg);
            }

            yield return wait;
        }
    }

}
