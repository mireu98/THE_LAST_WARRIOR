using System.Collections;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    private Transform _playerTransform;
    private PlayerAdd _playerAdd;         
    private Coroutine _tickco;
    private float _speed = 3.5f;

    void Start()
    {
        CachePlayer();
        Destroy(gameObject, 10f);
    }

    void CachePlayer()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.Player == null) return;

        _playerTransform = gm.Player.transform;
        _playerAdd = gm.Player.GetComponent<PlayerAdd>();
    }

    void FixedUpdate()
    {
        if (_playerTransform == null)
        {
            CachePlayer();
            if (_playerTransform == null) return;
        }

        Vector3 dis = (_playerTransform.position - transform.position).normalized;
        transform.position += dis * _speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerAdd = other.GetComponent<PlayerAdd>();

        if (_tickco == null)
            _tickco = StartCoroutine(TickDmg());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_tickco != null) StopCoroutine(_tickco);
        _tickco = null;
    }

    IEnumerator TickDmg()
    {
        while (true)
        {
            if (_playerAdd == null) yield break;

            _playerAdd.TakeDamage(5000f);

            yield return new WaitForSeconds(1.5f);
        }
    }
}
