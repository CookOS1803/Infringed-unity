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
        public event Action<EnemyController, Vector3> OnPlayerSpotted;

        public Vector3 LastKnownPlayerPosition { get; set; }
        public bool IsAlarmed { get; private set; }
        private VisionController _vision;
        private SoundResponder _soundResponder;
        private bool _spotBySound;

        private void Awake()
        {
            _vision = GetComponent<VisionController>();
            _soundResponder = GetComponent<SoundResponder>();
        }

        private void OnEnable()
        {
            _soundResponder.OnSound += _OnSound;
        }

        private void OnDisable()
        {
            _soundResponder.OnSound -= _OnSound;
        }

        private void Update()
        {
            if (!IsAlarmed)
                return;

            if (_vision.IsPlayerInView)
            {
                _spotBySound = false;
                OnPlayerSpotted?.Invoke(this, _vision.LastNoticedPlayer.position);
            }
            else
            {
                _spotBySound = true;
            }
        }

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

        private void _OnSound(Vector3 vector)
        {
            if (!IsAlarmed && _spotBySound)
                return;
            
            OnPlayerSpotted?.Invoke(this, vector);
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
