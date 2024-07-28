using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using Infringed.Quests;
using UnityEngine;

namespace Infringed.UI
{
    public class LogUI : MonoBehaviour
    {
        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private QuestSet _questSet;
        [SerializeField] private PlayerController _player;
        //[SerializeField] private 

        private void Awake()
        {
            if (_questSet == null)
                _questSet = FindObjectOfType<QuestSet>();
        }
    }
}
