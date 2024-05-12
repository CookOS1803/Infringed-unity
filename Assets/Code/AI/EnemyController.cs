using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using UnityEngine;

namespace Infringed.AI
{
    public class EnemyController : MonoBehaviour, IAttacker, IDisposable
    {
        public event Action<EnemyController> OnAlarm;
        public event Action<EnemyController> OnUnalarm;
        public event Action<EnemyController, Vector3> OnPlayerSpotted;
        public event Action<EnemyController> OnAttackStart;
        public event Action<EnemyController> OnEnemyDeathEnd;

        [SerializeField] private Weapon _weapon;
        [SerializeField, Min(0f)] private float _attackRange = 1f;
        public Vector3 LastKnownPlayerPosition { get; set; }
        public bool SpottedPlayer { get; set; }
        public bool SpottedPlayerThisFrame { get; set; }
        public bool IsAlarmed { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsDying { get; private set; }
        [Zenject.Inject(Id = CustomLayer.Player)] private LayerMask _playerLayer;
        private VisionController _vision;
        private SoundResponder _soundResponder;
        private Health _health;
        private bool _spotBySound;

        private void Awake()
        {
            _vision = GetComponent<VisionController>();
            _soundResponder = GetComponent<SoundResponder>();

            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _soundResponder.OnSound += _OnSound;
            _health.OnDeathStart += Die;
            _health.OnDeathEnd += _DeathEnd;
        }

        private void OnDisable()
        {
            _soundResponder.OnSound -= _OnSound;
            _health.OnDeathStart -= Die;
            _health.OnDeathEnd -= _DeathEnd;
        }

        private void Update()
        {
            if (!IsAlarmed || IsDying)
            {
                SpottedPlayer = false;
                return;
            }

            if (_vision.IsPlayerInView)
            {
                _spotBySound = false;
                OnPlayerSpotted?.Invoke(this, _vision.LastNoticedPlayer.position);
            }
            else
            {
                if (SpottedPlayerThisFrame)
                    SpottedPlayerThisFrame = false;
                else
                    SpottedPlayer = false;

                _spotBySound = true;
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            var door = collider.GetComponent<Map.Door>();

            door?.OpenTemporarily();
        }

#if UNITY_EDITOR
        [Header("Debug info")]
        [SerializeField] private bool _showAttackRange;
        [SerializeField] private Color _attackRangeColor;

        private void OnDrawGizmos()
        {
            if (_showAttackRange)
            {
                Gizmos.color = _attackRangeColor;
                Gizmos.DrawWireSphere(transform.position, _attackRange);
            }
        }
#endif

        public void Alarm()
        {
            if (IsAlarmed)
                return;

            IsAlarmed = true;
            OnAlarm?.Invoke(this);
        }

        public void Alarm(Vector3 playerPosition)
        {
            Alarm();

            OnPlayerSpotted?.Invoke(this, playerPosition);
        }

        public void Unalarm()
        {
            if (!IsAlarmed)
                return;

            IsAlarmed = false;
            OnUnalarm?.Invoke(this);
        }

        public void AttackStateStarted()
        {
            // nothing
        }

        public void AttackStateEnded()
        {
            IsAttacking = false;
        }

        public void Attack()
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                OnAttackStart?.Invoke(this);
            }
        }

        public bool IsPlayerInAttackRange()
        {
            return Physics.OverlapSphere(transform.position, _attackRange, _playerLayer.value).Length != 0;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void Die()
        {
            if (IsDying)
                return;
            
            IsDying = true;
        }

        private void _OnSound(Vector3 vector)
        {
            if (!IsAlarmed && _spotBySound)
                return;

            OnPlayerSpotted?.Invoke(this, vector);
        }

        private void _DeathEnd()
        {
            OnEnemyDeathEnd?.Invoke(this);
        }

        public void OnAttackStartEvent()
        {
            _weapon.StartDamaging();
        }

        public void OnAttackEndEvent()
        {
            _weapon.StopDamaging();
        }
    }
}
