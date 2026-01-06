using System.Collections;
using UnityEngine;

public class GroundFireWall : MonoBehaviour
{
    public GameObject WarningZone;
    public ParticleSystem GFW;
    private CapsuleCollider _capcol;

    private float damagePerTick = 2000f;
    private float tickInterval = 1.5f;

    private Coroutine _tickCo;
    private PlayerAdd _player;

    void Start()
    {
        _capcol = GetComponent<CapsuleCollider>();
        _capcol.enabled = false;

        WarningZone.SetActive(false);
        GFW.Stop();

        StartCoroutine(StartPattern());
    }

    IEnumerator StartPattern()
    {
        WarningZone.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        GFW.Play();
        _capcol.enabled = true;
        WarningZone.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        if (_tickCo != null) StopCoroutine(_tickCo);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_capcol.enabled) return;
        if (!other.CompareTag("Player")) return;

        _player = other.GetComponent<PlayerAdd>();
        if (_player == null) return;

        if (_tickCo == null)
            _tickCo = StartCoroutine(TickDamage());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_tickCo != null) StopCoroutine(_tickCo);
        _tickCo = null;
        _player = null;
    }

    IEnumerator TickDamage()
    {
        while (true)
        {
            if (_player == null) yield break;
            _player.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
