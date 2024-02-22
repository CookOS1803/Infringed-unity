using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using Zenject;

namespace Infringed.AI
{
    public class VisionController : MonoBehaviour
    {
        [field: SerializeField, Min(0f)] public float DistanceOfView { get; private set; } = 10f;
        [field: SerializeField, Range(0f, 360f)] public float FieldOfView { get; private set; } = 90f;
        [Inject(Id = CustomLayer.Player)] private LayerMask _playerLayer;
        
        public bool IsPlayerInView { get; private set; }
        public Transform LastNoticedPlayer { get; private set; }

        private void Update()
        {
            var noticedPlayer = _NoticePlayer();
            IsPlayerInView = noticedPlayer != null;

            if (IsPlayerInView)
            {
                LastNoticedPlayer = noticedPlayer;              
            }
        }

        private Transform _NoticePlayer()
        {
            var colliders = Physics.OverlapSphere(transform.position, DistanceOfView, _playerLayer.value);

            // Player is too far
            if (colliders.Length == 0)
                return null;

            var angle = Vector3.Angle(transform.forward, colliders[0].transform.position - transform.position);

            // Player is not in field of view
            if (angle > FieldOfView * 0.5f)
                return null;

            Physics.Linecast(transform.position + Vector3.up, colliders[0].transform.position + Vector3.up, out var hit);

            // View is blocked
            if (hit.collider == null || hit.collider.GetComponent<PlayerController>() == null)
                return null;

            return hit.transform;
        }
    }
}
