using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class EnemyAnimator : MonoBehaviour
    {
        private Animator _animator;
        private MovementController _movement;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<MovementController>();
        }

        private void Update()
        {
            _animator.SetBool("isMoving", _movement.IsMoving);
        }
    }
}
