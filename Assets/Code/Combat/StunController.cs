using System;
using System.Collections;
using UnityEngine;

namespace Infringed.Combat
{
    public class StunController : MonoBehaviour
    {
        public bool IsStunned { get; private set; } = false;
        public Vector3 StunSourceDirection { get; private set; }
        private float _stunClock = 0f;
        private float _currentStunTime = 0f;

        public event Action OnStunStart;
        public event Action OnStunEnd;

        private void Update()
        {
            if (!IsStunned)
                return;

            _stunClock += Time.deltaTime;

            if (_stunClock >= _currentStunTime)
            {
                _stunClock = 0f;
                IsStunned = false;    
                OnStunEnd?.Invoke();
            }
        }

        public void Stun(float stunTime, Vector3 stunSourcePosition)
        {
            if (!IsStunned)
            {
                _currentStunTime = stunTime;

                IsStunned = true;
                OnStunStart?.Invoke();
            }
            else if (stunTime >= _currentStunTime - _stunClock)
            {
                _stunClock = 0f;
                _currentStunTime = stunTime;
            }

            StunSourceDirection = (stunSourcePosition - transform.position).normalized;
        }
    }
}
