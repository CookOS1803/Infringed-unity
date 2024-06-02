using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.Map
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField, Min(0)] protected float _openTime = 2f;
        [SerializeField, Min(0)] private float _openingSpeed = 25f;
        protected NavMeshObstacle _obstacle;
        protected bool _isClosed = true;
        protected readonly Quaternion _openingRotation = Quaternion.Euler(0f, 90f, 0f);
        protected readonly Quaternion _closingRotation = Quaternion.Euler(0f, -90f, 0f);
        private Vector3 _forward;
        private Collider[] _colliders;
        private bool _changingState;

        private void Awake()
        {
            _obstacle = GetComponent<NavMeshObstacle>();
            _colliders = GetComponents<Collider>();
            _forward = transform.forward;
        }

        private void Update()
        {
            var newForward = Vector3.MoveTowards(transform.forward, _forward, Time.deltaTime * _openingSpeed);

            _changingState = transform.forward != newForward;

            _obstacle.enabled = !_isClosed && !_changingState;

            foreach (var c in _colliders)
            {
                c.enabled = !_changingState;
            }

            transform.forward = newForward;
        }

        public virtual void Interact(PlayerController user)
        {
            if (_isClosed)
            {
                _Open();
            }
            else
            {
                _Close();
            }
        }

        protected void _Open()
        {
            _forward = _openingRotation * _forward;
            _isClosed = false;
            _changingState = true;
        }

        protected void _Close()
        {
            _forward = _closingRotation * _forward;
            _isClosed = true;
            _changingState = true;
        }

        public void OpenTemporarily()
        {
            if (_isClosed)
            {
                _Open();
                StartCoroutine(_StayingOpen());
            }
        }

        public void OpenIndefinitely()
        {
            if (_isClosed)
                _Open();
        }

        private IEnumerator _StayingOpen()
        {
            float clock = 0f;

            while (clock < _openTime && !_isClosed)
            {
                clock += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            if (!_isClosed)
            {
                _Close();
            }
        }
    }
}
