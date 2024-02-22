using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    [RequireComponent(typeof(VisionController))]
    public class SuspicionController : MonoBehaviour
    {
        [field: SerializeField, Min(0f)] public float NoticeTime { get; private set; } = 2f;
        [field: SerializeField, Min(0f)] public float SuspicionTime { get; private set; } = 1f;
        [SerializeField, Min(0f)] private float _unseeFactor = 0.5f;
        private VisionController _vision;
        private SoundResponder _soundResponder;

        public float NoticeClock { get; private set; }
        public bool IsSuspecting { get; set; }
        public Vector3 SuspectPosition { get; private set; }

        private void Awake()
        {
            _vision = GetComponent<VisionController>();
            _soundResponder = GetComponent<SoundResponder>();
        }

        private void OnEnable()
        {
            //_soundResponder.OnSound += _OnSound;
        }

        private void OnDisable()
        {
            //_soundResponder.OnSound -= _OnSound;
        }

        private void Update()
        {
            if (_vision.IsPlayerInView)
            {
                var delta = Time.deltaTime * (_vision.DistanceOfView / Vector3.Distance(transform.position, _vision.LastNoticedPlayer.transform.position));
                NoticeClock = Mathf.MoveTowards(NoticeClock, NoticeTime, delta);

                if (NoticeClock >= SuspicionTime)
                {
                    IsSuspecting = true;
                    SuspectPosition = _vision.LastNoticedPlayer.position;
                }
            }
            else if (!IsSuspecting)
            {
                NoticeClock = Mathf.MoveTowards(NoticeClock, 0, Time.deltaTime * _unseeFactor);
            }
        }

        public void Suspect(Vector3 source)
        {
            SuspectPosition = source;
            IsSuspecting = true;
            
            if (NoticeClock < SuspicionTime)
                NoticeClock = SuspicionTime;
        }

        private void _OnSound(Vector3 source)
        {
            // drugoe uslovie nado
            if (!IsSuspecting)
            {
                SuspectPosition = source;

                if (NoticeClock < SuspicionTime)
                    NoticeClock = SuspicionTime;
            }

            IsSuspecting = true;
        }
    }
}
