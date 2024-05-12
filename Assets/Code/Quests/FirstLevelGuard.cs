using System.Collections;
using System.Collections.Generic;
using Infringed.AI;
using Infringed.Combat;
using Infringed.InventorySystem;
using Infringed.Map;
using Infringed.Player;
using TMPro;
using UnityEngine;

namespace Infringed.Quests
{
    public class FirstLevelGuard : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyToKill;
        [SerializeField] private Door _door;
        [SerializeField] private ItemData _key;
        [SerializeField] private GameObject _dialogueWindow;
        [SerializeField] private TMP_Text _text;
        [SerializeField, Min(0)] private float _playerTriggerRadius;
        private bool _playerHitGuard;
        private Health _health;

        private void OnEnable()
        {
            _enemyToKill.OnEnemyDeathEnd += _OnEnemyDeath;
        }

        private void OnDisable()
        {
            if (_enemyToKill)
                _enemyToKill.OnEnemyDeathEnd -= _OnEnemyDeath;

            if (_health)
                _health.OnDamageTaken -= _OnDamage;
        }

        private void Update()
        {
            var colliders = Physics.OverlapSphere(transform.position, _playerTriggerRadius, LayerMask.GetMask("Player"));

            if (colliders.Length == 0)
            {
                _dialogueWindow.SetActive(false);
                return;
            }

            _dialogueWindow.SetActive(true);

            var player = colliders[0].GetComponent<PlayerController>();

            transform.LookAt(player.transform);

            _CheckStatus(player);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _playerTriggerRadius);
        }

        private void _CheckStatus(PlayerController player)
        {
            if (_playerHitGuard)
            {
                _door.OpenIndefinitely();
                _text.text = "Alright, you're free to go";
                return;
            }

            // player has key
            foreach (Item i in player.Inventory)
            {
                if (i?.Data == _key)
                {
                    _text.text = "Oh, so you've got the key...";
                    return;
                }
            }

            if (_enemyToKill == null)
            {
                _text.text = "Excellent! Now hit me with your knife so they'll think i fought you back";
                return;
            }

            _text.text = "Hey, i could open the door, if you'd help me. I need the guy in the storage room dead, 'cause he denounced my brother and got him hanged";
        }

        private void _OnEnemyDeath(EnemyController sender)
        {
            _enemyToKill.OnEnemyDeathEnd -= _OnEnemyDeath;

            _health = gameObject.AddComponent<Health>();
            _health.OnDamageTaken += _OnDamage;
        }

        private void _OnDamage()
        {
            _health.OnDamageTaken -= _OnDamage;
            _playerHitGuard = true;
            Destroy(_health);
        }
    }
}
