using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float damageRadius = 0.5f;
    [SerializeField] private Vector3 secondPoint;
    [SerializeField] private LayerMask reactingLayer;
    [Zenject.Inject] private CustomAudio customAudio;
    private bool isDamaging = false;

    public void StartDamaging()
    {
        isDamaging = true;
    }

    public void StopDamaging()
    {
        isDamaging = false;
    }

    void Update()
    {
        if (isDamaging)
        {
            var cols = Physics.OverlapCapsule(transform.position, transform.TransformPoint(secondPoint), damageRadius, reactingLayer);

            if (cols.Length != 0)
            {
                OnHit(cols[0]);

                StopDamaging();
            }
        }
    }

    protected virtual void OnHit(Collider collider)
    {
        if (collider.TryGetComponent<Health>(out var health))
        {
            AudioSource.PlayClipAtPoint(customAudio.weaponHit, transform.position);
            health.TakeDamage(damage);
        }

        var particles = collider.GetComponent<ParticleSystem>();

        if (particles != null)
        {
            particles.Emit(6);
        }    
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = isDamaging ? Color.green : Color.red;

        Vector3 center = transform.TransformPoint(secondPoint);
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        Gizmos.DrawWireSphere(center, damageRadius);

        Vector3 vector31 = Vector3.Cross(transform.right, transform.position - center).normalized * damageRadius;
        Vector3 vector32 = Vector3.Cross(transform.forward, transform.position - center).normalized * damageRadius;
        Gizmos.DrawLine(transform.position + vector31, center + vector31);
        Gizmos.DrawLine(transform.position + -vector31, center + -vector31);
        Gizmos.DrawLine(transform.position + vector32, center + vector32);
        Gizmos.DrawLine(transform.position + -vector32, center + -vector32);
    }
}
