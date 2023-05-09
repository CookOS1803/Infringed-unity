using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage = 50;
    [SerializeField] protected float speed = 4f;
    [SerializeField, Min(0f)] protected float lifeTime = 5f;
    [SerializeField, Min(0f)] protected float stunTime = 2f;
    [SerializeField, Min(0f)] protected float soundRadius = 6f;
    [SerializeField, Min(0f)] protected Vector3 spinningVelocity;
    [Zenject.Inject] protected CustomAudio customAudio;
    [SerializeField] protected AudioClip hitClip;
    protected float lifeClock = 0f;

    public Vector3 target { get; set; }

    void Update()
    {
        Move();

        transform.Rotate(spinningVelocity * Time.deltaTime);

        lifeClock += Time.deltaTime;
        if (lifeClock > lifeTime)
            Destroy(gameObject);
    }

    protected abstract void Move();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hitClip != null)
            AudioSource.PlayClipAtPoint(hitClip, transform.position);

        if (other.TryGetComponent<Health>(out var health))
        {
            AudioSource.PlayClipAtPoint(customAudio.weaponHit, transform.position);
            health.TakeDamage(damage);
        }

        if (other.TryGetComponent<StunController>(out var stunner))
            stunner.Stun(stunTime);

        if (other.TryGetComponent<ParticleSystem>(out var particles))
            particles.Emit(6);

        SoundUtil.SpawnSound(transform.position, soundRadius);

        Destroy(gameObject);
    }


}
