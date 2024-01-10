using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.AI
{
    public class MovementController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        
        public bool IsMoving => Vector3.Distance(_agent.destination, _agent.transform.position) > _agent.stoppingDistance;
        public float AngularSpeed => _agent.angularSpeed;

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