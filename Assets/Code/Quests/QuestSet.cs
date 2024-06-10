using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Quests
{
    public class QuestSet : MonoBehaviour
    {
        public event Action<Quest> OnQuestAdd;

        [SerializeField] private List<Quest> _questList;
        private HashSet<Quest> _quests = new();

        private void Awake()
        {
            _questList.ForEach(quest => AddQuest(quest));
            _questList.Clear();
            _questList = null;
        }

        public void AddQuest(Quest quest)
        {
            _quests.Add(quest);

            OnQuestAdd?.Invoke(quest);
        }
    }

    [Serializable]
    public class Quest
    {
        public string Label;
    }
}
