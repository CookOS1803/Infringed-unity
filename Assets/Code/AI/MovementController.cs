using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.AI
{
    public class MovementController : MonoBehaviour, IMoveable
    {
        [SerializeField, Min(0f)] private float _calmSpeed = 1.5f;
        [SerializeField, Min(0f)] private float _alarmedSpeed = 3.5f;
        private NavMeshAgent _agent;
        private EnemyController _enemy;

        public bool IsMoving => CanMove && Vector3.Distance(_agent.destination, _agent.transform.position) > _agent.stoppingDistance;
        public float AngularSpeed => _agent.angularSpeed;
        public bool CanMove
        {
            get => !_agent.isStopped;
            set
            {
                _agent.isStopped = !value;

                if (!value)
                    SetDestination(_agent.transform.position);
            }
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _enemy = GetComponent<EnemyController>();

            _agent.speed = _calmSpeed;
        }

        private void OnEnable()
        {
            _enemy.OnAlarm += _OnAlarm;
            _enemy.OnUnalarm += _OnUnalarm;
        }

        private void OnDisable()
        {
            _enemy.OnAlarm -= _OnAlarm;
            _enemy.OnUnalarm -= _OnUnalarm;
        }

        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }

        private void _OnAlarm(EnemyController sender)
        {
            _agent.speed = _alarmedSpeed;
        }

        private void _OnUnalarm(EnemyController sender)
        {
            _agent.speed = _calmSpeed;
        }
    }
}