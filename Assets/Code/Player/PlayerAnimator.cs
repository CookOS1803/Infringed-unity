using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Player
{
    public class PlayerAnimator
    {
        private Animator _animator;
        private Transform _transform;

        public PlayerAnimator(Transform playerTransform)
        {
            _transform = playerTransform;
            _animator = _transform.GetComponent<Animator>();

        }

        public void AnimateMovement(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
            {
                _animator.SetBool("isMoving", false);
                return;
            }

            float angle = Vector3.SignedAngle(Vector3.forward, _transform.forward, Vector3.up);
            moveDirection = Quaternion.Euler(0f, -angle, 0f) * moveDirection;

            _animator.SetBool("isMoving", true);
            _animator.SetFloat("forward", moveDirection.z, 0.1f, Time.deltaTime);
            _animator.SetFloat("right", moveDirection.x, 0.1f, Time.deltaTime);
        }

        public void Attack()
        {
            _animator.SetTrigger("Attack");
        }

        public void Die()
        {
            _animator.SetTrigger("death");
        }

        public void ResetAnimations()
        {
            _animator.SetBool("isMoving", false);
        }
    }
}