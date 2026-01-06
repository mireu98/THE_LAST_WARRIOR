using Retro.ThirdPersonCharacter;
using System.Collections;
using UnityEngine;

public class PlayerAdd : MonoBehaviour
{
    private Animator _ani;
    private CharacterController _cc;
    private PlayerInput _playerInput;

    public bool AttackInProgress { get; private set; } = false;
    [SerializeField] ParticleSystem LevelupParticle;
    // Q
    [Header("Q")]
    public GameObject ShootSwordSkill;
    private bool canUseQSkill = true;
    private float _cooldownQ = 7f;
    private float _currentCooldownQ = 0f;

    // E
    [Header("E")]
    public GameObject Mobbing;
    private bool canUseESkill = true;
    private float _cooldownE = 10f;
    private float _currentCooldownE = 0f;

    // R
    [Header("R")]
    public GameObject Ult;
    private bool canUseRSkill = true;
    private float _cooldownR = 15f;
    private float _currentCooldownR = 0f;

    // Potion
    [Header("Potion")]
    private float _hpPotionCD = 9f;
    private float _hpPotionCurrentCD;
    private bool isHpPotionUse = false;


    private float _mpPotionCD = 12f;
    private float _mpPotionCurrentCD;
    private bool isMpPotionUse = false;


    public ParticleSystem DefenseEffect;

    public AudioSource AudioSource;

    public AudioClip LightAttackSfx;
    public AudioClip HeavyAttackSfx;
    public AudioClip LevelUpSfx;

    private void Start()
    {
        LevelupParticle.Stop();
        DefenseEffect.Stop();
        _cc = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _ani = GetComponent<Animator>();
    }

