using System.Collections;
using UnityEngine;

public class Ultimate : MonoBehaviour
{
    private Transform _portal;       // 포탈 위치/방향
    private Rigidbody _rb;

    private float _readyDistance = 1.5f;
    private float _readyTime = 2f;

    private float _shootDistance = 15f;
    private float _shootTime = 0.5f;

    // UltManager에서 생성 직후 호출해 줄 초기화 함수
    public void Init(Transform portal)
    {
        _portal = portal;
        StartCoroutine(ShootAndDrop());
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 3f);
    }

    IEnumerator ShootAndDrop()
    {
        if (_portal == null) yield break;  

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        
        Vector3 start = transform.position;
        Vector3 end = start + _portal.forward * _readyDistance;

        float t = 0f;
        while (t < 1f)
        {
            if (_portal == null)
            {
                Destroy(gameObject);
                yield break;
            }

            t += Time.deltaTime / _readyTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (_portal == null)
        {
            Destroy(gameObject);
            yield break;
        }

        t = 0f;
        start = transform.position;
        end = start + _portal.forward * _shootDistance;

        while (t < 1f)
        {
            if (_portal == null)
            {
                Destroy(gameObject);
                yield break;
            }

            t += Time.deltaTime / _shootTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
}
