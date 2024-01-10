using System;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.AI
{
    public class Patroler : MonoBehaviour
    {
        [SerializeField] private Transform[] _patrolPoints;
        private int _currentPoint = 0;
        private MovementController _movement;

        public bool IsOnTheWay => _movement.IsMoving;
        public float AngularSpeed => _movement.AngularSpeed;
        public Transform CurrentPatrolPoint => _patrolPoints[_currentPoint];

        private void Awake()
        {
            _movement = GetComponent<MovementController>();
        }
        
        public void GoToNextPoint()
        {
            _movement.SetDestination(CurrentPatrolPoint.position);
        }

        public void ChangePoint()
        {
            _currentPoint = (_currentPoint + 1) % _patrolPoints.Length;
        }
    }
}