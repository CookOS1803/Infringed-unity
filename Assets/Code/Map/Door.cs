using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.Map
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] protected float _openTime = 2f;
        protected NavMeshObstacle _obstacle;
        private bool _isClosed = true;
        protected bool IsClosed
        {
            get => _isClosed;
            set
            {
                _isClosed = value;

                _obstacle.enabled = !value;
            }
        }
        protected readonly Quaternion _openingRotation = Quaternion.Euler(0f, 90f, 0f);
        protected readonly Quaternion _closingRotation = Quaternion.Euler(0f, -90f, 0f);
        private Vector3 _forward;

        private void Start()
        {
            _obstacle = GetComponent<NavMeshObstacle>();
            _forward = transform.forward;
        }

        private void Update()
        {
            transform.forward = Vector3.MoveTowards(transform.forward, _forward, Time.deltaTime * 25f);
        }

        public virtual void Interact(PlayerController user)
        {
            if (IsClosed)
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
            _forward = _openingRotation * transform.forward;
            IsClosed = false;
        }

        protected void _Close()
        {
            _forward = _closingRotation * transform.forward;
            IsClosed = true;
        }

        public void OpenTemporarily()
        {
            if (IsClosed)
            {
                _Open();
                StartCoroutine(_StayingOpen());
            }
        }

        public void OpenIndefinitely()
        {
            if (IsClosed)
                _Open();
        }

        private IEnumerator _StayingOpen()
        {
            float clock = 0f;

            while (clock < _openTime && !IsClosed)
            {
                clock += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            if (!IsClosed)
            {
                _Close();
            }
        }
    }
}