    private void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.IsPlayerDead) return;
        if (gm.PlayerCurrentExp >= gm.PlayerMaxExp)
            LevelUp();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (gm.PlayerLevel < 3) return;
            if (!canUseQSkill) return;
            if (gm.PlayerCurrentMp < gm.NeedQMp) return;
            if (!CanUseSkillNow()) return;
            gm.IsAttacking = true;
            canUseQSkill = false;
            gm.PlayerCurrentMp -= gm.NeedQMp;

            _ani.SetTrigger("ShootSword");
            Quaternion spawnRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            Instantiate(ShootSwordSkill, transform.position + new Vector3(0, 3, 0), spawnRot);
            StartCoroutine(QCooldown());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (gm.PlayerLevel < 6) return;
            if (!canUseESkill) return;
            if (gm.PlayerCurrentMp < gm.NeedEMp) return;
            if (!CanUseSkillNow()) return;
            gm.IsAttacking = true;
            canUseESkill = false;
            gm.PlayerCurrentMp -= gm.NeedEMp;

            _ani.SetTrigger("ShootSword");
            Quaternion spawnRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            Instantiate(Mobbing, transform.position + transform.forward * 4f, spawnRot);

            StartCoroutine(ECooldown());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gm.PlayerLevel < 10) return;
            if (!canUseRSkill) return;
            if (gm.PlayerCurrentMp < gm.NeedRMp) return;
            if (!CanUseSkillNow()) return;
            gm.IsAttacking = true;
            canUseRSkill = false;
            gm.PlayerCurrentMp -= gm.NeedRMp;

            _ani.SetTrigger("ShootSword");
            Quaternion spawnRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            Instantiate(Ult, transform.position + transform.forward * -3f + new Vector3(0, 3, 0), spawnRot);

            StartCoroutine(RCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isHpPotionUse) return;
            isHpPotionUse = true;

            gm.PlayerCurrentHp = Mathf.Min(gm.PlayerCurrentHp + gm.HealHp, gm.PlayerMaxHp);
            StartCoroutine(HpPotionCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isMpPotionUse) return;
            isMpPotionUse = true;

            gm.PlayerCurrentMp = Mathf.Min(gm.PlayerCurrentMp + gm.HealMp, gm.PlayerMaxMp);
            StartCoroutine(MpPotionCooldown());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (gm.IsDefense) return;
            if (gm.IsAttacking) return;
            gm.IsDefense = true;
            _ani.SetBool("isDefense", gm.IsDefense);
            _ani.SetTrigger("Defense");

        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!gm.IsDefense) return;
            gm.IsDefense = false;
            _ani.SetBool("isDefense", gm.IsDefense);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gm.isESCClicked = !gm.isESCClicked;
            if(gm.isESCClicked)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                gm.EscUI.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                gm.EscUI.SetActive(false);
            }
        }

        // 테스트용 레벨업
        if (Input.GetKey(KeyCode.P))
        {
            gm.PlayerCurrentExp += 300f;
            if (gm.PlayerCurrentExp >= gm.PlayerMaxExp)
                LevelUp();
        }
    }

    //애니메이션
    public void SetIsAttackingAni()
    {
        GameManager.Instance.IsAttacking = false;
    }

    private bool CanUseSkillNow()
    {
        bool grounded = _cc != null && _cc.isGrounded;
        if (!grounded) return false;
        if (_playerInput != null && _playerInput.AttackInput) return false;
        if (AttackInProgress) return false;
        return true;
    }

    private IEnumerator QCooldown()
    {
        _currentCooldownQ = _cooldownQ;
        while (_currentCooldownQ > 0f)
        {
            GameManager.Instance.UI?.ShowQCooldown(_currentCooldownQ, _cooldownQ);
            _currentCooldownQ -= Time.deltaTime;
            yield return null;
        }
        canUseQSkill = true;
        GameManager.Instance.UI?.HideQCooldown();
    }

    private IEnumerator ECooldown()
    {
        _currentCooldownE = _cooldownE;
        while (_currentCooldownE > 0f)
        {
            GameManager.Instance.UI?.ShowECooldown(_currentCooldownE, _cooldownE);
            _currentCooldownE -= Time.deltaTime;
            yield return null;
        }
        canUseESkill = true;
        GameManager.Instance.UI?.HideECooldown();
    }

    private IEnumerator RCooldown()
    {
        _currentCooldownR = _cooldownR;
        while (_currentCooldownR > 0f)
        {
            GameManager.Instance.UI?.ShowRCooldown(_currentCooldownR, _cooldownR);
            _currentCooldownR -= Time.deltaTime;
            yield return null;
        }
        canUseRSkill = true;
        GameManager.Instance.UI?.HideRCooldown();
    }

    private IEnumerator HpPotionCooldown()
    {
        _hpPotionCurrentCD = _hpPotionCD;
        while (_hpPotionCurrentCD > 0f)
        {
            GameManager.Instance.UI?.ShowHpPotion(_hpPotionCurrentCD, _hpPotionCD);
            _hpPotionCurrentCD -= Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.UI?.HideHpPotion();
        isHpPotionUse = false;
    }

    private IEnumerator MpPotionCooldown()
    {
        _mpPotionCurrentCD = _mpPotionCD;
        while (_mpPotionCurrentCD > 0f)
        {
            GameManager.Instance.UI?.ShowMpPotion(_mpPotionCurrentCD, _mpPotionCD);
            _mpPotionCurrentCD -= Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.UI?.HideMpPotion();
        isMpPotionUse = false;
    }

    public void LevelUp()
    {
        var gm = GameManager.Instance;

        float tempExp = gm.PlayerCurrentExp - gm.PlayerMaxExp;
        gm.PlayerMaxExp *= 1.25f;

        gm.PlayerMaxHp += 750f;
        gm.PlayerMaxMp += 550f;

        gm.PlayerCurrentExp = tempExp;
        gm.PlayerCurrentHp = gm.PlayerMaxHp;
        gm.PlayerCurrentMp = gm.PlayerMaxMp;

        gm.PlayerDmg += 150f;

        gm.NeedQMp += 50f;
        gm.NeedEMp += 70f;
        gm.NeedRMp += 100f;

        gm.HealHp += 300f;
        gm.HealMp += 300f;

        gm.PlayerLevel++;

        PlayLevelUpFX();
    }

    public void PlayLevelUpFX()
    {
        if (LevelupParticle)
        {
            LevelupParticle.Play();
            AudioSource.pitch = 5f;
            AudioSource.PlayOneShot(LevelUpSfx);
            AudioSource.pitch = 1f;
        }
    }

    public void TakeDamage(float damage)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.IsPlayerDead) return;

        _ani.SetTrigger("Hit");
        gm.PlayerCurrentHp -= damage;

        if (gm.PlayerCurrentHp <= 0f)
        {
            gm.PlayerCurrentHp = 0f;
            gm.IsPlayerDead = true;
            SkillRegistry.Instance?.ClearAll();
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            _ani.SetBool("isDead", true);
            _ani.SetTrigger("Dead");
        }
    }
    public void LightAttackSFX()
    {
        AudioSource.PlayOneShot(LightAttackSfx);
    }

    public void HeavyAttackSFX()
    {
        AudioSource.PlayOneShot(HeavyAttackSfx);
    }
}
