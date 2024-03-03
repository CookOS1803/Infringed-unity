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
        [SerializeField, Min(0f)] private float _alarmTime = 10f;
        private HashSet<EnemyController> _enemies;
        private float _alarmClock;

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

        private void Update()
        {
            if (!Alarm)
                return;
            
            _alarmClock += Time.deltaTime;

            if (_alarmClock >= _alarmTime)
            {
                _Unalarm();
            }
        }

        private void _Subscribe(EnemyController enemy)
        {
            enemy.OnAlarm += _OnAlarm;
            enemy.OnDeath += _OnEnemyDeath;
            enemy.OnPlayerSpotted += _OnPlayerSpotted;
        }

        private void _Unsubscribe(EnemyController enemy)
        {
            enemy.OnAlarm -= _OnAlarm;
            enemy.OnDeath -= _OnEnemyDeath;
            enemy.OnPlayerSpotted -= _OnPlayerSpotted;
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

        private void _OnEnemyDeath(EnemyController sender)
        {
            _enemies.Remove(sender);

            _Unsubscribe(sender);
        }

        private void _OnPlayerSpotted(EnemyController sender, Vector3 vector)
        {
            _alarmClock = 0f;

            foreach (var enemy in _enemies)
            {
                enemy.LastKnownPlayerPosition = vector;
            }
        }

        private void _Unalarm()
        {
            Alarm = false;
            _alarmClock = 0f;

            foreach (var enemy in _enemies)
            {
                enemy.OnAlarm += _OnAlarm;
                enemy.Unalarm();
            }
        }
    }
}
