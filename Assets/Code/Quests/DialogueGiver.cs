using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.AI;
using Infringed.Map;
using Infringed.Player;
using UnityEngine;

namespace Infringed.Quests
{
    public class DialogueGiver : MonoBehaviour, IInteractable
    {
        public event Action<DialogueGiver> OnDialogueEnd;

        [field: SerializeField] public List<DialogueChoice> DialogueChoices { get; private set; }
        [field: SerializeField] public AIManager AIManager { get; private set; }
        [field: SerializeField] public int CurrentIndex { get; set; }

        private void Awake()
        {
            AIManager.OnAlarm += EndDialogue;
        }

        private void OnDestroy()
        {
            AIManager.OnAlarm -= EndDialogue;
            OnDialogueEnd = null;
        }

        public void EndDialogue()
        {
            OnDialogueEnd?.Invoke(this);
        }

        public void Interact(PlayerController user)
        {
            if (AIManager.Alarm)
                return;

            user.InitiateDialogue(this);
        }
    }
}
