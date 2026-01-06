using System.Collections;
using UnityEngine;
public class ShootSword : MonoBehaviour
{
    public Transform Sword;       // 검 Transform
    public Transform Portal;      // 포탈 Transform
    private float _readyDistance = 1.5f;   // 포탈에서 얼마나 앞으로 뽑을지
    private float _readyTime = 2f;    // 앞으로 나오는 데 걸리는 시간

    private float _shootDistance = 20f;   // 포탈에서 얼마나 앞으로 뽑을지
    private float _shootTime = 0.75f;    // 앞으로 나오는 데 걸리는 시간
    private bool exploded = false;

    public GameObject Ps;
    public GameObject HitPortal;
    private Rigidbody _rb;
    private BoxCollider HitPortalAttackTime;
    private HitPortalDamage _hitPortalDamage;

    private float _dmg;
    public float Dmg { get => _dmg; set => _dmg = value; }


    private void Awake()
    {
        _rb = Sword.GetComponent<Rigidbody>();
        HitPortalAttackTime = HitPortal.GetComponent<BoxCollider>();
        HitPortalAttackTime.enabled = false;
        _hitPortalDamage = HitPortal.GetComponent<HitPortalDamage>();
    }

    private void Start()
    {
        Dmg = GameManager.Instance.PlayerDmg * 3f;
        StartCoroutine(ShootAndDrop());
    }

    IEnumerator ShootAndDrop()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        Vector3 start = Sword.position;
        Vector3 end = start + Portal.forward * _readyDistance;

        float tick = 0f;
        while (tick < 1f)
        {
            tick += Time.deltaTime / _readyTime;
            Sword.position = Vector3.Lerp(start, end, tick);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        tick = 0f;
        start = Sword.position;
        end = start + Portal.forward * _shootDistance;
        

        while (tick < 1f)
        {
            tick += Time.deltaTime / _shootTime;
            Sword.position = Vector3.Lerp(start, end, tick);
            if (!exploded && Vector3.Distance(Sword.position, HitPortal.transform.position) < 0.2f)
            {
                exploded = true;
                Ps.SetActive(true);
                HitPortalAttackTime.enabled = true;
                _hitPortalDamage.SetDamage(Dmg);
            }
            yield return null;
        }
            HitPortalAttackTime.enabled = false;
            Ps.SetActive(false);
    }
}
