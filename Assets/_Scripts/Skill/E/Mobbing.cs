using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mobbing : MonoBehaviour
{
    public float Dmg { get; private set; }

    private float _grabTime = 1.5f;
    private float _tickDmgTime = 1.5f;
    private float _tickInterval = 0.5f;

    private NavMeshAgent _targetAgent;
    private HashSet<Enemy> _targets = new HashSet<Enemy>();
    private Dictionary<NavMeshAgent, Coroutine> _pullCos = new Dictionary<NavMeshAgent, Coroutine>();
    private Coroutine _tickCo;

    private void Start()
    {
        Dmg = GameManager.Instance.PlayerDmg * 1.5f;
        Destroy(gameObject, 2f);
    }
    private void OnEnable()
    {
        _targets.Clear();
        _pullCos.Clear();
    }

    private void OnDisable()
    {
        if (_tickCo != null) StopCoroutine(_tickCo);
        _tickCo = null;

        foreach (var kv in _pullCos)
        {
            if (kv.Value != null) StopCoroutine(kv.Value);
        }
        _pullCos.Clear();
        _targets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        _targets.Add(enemy);

        if (_tickCo == null)
            _tickCo = StartCoroutine(TickDamageAll());

        if (other.CompareTag("Enemy"))
        {
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null && !_pullCos.ContainsKey(agent))
            {
                _pullCos[agent] = StartCoroutine(PullEnemy(agent));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        _targets.Remove(enemy);

        if (other.CompareTag("Enemy"))
        {
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null && _pullCos.TryGetValue(agent, out var co))
            {
                if (co != null) StopCoroutine(co);
                _pullCos.Remove(agent);

                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
            }
        }

        if (_targets.Count == 0 && _tickCo != null)
        {
            StopCoroutine(_tickCo);
            _tickCo = null;
        }
    }

    private IEnumerator TickDamageAll()
    {
        float time = 0f;

        while (time < _tickDmgTime)
        {
            var snapshot = new List<Enemy>(_targets);

            foreach (var enemy in snapshot)
            {
                if (enemy == null) continue;
                if (enemy.isDie) continue;
                enemy.TakeDamage(Dmg);
            }

            yield return new WaitForSecondsRealtime(_tickInterval);
            time += _tickInterval;
        }

        _tickCo = null;
        _targets.Clear();

        foreach (var keyVal in _pullCos)
        {
            var agent = keyVal.Key;
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.isStopped = false;
        }
        _pullCos.Clear();
    }

    private IEnumerator PullEnemy(NavMeshAgent agent)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) yield break;

        agent.isStopped = true;
        agent.ResetPath();

        float timer = 0f;
        Vector3 startPos = agent.transform.position;
        float fixedY = startPos.y;

        Vector3 targetPos = transform.position;
        targetPos.y = fixedY;

        while (timer < _grabTime)
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh) yield break;

            timer += Time.deltaTime;
            float t = timer / _grabTime;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y = fixedY;

            agent.Warp(pos);
            yield return null;
        }
    }
}
