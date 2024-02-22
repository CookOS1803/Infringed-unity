using System;
using System.Collections;
using UnityEngine;

namespace Infringed.Combat
{
    public class StunController : MonoBehaviour
    {
        public bool IsStunned { get; private set; } = false;
        private float _stunClock = 0f;
        private float _currentStunTime = 0f;

        public event Action OnStunStart;
        public event Action OnStunEnd;

        public void Stun(float stunTime)
        {
            if (!IsStunned)
            {
                _currentStunTime = stunTime;
                IsStunned = true;

                StartCoroutine(_StunRoutine());
            }
            else if (stunTime >= _currentStunTime - _stunClock)
            {
                _stunClock = 0f;
                _currentStunTime = stunTime;
            }
        }

        private IEnumerator _StunRoutine()
        {
            OnStunStart?.Invoke();

            while (_stunClock < _currentStunTime)
            {
                _stunClock += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            _stunClock = 0f;
            IsStunned = false;

            OnStunEnd?.Invoke();
        }
    }
}
