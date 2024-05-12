using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using Infringed.Player;
using UnityEngine;

namespace Infringed
{
    public class OnPlayerDeathActivator : MonoBehaviour
    {
        private Health _playerHealth;
        private bool _currentStatus = false;

        [Zenject.Inject]
        private void _SetHealth(PlayerController _player)
        {
            _playerHealth = _player.GetComponent<Health>();
        }

        private void Awake()
        {
            _SetStatus(false);

            _playerHealth.OnDeathEnd += _OnActivate;
        }

        private void OnDestroy()
        {
            _playerHealth.OnDeathEnd -= _OnActivate;
        }

        public void SwitchStatus()
        {
            _currentStatus = !_currentStatus;
            _SetStatus(_currentStatus);
        }

        private void _OnActivate()
        {
            _SetStatus(true);
        }

        private void _SetStatus(bool status)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(status);
            }
        }
    }
}
