using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using Infringed.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class UIDialogueGiver : MonoBehaviour
    {
        public event Action OnInitiateDialogue;
        public event Action OnDialogueEnd;

        [SerializeField] private PlayerController _player;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Transform _choicesContainer;
        [SerializeField] private GameObject _choicePrefab;
        private DialogueGiver _giver;
        private ToggleableGraphics _toggle;

        private void Awake()
        {
            if (_player == null)
                _player = FindObjectOfType<PlayerController>();

            _toggle = GetComponent<ToggleableGraphics>();

            _player.OnInitiateDialogue += _OnInitiateDialogue;
        }

        private void Start()
        {
            _toggle.SetGraphicsStatus(false);
        }

        private void OnDestroy()
        {
            _player.OnInitiateDialogue -= _OnInitiateDialogue;
        }

        private void Update()
        {
        }

        private void _OnInitiateDialogue(DialogueGiver giver)
        {
            OnInitiateDialogue?.Invoke();

            giver.OnDialogueEnd += _OnDialogueEnd;
            _giver = giver;

            _toggle.SetGraphicsStatus(true);

            foreach (Transform c in _choicesContainer)
            {
                Destroy(c.gameObject);
            }

            _NewChoices(_giver.CurrentIndex);
        }

        private void _NewChoices(int index)
        {
            foreach (Transform c in _choicesContainer)
            {
                Destroy(c.gameObject);
            }

            if (index < 0)
            {
                _giver.EndDialogue();
                return;
            }

            var choice = _giver.DialogueChoices[index];

            _text.text = choice.Text;

            foreach (var i in choice.Options)
            {
                var instance = Instantiate(_choicePrefab, _choicesContainer).GetComponent<UIDialogueChoice>();
                instance.Text.text = i.Label;
                instance.Button.onClick.AddListener(() =>
                {
                    _NewChoices(i.Index);
                    i.Click();
                });
            }
        }

        private void _OnDialogueEnd(DialogueGiver giver)
        {
            OnDialogueEnd?.Invoke();

            giver.OnDialogueEnd -= _OnDialogueEnd;
            _giver = giver;

            _toggle.SetGraphicsStatus(false);
        }
    }
}
