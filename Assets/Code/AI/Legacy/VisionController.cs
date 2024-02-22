using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Infringed.Legacy
{
    [Obsolete]
    public class VisionController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _distanceOfView = 10f;
        [SerializeField, Range(0f, 360f)] private float _fieldOfView = 90f;
        [SerializeField, Min(0f)] private float noticeTime = 2f;
        [SerializeField, Min(0f)] private float forgetTime = 1f;
        [SerializeField, Min(0f)] private float unseeFactor = 0.5f;
        [Inject(Id = CustomLayer.Player)] private LayerMask playerLayer;
        [Inject] private AIManager aiManager;
        [Inject] private Player.PlayerController playerRef;
        private EnemyController enemyController;
        private float _noticeClock = 0f;
        private bool _isSeeingPlayer = false;
        public float noticeClock
        {
            get => _noticeClock;
            set
            {
                _noticeClock = value;

                onNoticeClockChange?.Invoke();
            }
        }

        public Transform player { get; set; }
        public bool hasSeenPlayerHiding { get; set; } = false;
        public float forgetClock { get; set; } = 0f;
        public float distanceOfView => _distanceOfView;
        public float fieldOfView => _fieldOfView;
        public bool isSeeingPlayer => _isSeeingPlayer;
        public float normalizedNoticeClock => noticeClock / noticeTime;

        public event Action onNoticeClockChange;
        public event Action onNoticeClockReset;

        private void Awake()
        {
            enemyController = GetComponent<EnemyController>();
        }

        private void Update()
        {
            _isSeeingPlayer = NoticePlayer();

            if (aiManager.alarm && player != null)
            {
                if (forgetClock < forgetTime)
                    forgetClock += Time.deltaTime;
                else
                {
                    if (player != null)
                    {
                        playerRef.OnHide -= OnPlayerHide;
                        playerRef.OnExitHideout -= OnPlayerExitHideout;
                    }
                    player = null;
                    _isSeeingPlayer = false;
                }
            }
        }

        public void ResetNoticeClock()
        {
            noticeClock = 0f;

            onNoticeClockReset?.Invoke();
        }

        private bool NoticePlayer()
        {
            var cols = Physics.OverlapSphere(transform.position, distanceOfView, playerLayer.value);

            // Player is too far
            if (cols.Length == 0)
                return false;

            float angle = Vector3.Angle(transform.forward, cols[0].transform.position - transform.position);

            // Player is not in field of view
            if (angle > fieldOfView * 0.5f)
                return false;

            Physics.Linecast(transform.position + Vector3.up, cols[0].transform.position + Vector3.up, out var hit);

            return hit.collider != null && hit.collider.GetComponent<Player.PlayerController>() != null;
        }

        public void StartWatching()
        {
            StopAllCoroutines();
            StartCoroutine(Watching());
        }

        public void StopWatching()
        {
            StopAllCoroutines();
        }

        private IEnumerator Watching()
        {
            while (true)
            {
                yield return new WaitUntil(() => isSeeingPlayer);

                enemyController.StopBehavior();

                yield return LookingAtPlayer();
                yield return UnseeingPlayer();

                enemyController.ResumeBehavior();
            }
        }

        private IEnumerator LookingAtPlayer()
        {
            while (isSeeingPlayer)
            {
                if (!aiManager.alarm && noticeClock < noticeTime)
                {
                    enemyController.CanMove = false;
                    enemyController.enemyState = EnemyState.LookingAtPlayer;
                    transform.LookAt(playerRef.transform);

                    noticeClock += Time.deltaTime * (distanceOfView / Vector3.Distance(transform.position, playerRef.transform.position));
                }
                else
                {
                    if (player == null)
                    {
                        playerRef.OnHide += OnPlayerHide;
                        playerRef.OnExitHideout += OnPlayerExitHideout;
                    }
                    player = playerRef.transform;
                    forgetClock = 0f;
                    aiManager.SoundTheAlarm();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator UnseeingPlayer()
        {
            if (!aiManager.alarm)
            {
                while (noticeClock > 0f && !isSeeingPlayer)
                {
                    noticeClock -= Time.deltaTime * unseeFactor;

                    yield return new WaitForEndOfFrame();
                }

                if (!isSeeingPlayer)
                {
                    ResetNoticeClock();
                    enemyController.CanMove = true;
                }
            }
        }

        private void OnPlayerHide()
        {
            hasSeenPlayerHiding = true;
        }

        private void OnPlayerExitHideout()
        {
            hasSeenPlayerHiding = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, distanceOfView);

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, fieldOfView / 2f, 0f) * (transform.forward) * distanceOfView);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, -fieldOfView / 2f, 0f) * (transform.forward) * distanceOfView);
        }
    }
}
