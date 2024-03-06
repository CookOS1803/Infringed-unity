using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private PlayerController _player;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _player = GetComponent<PlayerController>();

        }

        private void OnEnable()
        {
            _player.OnPlayerAttackStart += _Attack;
            _player.OnPlayerDeathStart += _Die;
        }

        private void OnDisable()
        {
            _player.OnPlayerAttackStart -= _Attack;
            _player.OnPlayerDeathStart -= _Die;
        }

        private void Update()
        {
            _animator.enabled = !_player.IsHiding;

            if (_animator.enabled)
                _AnimateMovement(_player.MoveDirection);
        }

        private void _Attack()
        {
            if (_animator.enabled)
                _animator.SetTrigger("Attack");
        }

        private void _Die()
        {
            if (_animator.enabled)
                _animator.SetTrigger("death");
        }

        private void _AnimateMovement(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
            {
                _animator.SetBool("isMoving", false);
                return;
            }

            float angle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
            moveDirection = Quaternion.Euler(0f, -angle, 0f) * moveDirection;

            _animator.SetBool("isMoving", true);
            _animator.SetFloat("forward", moveDirection.z, 0.1f, Time.deltaTime);
            _animator.SetFloat("right", moveDirection.x, 0.1f, Time.deltaTime);
        }
    }
}