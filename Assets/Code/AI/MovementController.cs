using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.AI
{
    public class MovementController : MonoBehaviour, IMoveable
    {
        private NavMeshAgent _agent;
        
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
        }

        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
            
        }
    }
}