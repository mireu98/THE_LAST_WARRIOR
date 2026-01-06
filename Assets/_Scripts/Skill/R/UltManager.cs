using System.Collections;
using UnityEngine;

public class UltManager : MonoBehaviour
{
    public GameObject SwordPrefab;   // Ä® ÇÁ¸®ÆÕ
    public GameObject SpawnRangeOb;
    public GameObject Portal;         
    public GameObject HitBox;
    private Transform _portal;
    private CapsuleCollider _capsuleCollider;

    private float _spawnTime = 7f;
    private float _waitTime = 2f;


    void Start()
    {
        HitBox.SetActive(false);
        _portal = Portal.GetComponent<Transform>();
        _capsuleCollider = SpawnRangeOb.GetComponent<CapsuleCollider>();
        StartCoroutine(SpawnSword());
        StartCoroutine(OnHitBoxTimer());
        Destroy(gameObject, 10f);
    }

    IEnumerator OnHitBoxTimer()
    {
        float time = 0f;
        while (time < _waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        HitBox.SetActive(true);
    }


    IEnumerator SpawnSword()
    {
        float time = 0f;
        while (time < _spawnTime)
        {
            time += 0.1f;
            Vector3 pos = SpawnSwordXY();

            GameObject swordObj = Instantiate(SwordPrefab, pos, _portal.rotation*SwordPrefab.transform.rotation);

            var ult = swordObj.GetComponent<Ultimate>();
            if (ult != null)
                ult.Init(_portal);

            var clip = swordObj.GetComponent<UltiShader>();
            if (clip != null)
            {
                clip.Startportal = _portal;
                if (clip.SwordRenderer == null)
                    clip.SwordRenderer = swordObj.GetComponentInChildren<Renderer>();
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    Vector3 SpawnSwordXY()
    {
        Vector3 originPosition = SpawnRangeOb.transform.position;
        float halfX = _capsuleCollider.bounds.size.x * 0.5f;
        float halfY = _capsuleCollider.bounds.size.y * 0.5f;
        float halfZ = _capsuleCollider.bounds.size.z * 0.5f;

        float randX = Random.Range(-halfX, halfX);
        float randY = Random.Range(-halfY, halfY);
        float randZ = Random.Range(-halfZ, halfZ);

        return originPosition + new Vector3(randX, randY, randZ);
    }
}
