using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using Zenject;

namespace Infringed.AI
{
    public class VisionController : MonoBehaviour
    {
        public event Action OnPlayerInView;

        [field: SerializeField, Min(0f)] public float DistanceOfView { get; private set; } = 10f;
        [field: SerializeField, Range(0f, 360f)] public float FieldOfView { get; private set; } = 90f;
        [Inject(Id = CustomLayer.Player)] private LayerMask _playerLayer;
        private Collider[] _nonAllocColliders;

        public bool IsPlayerInView { get; private set; }
        public Transform LastNoticedPlayer { get; private set; }

        private void Awake()
        {
            _nonAllocColliders = new Collider[1];
        }

        private void FixedUpdate()
        {
            var noticedPlayer = _NoticePlayer();
            IsPlayerInView = noticedPlayer != null;

            if (IsPlayerInView)
            {
                LastNoticedPlayer = noticedPlayer;
                OnPlayerInView?.Invoke();
            }
        }

        private void OnDisable()
        {
            IsPlayerInView = false;
        }

        private Transform _NoticePlayer()
        {
            var length = Physics.OverlapSphereNonAlloc(transform.position, DistanceOfView, _nonAllocColliders, _playerLayer.value);

            // Player is too far
            if (length == 0)
                return null;

            var first = _nonAllocColliders[0];

            var angle = Vector3.Angle(transform.forward, first.transform.position - transform.position);

            // Player is not in field of view
            if (angle > FieldOfView * 0.5f)
                return null;

            Physics.Linecast(transform.position + Vector3.up, first.transform.position + Vector3.up, out var hit);

            // View is blocked
            if (hit.collider == null || hit.collider.GetComponent<PlayerController>() == null)
                return null;

            return hit.transform;
        }
    }
}
