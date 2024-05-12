using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed.AI
{
    public class AIManager : MonoBehaviour
    {
        public bool Alarm { get; private set; }
        public float AlarmClock { get; private set; }
        [field: SerializeField, Min(0f)] public float AlarmTime { get; private set; } = 10f;
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

        private void Update()
        {
            if (!Alarm)
                return;
            
            AlarmClock += Time.deltaTime;

            if (AlarmClock >= AlarmTime)
            {
                _Unalarm();
            }
        }

        private void _Subscribe(EnemyController enemy)
        {
            enemy.OnAlarm += _OnAlarm;
            enemy.OnEnemyDeathEnd += _OnEnemyDeath;
            enemy.OnPlayerSpotted += _OnPlayerSpotted;
        }

        private void _Unsubscribe(EnemyController enemy)
        {
            enemy.OnAlarm -= _OnAlarm;
            enemy.OnEnemyDeathEnd -= _OnEnemyDeath;
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
            AlarmClock = 0f;

            foreach (var enemy in _enemies)
            {
                enemy.LastKnownPlayerPosition = vector;
                enemy.SpottedPlayer = true;
                enemy.SpottedPlayerThisFrame = true;
            }
        }

        private void _Unalarm()
        {
            Alarm = false;
            AlarmClock = 0f;

            foreach (var enemy in _enemies)
            {
                enemy.OnAlarm += _OnAlarm;
                enemy.Unalarm();
            }
        }
    }
}
