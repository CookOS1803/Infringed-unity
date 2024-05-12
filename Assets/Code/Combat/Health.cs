using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public class Health : MonoBehaviour
    {
        public event Action OnChange;
        public event Action OnDeathStart;
        public event Action OnDeathEnd;
        public event Action OnDamageTaken;

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;

        public float NormalizedHealth => (float)_currentHealth / _maxHealth;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;

            OnChange?.Invoke();
            OnDamageTaken?.Invoke();

            if (_currentHealth <= 0)
            {
                OnDeathStart?.Invoke();
            }
        }

        public void TakeHealing(int healing)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth + healing, _currentHealth, _maxHealth);

            OnChange?.Invoke();
        }

        /// <summary>
        /// Called by DeathState
        /// </summary>
        public void DeathStateEnd()
        {
            OnDeathEnd?.Invoke();
        }
    }
}