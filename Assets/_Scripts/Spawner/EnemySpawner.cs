using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private bool _spawnOnce = true;
    private bool _spawned = false;
    private Coroutine _openCo;
    public GameObject OpenWall;
    private List<Enemy> _spawnedEnemies = new();

    private void OnTriggerEnter(Collider other)
    {
        if (_spawnOnce && _spawned) return;
        if (!other.CompareTag("Player")) return;

        Spawn();
        _spawned = true;
    }

    private void FixedUpdate()
    {
        if (!_spawned) return;
        _spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (_spawnedEnemies.Count == 0)
        {
            if (_openCo != null) return;
            _openCo = StartCoroutine(OpenStage());
        }
    }

    IEnumerator OpenStage()
    {
        Vector3 start = OpenWall.transform.localPosition;
        Vector3 end = start + new Vector3(0, 3.5f, 0);

        float duration = 3f;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;
            OpenWall.transform.localPosition = Vector3.Lerp(start, end, time);
            yield return null; 
        }

        OpenWall.transform.localPosition = end;
        OpenWall.SetActive(false);
    }

    private void Spawn()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab이 비었음");
            return;
        }

        if (_spawnPoints == null || _spawnPoints.Count == 0)
        {
            Debug.LogError("[EnemySpawner] spawnPoints가 비었음");
            return;
        }

        foreach (var point in _spawnPoints)
        {
            if (point == null) continue;
            var enemyPre = Instantiate(_enemyPrefab, point.position, point.rotation);
            var enemy = enemyPre.GetComponent<Enemy>();
            if (enemy != null) _spawnedEnemies.Add(enemy);
        }
    }

}
