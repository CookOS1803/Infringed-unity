using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class KnghtSounds : EntitySounds
    {
        [SerializeField] private AudioClip _suspicionClip;
        [SerializeField] private AudioClip _alarmClip;
        private SuspicionController _suspicion;
        private EnemyController _enemy;

        protected override void Awake()
        {
            base.Awake();

            _suspicion = GetComponent<SuspicionController>();
            _enemy = GetComponent<EnemyController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _suspicion.OnSuspect += _OnSuspect;
            _enemy.OnFirstAlarm += _OnAlarm;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _suspicion.OnSuspect -= _OnSuspect;
            _enemy.OnFirstAlarm -= _OnAlarm;
        }

        private void _OnSuspect()
        {
            _audio.Play(_suspicionClip);
        }

        private void _OnAlarm(EnemyController controller)
        {
            _audio.Play(_alarmClip);
        }
    }
}
