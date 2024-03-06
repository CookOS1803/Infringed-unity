using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;

namespace Infringed
{
    public class OnPlayerDeathActivator : MonoBehaviour
    {
        [Zenject.Inject] private PlayerController _player;
        private bool _currentStatus = false;

        private void Awake()
        {
            _SetStatus(false);

            _player.OnPlayerDeathEnd += _OnActivate;
        }

        private void OnDestroy()
        {
            _player.OnPlayerDeathEnd -= _OnActivate;
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

        public void SwitchStatus()
        {
            _currentStatus = !_currentStatus;
            _SetStatus(_currentStatus);
        }
    }
}
