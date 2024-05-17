using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    [RequireComponent(typeof(VisionController))]
    public class SuspicionController : MonoBehaviour
    {
        public event Action OnSuspect;

        [field: SerializeField, Min(0f)] public float NoticeTime { get; private set; } = 2f;
        [field: SerializeField, Min(0f)] public float SuspicionTime { get; private set; } = 1f;
        [SerializeField, Min(0f)] private float _unseeFactor = 0.5f;
        private VisionController _vision;
        private EnemyController _enemy;

        public float NoticeClock { get; private set; }
        public bool IsSuspecting { get; set; }
        public Vector3 SuspectPosition { get; private set; }

        private void Awake()
        {
            _vision = GetComponent<VisionController>();
            _enemy = GetComponent<EnemyController>();
        }

        private void Update()
        {
            if (_enemy.IsAlarmed)
            {
                NoticeClock = 0f;
                IsSuspecting = false;
                return;
            }

            if (_vision.IsPlayerInView)
            {
                _SuspectPlayerInView();
            }
            else if (!IsSuspecting)
            {
                NoticeClock = Mathf.MoveTowards(NoticeClock, 0, Time.deltaTime * _unseeFactor);
            }
        }

        public void Suspect(Vector3 source)
        {
            SuspectPosition = source;
            _StartSuspecting();

            if (NoticeClock < SuspicionTime)
                NoticeClock = SuspicionTime;
        }

        private void _SuspectPlayerInView()
        {
            var delta = Time.deltaTime * (_vision.DistanceOfView / Vector3.Distance(transform.position, _vision.LastNoticedPlayer.position));
            NoticeClock = Mathf.MoveTowards(NoticeClock, NoticeTime, delta);

            if (NoticeClock >= SuspicionTime)
            {
                _StartSuspecting();
                SuspectPosition = _vision.LastNoticedPlayer.position;
            }
        }

        private void _StartSuspecting()
        {
            if (!IsSuspecting)
                OnSuspect?.Invoke();

            IsSuspecting = true;
        }
    }
}
