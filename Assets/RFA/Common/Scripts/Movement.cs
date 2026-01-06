using UnityEngine;

namespace Retro.ThirdPersonCharacter
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Combat))]
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        private Animator _animator;
        private PlayerInput _playerInput;
        private Combat _combat;
        private CharacterController _characterController;

        private Vector2 lastMovementInput;
        private Vector3 moveDirection = Vector3.zero;

        public float gravity = 10;
        private float jumpSpeed = 5;

        private float MaxSpeed = 3;
        private float DecelerationOnStop = 0.00f;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _playerInput = GetComponent<PlayerInput>();
            _combat = GetComponent<Combat>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlayerDead) return;
            if (GameManager.Instance != null && GameManager.Instance.IsCutscene)
                return;

            if (_animator == null) return;

            if (_combat.AttackInProgress)
            {
                StopMovementOnAttack();
            }
            else
            {
                if (GameManager.Instance.IsDefense) return;
                Move();
            }

        }
        private void Move()
        {
            var x = _playerInput.MovementInput.x;
            var y = _playerInput.MovementInput.y;

            bool grounded = _characterController.isGrounded;

            if (grounded && moveDirection.y < 0f)
            {
                moveDirection.y = -2f;
            }

            Vector3 input = new Vector3(x, 0, y);
            input = transform.TransformDirection(input);
            input *= MaxSpeed;

            moveDirection.x = input.x;
            moveDirection.z = input.z;

            if (grounded && _playerInput.JumpInput)
            {
                moveDirection.y = jumpSpeed * 1.5f;
            }

            moveDirection.y -= gravity * 1.5f * Time.deltaTime;

            _characterController.Move(moveDirection * MaxSpeed * Time.deltaTime);

            _animator.SetFloat("InputX", x);
            _animator.SetFloat("InputY", y);
            _animator.SetBool("IsInAir", !grounded);
        }


        private void StopMovementOnAttack()
        {
            var temp = lastMovementInput;
            temp.x -= DecelerationOnStop;
            temp.y -= DecelerationOnStop;
            lastMovementInput = temp;

            _animator.SetFloat("InputX", lastMovementInput.x);
            _animator.SetFloat("InputY", lastMovementInput.y);
        }
    }
}