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
        var health = other.GetComponent<Health>();
        health?.TakeDamage(damage);

        var stunner = other.GetComponent<StunController>();
        stunner?.Stun(stunTime);

        var particles = other.GetComponent<ParticleSystem>();
        if (particles != null)
        {
            particles.Emit(6);
        }

        SoundUtil.SpawnSound(transform.position, soundRadius);

        Destroy(gameObject);
    }


}
