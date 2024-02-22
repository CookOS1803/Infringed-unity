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
        [SerializeField, Min(0f)] protected float _stunTime = 2f;
        [SerializeField, Min(0f)] protected float _soundRadius = 6f;
        [SerializeField, Min(0f)] protected Vector3 _spinningVelocity;
        [Zenject.Inject] protected CustomAudio _customAudio;
        [SerializeField] protected AudioClip _hitClip;
        protected float _lifeClock = 0f;

        public Vector3 Target { get; set; }

        private void Update()
        {
            _Move();

            transform.Rotate(_spinningVelocity * Time.deltaTime);

            _lifeClock += Time.deltaTime;
            if (_lifeClock > _lifeTime)
                Destroy(gameObject);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_hitClip != null)
                AudioSource.PlayClipAtPoint(_hitClip, transform.position);

            if (other.TryGetComponent<Health>(out var health))
            {
                AudioSource.PlayClipAtPoint(_customAudio.WeaponHit, transform.position);
                health.TakeDamage(_damage);
            }

            if (other.TryGetComponent<StunController>(out var stunner))
                stunner.Stun(_stunTime);

            if (other.TryGetComponent<ParticleSystem>(out var particles))
                particles.Emit(6);

            SoundUtil.SpawnSound(transform.position, _soundRadius);

            Destroy(gameObject);
        }

        protected abstract void _Move();
    }
}
