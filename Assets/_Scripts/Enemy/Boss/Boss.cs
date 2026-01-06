using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject DragonBreath;
    private NavMeshAgent _agent;
    private ParticleSystem _dragonBreathParticle;
    private BoxCollider _dragonBreathCol;
    private Coroutine _pattern;
    private bool isAwake = false;
    private bool isFire = false;

    public CinemachineVirtualCamera Cam1;
    public CinemachineVirtualCamera Cam2;
    public CinemachineVirtualCamera DieCam;

    public CapsuleCollider BossCol;
    public Transform BreathFX;
    public Transform MouthAnchor;
    private Coroutine _breathLoop;
    private Coroutine _sweepCo;
    private Quaternion _breathBaseRot;
    private float _sweepDuration = 2.0f;
    private float _sweepAngle = 90f;
    private float _airPitchDown = 30f;
    private bool isSweepClockwise = false;
    private bool _breathLoopStarted = false;

    private float _groundBreathCooldown = 15f;
    private float _flyCooldown = 30f;

    private float _nextGroundBreathTime = 0f;
    private float _nextFlyTime = 0f;

    private BoxCollider _spawnRangeCol;
    public GameObject FireWall;
    public GameObject SpawnRangeOb;
    public GameObject GroundFireWall;

    private List<GameObject> _spawnedSkills = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        // ∫∏Ω∫ Ω∫≈»
        _dragonBreathParticle = DragonBreath.GetComponentInChildren<ParticleSystem>();
        _dragonBreathCol = DragonBreath.GetComponent<BoxCollider>();
        MaxHp = 200000f;
        CurrentHp = MaxHp;
        Damage = 5000f;
        AttackCooldown = 1.5f;
        Exp = 50000f;

        Speed = 0f;
        _dragonBreathCol.enabled = false;
        _dragonBreathParticle.Stop();
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.updateRotation = false;
        }
        _spawnRangeCol = SpawnRangeOb.GetComponent<BoxCollider>();
        Sleep();
    }
    private void LateUpdate()
    {
        if (MouthAnchor == null || BreathFX == null) return;
        BreathFX.position = MouthAnchor.position;
    }

    protected override void Update()
    {
        if (!isAwake) return;
        base.Update();
    }

    public void OnIdleEnter()
    {
        if (!isAwake || isDie) return;
        if (_breathLoopStarted) return;
        _breathLoopStarted = true;

        _breathLoop = StartCoroutine(BreathLoop());
    }

    private IEnumerator BreathLoop()
    {
        yield return new WaitForSeconds(0.3f);

        _nextGroundBreathTime = Time.time + _groundBreathCooldown;
        _nextFlyTime = Time.time + _flyCooldown;

        while (isAwake && !isDie)
        {
            if (GameManager.Instance.IsPlayerDead) yield break;
            if (isFire) { yield return null; continue; }

            bool canGround = Time.time >= _nextGroundBreathTime;
            bool canFly = Time.time >= _nextFlyTime;

            if (!canGround && !canFly)
            {
                float next = Mathf.Min(_nextGroundBreathTime, _nextFlyTime);
                while (Time.time < next)
                {
                    if (GameManager.Instance.IsPlayerDead) yield break;
                    yield return null;
                }
                continue;
            }

            if (canGround && canFly)
            {
                if (Random.value < 0.5f) canFly = false;
                else canGround = false;
            }

            if (canGround)
            {
                BossCol.enabled = false;
                isFire = true;
                _ani.SetTrigger("Fire");

                _nextGroundBreathTime = Time.time + _groundBreathCooldown;
            }
            else
            {
                BossCol.enabled = false;
                isFire = false;
                _ani.SetTrigger("Fly");

                _nextFlyTime = Time.time + _flyCooldown;
                _nextGroundBreathTime = Time.time + _groundBreathCooldown;
            }

            yield return null;
        }

        _breathLoop = null;
        _breathLoopStarted = false;
    }


    public void BossFlameAttackAnimEnd()
    {
        BossCol.enabled = true;
        isFire = false;
    }

    public void WakeUp()
    {
        _ani.SetTrigger("WakeUp");
    }

    private void SetBreathStartRotation()
    {
        if (BreathFX == null) return;

        float half = _sweepAngle * 0.5f;
        float startYaw = transform.eulerAngles.y + (isSweepClockwise ? -half : half);

        Vector3 dir = Quaternion.Euler(0f, startYaw, 0f) * Vector3.forward;

        Vector3 forward = Vector3.Cross(Vector3.up, dir);
        BreathFX.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    //In the Air Breath
    public void TurnOnDragonBreath_Air()
    {
        StartCoroutine(TurnOnBreathNextFrame());
    }

    private IEnumerator TurnOnBreathNextFrame()
    {
        _dragonBreathCol.enabled = true;
        _dragonBreathCol.enabled = false;
        BreathFX.position = MouthAnchor.position;
        _breathBaseRot = Quaternion.Euler(_airPitchDown, transform.eulerAngles.y, 0f);
        SetBreathStartRotationWithBase(_breathBaseRot);

        yield return new WaitForFixedUpdate();
        _dragonBreathCol.enabled = true;
        Physics.SyncTransforms();
        StartSweepWithBase(_breathBaseRot);
        _dragonBreathParticle.Play();
    }

    //In the Air Breath
    private void SetBreathStartRotationWithBase(Quaternion baseRot)
    {
        float half = _sweepAngle * 0.5f;
        float start = isSweepClockwise ? -half : half;
        BreathFX.rotation = baseRot * Quaternion.Euler(0f, start, 0f);
    }

    //In the Air Breath
    private void StartSweepWithBase(Quaternion baseRot)
    {
        if (_sweepCo != null) StopCoroutine(_sweepCo);
        _sweepCo = StartCoroutine(BreathSweepCo(baseRot));
    }

    //In the Air Breath
    private IEnumerator BreathSweepCo(Quaternion baseRot)
    {
        float half = _sweepAngle * 0.5f;
        float from = isSweepClockwise ? -half : half;
        float to = isSweepClockwise ? half : -half;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _sweepDuration;
            float yaw = Mathf.Lerp(from, to, t);

            BreathFX.rotation = baseRot * Quaternion.Euler(0f, yaw, 0f);

            yield return null;
        }

        TurnOffDragonBreath();
    }

    //On Ground Breath
    private IEnumerator BreathSweepCo()
    {
        float half = _sweepAngle * 0.5f;

        float from = isSweepClockwise ? -half : half;
        float to = isSweepClockwise ? half : -half;

        float t = 0f;

        Quaternion baseRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime / _sweepDuration;
            float yaw = Mathf.Lerp(from, to, t);

            BreathFX.rotation = baseRot * Quaternion.Euler(0f, yaw, 0f);
            yield return null;
        }
        TurnOffDragonBreath();
    }

    private void Sleep()
    {
        isAwake = false;
        attackTimer = 0f;
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }
        HitBox.GetComponent<Collider>().enabled = false;
    }

    public void StartMoveNavmeshAgent()
    {
        if (isAwake) return;
        _agent.enabled = true;
        _agent.isStopped = true;
        isAwake = true;
    }

    //On Ground Breath
    public void TurnOnDragonBreath()
    {
        _dragonBreathCol.enabled = true;
        _dragonBreathCol.enabled = false;
        SetBreathStartRotation();
        _dragonBreathCol.enabled = true;
        _dragonBreathParticle.Play();
        if (_sweepCo != null) StopCoroutine(_sweepCo);
        _sweepCo = StartCoroutine(BreathSweepCo());
    }

    public void TurnOffDragonBreath()
    {
        _dragonBreathCol.enabled = false;
        _dragonBreathParticle.Stop();
        if (_sweepCo != null)
        {
            StopCoroutine(_sweepCo);
            _sweepCo = null;
        }
    }

    public void PatternStart()
    {
        if (_pattern != null) return;
        _pattern = StartCoroutine(PatternCo());
    }

    IEnumerator PatternCo()
    {
        while (isAwake && !isDie)
        {
            for (int i = 0; i < 10; i++)
            {
                var gfw = Instantiate(GroundFireWall, SpawnSwordXY(), Quaternion.identity);
                _spawnedSkills.Add(gfw);
            }

            yield return new WaitForSecondsRealtime(8f);

            var fw = Instantiate(FireWall, transform.position + new Vector3(-12, 0, 0), Quaternion.identity);
            _spawnedSkills.Add(fw);
            var fw2 = Instantiate(FireWall, transform.position + new Vector3(12, 0, 0), Quaternion.identity);
            _spawnedSkills.Add(fw2);

            yield return new WaitForSecondsRealtime(5f);
        }

        _pattern = null;
    }

    public void CutScene1()
    {
        CutsceneLockPlayer();
        Cam1.Priority = 11;
    }

    public void CutScene2()
    {
        Cam1.Priority = 1;
        Cam2.Priority = 11;
    }

    public void EndCutScene()
    {
        CutsceneUnlockPlayer();
        Cam1.Priority = 1;
        Cam2.Priority = 1;
    }

    private void CutsceneLockPlayer()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.IsCutscene = true;
    }

    private void CutsceneUnlockPlayer()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.IsCutscene = false;
    }

    Vector3 SpawnSwordXY()
    {
        Vector3 originPosition = SpawnRangeOb.transform.position;
        float halfX = _spawnRangeCol.bounds.size.x * 0.5f;
        float halfY = _spawnRangeCol.bounds.size.y * 0.5f;
        float halfZ = _spawnRangeCol.bounds.size.z * 0.5f;

        float randX = Random.Range(-halfX, halfX);
        float randY = Random.Range(-halfY, halfY);
        float randZ = Random.Range(-halfZ, halfZ);

        return originPosition + new Vector3(randX, randY, randZ);
    }

    private void CleanupSkills()
    {
        StopAllCoroutines();

        for (int i = _spawnedSkills.Count - 1; i >= 0; i--)
        {
            var skill = _spawnedSkills[i];
            if (skill == null) continue;

            foreach (var col in skill.GetComponentsInChildren<Collider>())
                col.enabled = false;

            foreach (var ps in skill.GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            foreach (var render in skill.GetComponentsInChildren<Renderer>())
                render.enabled = false;

            Destroy(skill);
        }

        _spawnedSkills.Clear();
    }

    protected override void Die()
    {
        if (!isDie) return;
        SkillRegistry.Instance?.ClearAll();

        CleanupSkills();
        StopAllCoroutines();

        if (_dragonBreathCol != null)
            _dragonBreathCol.enabled = false;

        if (_dragonBreathParticle != null)
            _dragonBreathParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (BossCol != null)
            BossCol.enabled = false;
        DieCam.Priority = 11;
        base.Die();
        var gm = GameManager.Instance;
        gm.isGameClear = true;
    }

}
