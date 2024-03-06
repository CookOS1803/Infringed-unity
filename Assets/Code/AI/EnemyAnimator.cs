using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using UnityEngine;

namespace Infringed.AI
{
    public class EnemyAnimator : MonoBehaviour
    {
        private Animator _animator;
        private MovementController _movement;
        private EnemyController _enemy;
        private StunController _stunController;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<MovementController>();
            _enemy = GetComponent<EnemyController>();
            _stunController = GetComponent<StunController>();
        }

        private void OnEnable()
        {
            _enemy.OnAttackStart += _OnAttackStart;
        }

        private void OnDisable()
        {
            _enemy.OnAttackStart -= _OnAttackStart;
        }

        private void Update()
        {
            _animator.SetBool("isMoving", _movement.IsMoving);
            _animator.SetBool("isAlarmed", _enemy.IsAlarmed);
            _animator.SetBool("isStunned", _stunController.IsStunned);
            _animator.SetBool("isDying", _enemy.IsDying);
        }

        private void _OnAttackStart(EnemyController controller)
        {
            _animator.SetTrigger("attack");
        }
    }
}
