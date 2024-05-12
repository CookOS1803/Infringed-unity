using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public class DropItemOnDeath : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDeathEnd += OnDeathEnd;
        }

        private void OnDeathEnd()
        {
            Instantiate(_itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
