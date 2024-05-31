using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private int _damage = 30;
        [SerializeField] private float _damageRadius = 0.5f;
        [SerializeField] private Vector3 _secondPoint;
        [SerializeField] private LayerMask _reactingLayer;
        private bool _isDamaging = false;
        private Collider[] _nonAllocColliders;

        private void Awake()
        {
            _nonAllocColliders = new Collider[1];
        }

        private void FixedUpdate()
        {
            if (_isDamaging)
            {
                var length = Physics.OverlapCapsuleNonAlloc(transform.position, transform.TransformPoint(_secondPoint), _damageRadius, _nonAllocColliders, _reactingLayer);

                if (length != 0)
                {
                    _OnHit(_nonAllocColliders[0]);

                    StopDamaging();
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _isDamaging ? Color.green : Color.red;

            Vector3 center = transform.TransformPoint(_secondPoint);
            Gizmos.DrawWireSphere(transform.position, _damageRadius);
            Gizmos.DrawWireSphere(center, _damageRadius);

            Vector3 vector31 = Vector3.Cross(transform.right, transform.position - center).normalized * _damageRadius;
            Vector3 vector32 = Vector3.Cross(transform.forward, transform.position - center).normalized * _damageRadius;
            Gizmos.DrawLine(transform.position + vector31, center + vector31);
            Gizmos.DrawLine(transform.position + -vector31, center + -vector31);
            Gizmos.DrawLine(transform.position + vector32, center + vector32);
            Gizmos.DrawLine(transform.position + -vector32, center + -vector32);
        }
        
        public void StartDamaging()
        {
            _isDamaging = true;
        }

        public void StopDamaging()
        {
            _isDamaging = false;
        }

        protected virtual void _OnHit(Collider collider)
        {
            if (collider.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(_damage);
            }

            var particles = collider.GetComponent<ParticleSystem>();

            if (particles != null)
            {
                particles.Emit(6);
            }
        }
    }
}
