using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed.AI
{
    public class AIManager : MonoBehaviour
    {
        public bool Alarm { get; private set; }
        private HashSet<EnemyController> _enemies;

        private void Awake()
        {
            _enemies = FindObjectsOfType<EnemyController>().ToHashSet();
        }

        private void OnEnable()
        {
            foreach (var enemy in _enemies)
            {
                _Subscribe(enemy);
            }
        }

        private void OnDisable()
        {
            foreach (var enemy in _enemies)
            {
                _Unsubscribe(enemy);
            }
        }

        private void _Subscribe(EnemyController enemy)
        {
            enemy.OnAlarm += _OnAlarm;
            enemy.OnDeath += _OnDeath;
        }

        private void _Unsubscribe(EnemyController enemy)
        {
            enemy.OnAlarm -= _OnAlarm;
            enemy.OnDeath -= _OnDeath;
        }

        private void _OnAlarm(EnemyController sender)
        {
            Alarm = true;

            foreach (var enemy in _enemies)
            {
                enemy.OnAlarm -= _OnAlarm;
                enemy.Alarm();
            }
        }

        private void _OnDeath(EnemyController sender)
        {
            _enemies.Remove(sender);

            _Unsubscribe(sender);
        }
    }
}
