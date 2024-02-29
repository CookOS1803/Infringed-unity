using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class EnemyAnimator : MonoBehaviour
    {
        private Animator _animator;
        private MovementController _movement;
        private EnemyController _enemy;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<MovementController>();
            _enemy = GetComponent<EnemyController>();
        }

        private void OnEnable()
        {
            _enemy.OnAlarm += _OnAlarm;
            _enemy.OnUnalarm += _OnUnalarm;
        }

        private void OnDisable()
        {
            _enemy.OnAlarm -= _OnAlarm;
            _enemy.OnUnalarm -= _OnUnalarm;
        }

        private void Update()
        {
            _animator.SetBool("isMoving", _movement.IsMoving);
        }

        private void _OnAlarm(EnemyController sender)
        {
            _animator.SetBool("isAlarmed", true);
        }

        private void _OnUnalarm(EnemyController sender)
        {
            _animator.SetBool("isAlarmed", false);
        }
    }
}
