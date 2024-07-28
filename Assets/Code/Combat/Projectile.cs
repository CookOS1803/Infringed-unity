using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected int _damage = 50;
        [SerializeField] protected float _speed = 4f;
        [SerializeField, Min(0f)] protected float _lifeTime = 5f;
        [field: SerializeField] public bool Stun { get; set; }
        [SerializeField, Min(0f)] protected float _stunTime = 2f;
        [SerializeField, Min(0f)] protected float _soundRadius = 6f;
        [SerializeField, Min(0f)] protected Vector3 _spinningVelocity;
        [SerializeField] protected AudioClip _hitClip;
        protected float _lifeClock = 0f;
        protected Vector3 _target;
        protected Rigidbody _rigidbody;

        public Vector3 Target => _target;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate()
        {
            _Move();

            var rotation = Quaternion.Euler(_spinningVelocity * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(transform.rotation * rotation);

            _lifeClock += Time.fixedDeltaTime;
            if (_lifeClock > _lifeTime)
                Destroy(gameObject);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_hitClip != null)
                AudioSource.PlayClipAtPoint(_hitClip, transform.position);

            if (other.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(_damage);
            }

            if (Stun && other.TryGetComponent<StunController>(out var stunner))
                stunner.Stun(_stunTime, transform.position);

            if (other.TryGetComponent<ParticleSystem>(out var particles))
                particles.Emit(6);

            SoundUtil.SpawnSound(transform.position, _soundRadius);

            Destroy(gameObject);
        }

        public virtual void SetTarget(Vector3 target)
        {
            _target = target;
        }

        protected abstract void _Move();
    }
}
