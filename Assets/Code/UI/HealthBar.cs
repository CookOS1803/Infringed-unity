using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using Infringed.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infringed.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _bar;
        private Health _playerHealth;

        [Inject]
        private void _SetHealth(PlayerController player)
        {
            _playerHealth = player.GetComponent<Health>();

            _playerHealth.OnChange += _UpdateBar;

        }

        private void _UpdateBar()
        {
            _bar.fillAmount = _playerHealth.NormalizedHealth;
        }
    }
}
