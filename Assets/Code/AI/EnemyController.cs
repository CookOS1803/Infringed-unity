using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class EnemyController : MonoBehaviour, IMortal
    {
        public event Action<EnemyController> OnAlarm;
        public event Action<EnemyController> OnUnalarm;
        public event Action<EnemyController> OnDeath;

        public bool IsAlarmed { get; private set; }

        private void OnTriggerEnter(Collider collider)
        {
            var door = collider.GetComponent<Map.Door>();

            door?.OpenTemporarily();
        }

        public void Alarm()
        {
            if (!IsAlarmed)
                _SetAlarmedState();
        }

        public void Unalarm()
        {
            if (IsAlarmed)
                _SetUnalarmedState();
        }

        public void OnDeathEnd()
        {
            
        }

        private void _SetAlarmedState()
        {
            IsAlarmed = true;
            OnAlarm?.Invoke(this);
        }

        private void _SetUnalarmedState()
        {
            IsAlarmed = false;
            OnUnalarm?.Invoke(this);
        }
    }
}
