using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    public float MaxHp;
    public float CurrentHp;
    public float Speed
    {
        get { return _ani.GetFloat("Speed"); }
        set { _ani.SetFloat("Speed", value); }
    }
    public float Damage;

    public float Exp;

    public float AttackRange = 2f;
    public float AttackCooldown = 1.5f;

    public bool isHit = false;
    public bool isDie = false;

    public GameObject HpBar;
    public GameObject HitBox;

    public LayerMask EnemyMask;

    protected Animator _ani;
    protected Transform _player;
    protected float attackTimer = 0f;



    private int isAttackingHash = Animator.StringToHash("isAttacking");
    private bool isAttackingParam = false;

    protected virtual void Awake()
    {
        _ani = GetComponent<Animator>();
        HitBox.GetComponent<BoxCollider>().enabled = false;
        HpBar.GetComponent<Image>().fillAmount = 1f;
        HpBar.SetActive(false);
        isAttackingParam = HasAnimatorParam(_ani, isAttackingHash);
    }

    protected virtual void Update()
    {
        // 공통 AI 처리
        if (_player == null)
        {
            var gm = GameManager.Instance;
            if (gm != null && gm.Player != null)
                _player = gm.Player.transform;
        }
        if (isDie) return;
        if (_player == null) return;
        if (GameManager.Instance.IsPlayerDead == true)
        {
            if (isAttackingParam && _ani.GetBool("isAttacking"))
            {
                _ani.SetBool("isAttacking", false);
                return;
            }
        }
        HpBar.GetComponent<Image>().fillAmount = CurrentHp / MaxHp;
    }

    protected virtual void FixedUpdate()
    {
        if (isDie) return;
        if (GameManager.Instance.IsPlayerDead)
        {
            if (isAttackingParam && _ani.GetBool("isAttacking"))
            {
                _ani.SetBool("isAttacking", false);
                return;
            }
        }
    }

    private bool HasAnimatorParam(Animator anim, int hash)
    {
        if (anim == null) return false;
        foreach (var param in anim.parameters)
            if (param.nameHash == hash) return true;
        return false;
    }

    public virtual void DamageToPlayer()
    {
        if (_player == null) return;
        var player = _player.GetComponent<PlayerAdd>();
        if (GameManager.Instance.IsDefense)
        {
            GameManager.Instance.IsDefense = false;
            player.GetComponent<Animator>().SetBool("isDefense", GameManager.Instance.IsDefense);
            player.DefenseEffect.Play();
            var playerController = _player.GetComponent<CharacterController>();
            Vector3 knockDir = (playerController.transform.position - transform.position).normalized;
            StartCoroutine(PlayerKnockBack(playerController, knockDir, 12f, 0.15f));
            return;
        }
        if (player == null) return;

        player.TakeDamage(Damage);
    }

    public virtual void TakeDamage(float dmg)
    {
        if (!HpBar.activeSelf) HpBar.SetActive(true);
        if (isDie) return;
        isHit = true;
        CurrentHp -= dmg;
        _ani.SetTrigger("Hit");
        if (CurrentHp <= 0)
        {
            HpBar.GetComponent<Image>().fillAmount = 0f;
            isDie = true;
            _ani.SetBool("isDie", isDie);
            Die();
        }
    }

    //애니메이션 이벤트
    public void EndHit()
    {
        isHit = false;
    }
    //애니메이션 이벤트
    public void AttackStart()
    {
        //HitBox.SetActive(false);
        //HitBox.SetActive(true);
        HitBox.GetComponent<BoxCollider>().enabled = true;
    }
    //애니메이션 이벤트
    public void AttackEnd()
    {
        //HitBox.SetActive(false);
        HitBox.GetComponent<BoxCollider>().enabled = false;
    }

    protected virtual void Die()
    {
        if (!isDie) return;
        NavMeshAgent obj = GetComponent<NavMeshAgent>();
        isHit = false;
        Speed = 0f;
        var cols = GetComponentsInChildren<Collider>(true);
        foreach (var col in cols)
        {
            col.enabled = false;
        }
        _ani.SetTrigger("Die");
        var gm = GameManager.Instance;
        gm.PlayerCurrentExp += Exp;
        obj.enabled = false;
        Destroy(gameObject, 3f);
    }

    IEnumerator PlayerKnockBack(CharacterController controller, Vector3 dir, float power, float duration)
    {
        float tick = 0f;
        while (tick < duration)
        {
            controller.Move(dir * power * Time.deltaTime);
            tick += Time.deltaTime;
            yield return null;
        }
    }
}
