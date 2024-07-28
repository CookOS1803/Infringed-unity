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
        private Health _health;
        private DialogueGiver _giver;
        private FirstLevelGuardsSounds _sound;
        private bool _tookQuest;

        private void Awake()
        {
            _giver = GetComponent<DialogueGiver>();
            _sound = GetComponent<FirstLevelGuardsSounds>();
        }

        private void Start()
        {
            _giver.DialogueChoices[0].Options[0].OnClicked += choice =>
            {
                _giver.DialogueChoices[0].Text = "Приветствую. Разобрался с нашим общим знакомым?";
                choice.Label = "Пока нет";

                _giver.DialogueChoices[1].Text = "Не буду тебя торопить, но знай, что в долгу не останусь.";

                choice.ClearCallback();

                _tookQuest = true;

                choice.OnClicked += choice =>
                {
                    _giver.DialogueChoices[1].Options[0].Label = "Хорошо";

                    choice.ClearCallback();
                };
            };
        }

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

        private void _OnEnemyDeath(EnemyController sender)
        {
            _enemyToKill.OnEnemyDeathEnd -= _OnEnemyDeath;

            if (!_tookQuest)
            {
                _giver.DialogueChoices[0].Options[0].ClearCallback();
                _giver.DialogueChoices[0].Options[0].Label = "Уже";
                _giver.DialogueChoices[1].Text = "Неужели? Тогда я готов помочь тебе. Прежде чем я тебя пропущу, ты должен ударить меня, чтобы меня не заподозрили в том, что я не сопротивлялся беглецу.";
                
            }
            else
            {
                _giver.DialogueChoices[0].Options[0].Label = "Да";
                _giver.DialogueChoices[1].Text = "Отлично! Прежде чем я тебя пропущу, ты должен ударить меня, чтобы меня не заподозрили в том, что я не сопротивлялся беглецу.";
            }
            _giver.DialogueChoices[1].Options[0].ClearCallback();
            _giver.DialogueChoices[1].Options[0].Label = "Крепись";
            _giver.DialogueChoices[1].Options[0].OnClicked += choice =>
            {
                _health = gameObject.AddComponent<Health>();
                _health.OnDamageTaken += _OnDamage;
                _sound.Initialize(_health);
                _giver.CurrentIndex = 1;

                choice.ClearCallback();
            };
        }

        private void _OnDamage()
        {
            _health.OnDamageTaken -= _OnDamage;

            _giver.DialogueChoices[1].Text = "Шрам конечно останется, но свою часть сделки ты выполнил, теперь моя очередь. Можешь идти.";
            _giver.DialogueChoices[1].Options[0].Label = "Может ещё увидимся";
            _giver.DialogueChoices[1].Options[0].OnClicked += choice =>
            {
                _door.OpenIndefinitely();

                choice.ClearCallback();
            };
        }
    }
}
