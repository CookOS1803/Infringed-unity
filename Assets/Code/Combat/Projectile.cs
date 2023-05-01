using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 50;
    [SerializeField] private float speed = 4f;
    [SerializeField, Min(0f)] private float lifeTime = 5f;
    [SerializeField, Min(0f)] private float stunTime = 2f;
    [SerializeField, Min(0f)] private float soundRadius = 6f;
    private float lifeClock = 0f;

    void Update()
    {
        transform.Translate(0f, 0f, speed * Time.deltaTime);

        lifeClock += Time.deltaTime;
        if (lifeClock > lifeTime)
            Destroy(gameObject);
    }

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
