using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.InventorySystem;
using Infringed.Player;
using Infringed.Quests;
using UnityEngine;

namespace Infringed.UI
{
    public class LogUI : MonoBehaviour
    {
        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private Transform _entryParent;
        [SerializeField] private QuestSet _questSet;
        [SerializeField] private Color _questColor;
        [SerializeField] private PlayerController _player;
        [SerializeField] private Color _inventoryColor;
        [SerializeField] private Color _smallInventoryColor;
        [SerializeField] private Color _essentiaColor;

        private void Awake()
        {
            if (_questSet == null)
                _questSet = FindObjectOfType<QuestSet>();

            if (_player == null)
                _player = FindObjectOfType<PlayerController>();

            _questSet.OnQuestAdd += _OnQuestAdd;
            _player.Inventory.OnItemAdd += _OnItemAdd;
            _player.Inventory.OnImportantItemAdd += _OnImportantItemAdd;
            _player.Inventory.OnItemAddFailure += _OnItemAddFailure;
            _player.OnEssentia += _OnEssentia;
        }

        private void OnDestroy()
        {
            _questSet.OnQuestAdd -= _OnQuestAdd;
            _player.Inventory.OnItemAdd -= _OnItemAdd;
            _player.Inventory.OnImportantItemAdd -= _OnImportantItemAdd;
            _player.Inventory.OnItemAddFailure -= _OnItemAddFailure;
            _player.OnEssentia -= _OnEssentia;
        }

        private void _OnItemAdd(InventoryItemInstance item)
        {
            _OnImportantItemAdd(item.Data);
        }

        private void _OnImportantItemAdd(ItemData data)
        {
            var message = "Подобран предмет: " + data.Name;
            _Log(message, _inventoryColor);
        }

        private void _OnItemAddFailure(ItemData data)
        {
            _Log("Нехватает места в инвентаре", _smallInventoryColor);
        }

        private void _OnEssentia()
        {
            _Log("Получена 1 эссенция", _essentiaColor);
        }

        private void _OnQuestAdd(Quest quest)
        {
            _Log(quest.Label, _questColor);
        }

        private void _Log(string message, Color color)
        {
            var entry = Instantiate(_entryPrefab, _entryParent).GetComponent<LogEntryUI>();
            entry.Initialize(message, color);
        }        
    }
}
