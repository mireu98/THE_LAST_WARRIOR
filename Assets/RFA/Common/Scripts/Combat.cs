using NaughtyCharacter;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Retro.ThirdPersonCharacter
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    public class Combat : MonoBehaviour
    {
        private const string attackTriggerName = "Attack";
        private const string specialAttackTriggerName = "Ability";

        private Animator _animator;
        private PlayerInput _playerInput;
        private CharacterController _cc;


        public float abilityMoveSpeed = 6f;      // 얼마나 빠르게 전진할지
        public float abilityMoveDuration = 0.3f; // 몇 초 동안 전진할지
        private Coroutine _abilityMoveRoutine;

        public bool AttackInProgress {get; private set;} = false;

        public BaseAttackHitbox AttackHitBox;   // HitBox
        public WeaponHitbox AbilityHitbox;   // AbilityHitBox

        // 애니메이션 이벤트 or 코드에서 호출
        public void EnableLightHitbox() => AttackHitBox.Enable();
        public void DisableLightHitbox() => AttackHitBox.Disable();

        public void EnableHeavyHitbox() => AbilityHitbox.Enable();
        public void DisableHeavyHitbox() => AbilityHitbox.Disable();

        private bool canAbility = true;
        private float _cooldown = 5f;
        private float _currentCooldown = 0f;
        
        private void Start()
        { 
            _animator = GetComponent<Animator>();
            _playerInput = GetComponent<PlayerInput>();
            _cc = GetComponent<CharacterController>();
            AttackHitBox.GetComponent<BoxCollider>().enabled = false;
            foreach(var col in AbilityHitbox.GetComponentsInChildren<BoxCollider>())
            {
                col.enabled = false;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlayerDead) return;

            bool grounded = _cc.isGrounded;
            if (_playerInput.AttackInput && !AttackInProgress && grounded)
            {
                GameManager.Instance.IsAttacking = true;
                Attack();
            }
            else if (_playerInput.SpecialAttackInput && !AttackInProgress && grounded && canAbility)
            {
                GameManager.Instance.IsAttacking = true;
                canAbility = false;
                SpecialAttack();
                StartCoroutine(StartCooldown());
            }

        }

        private void SetAttackStart()
        {
            AttackInProgress = true;
        }

        private void SetAttackEnd()
        {
            AttackInProgress = false;
        }

        private void Attack()
        {
            _animator.SetTrigger(attackTriggerName);
        }

        private void SpecialAttack()
        {
            _animator.SetTrigger(specialAttackTriggerName);
            StartAbilityMove();
        }

        IEnumerator StartCooldown()
        {
            _currentCooldown = _cooldown;

            while (_currentCooldown > 0f)
            {
                GameManager.Instance.UI?.ShowMRBCooldown(_currentCooldown, _cooldown);
                _currentCooldown -= Time.deltaTime;
                yield return null;
            }

            canAbility = true;
            GameManager.Instance.UI?.HideMRBCooldown();
        }

        public void StartAbilityMove()
        {
            if (_abilityMoveRoutine != null)
                StopCoroutine(_abilityMoveRoutine);

            _abilityMoveRoutine = StartCoroutine(AbilityMoveRoutine());

        }

        IEnumerator AbilityMoveRoutine()
        {
            float t = 0f;
            yield return new WaitForSeconds(0.8f);
            while (t < abilityMoveDuration)
            {
                // 플레이어가 바라보는 방향으로 전진
                Vector3 dir = transform.forward;

                _cc.Move(dir * abilityMoveSpeed * Time.deltaTime);

                t += Time.deltaTime;
                yield return null;
            }
        }

    }
}