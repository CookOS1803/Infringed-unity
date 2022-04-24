using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float damageRadius = 0.5f;
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
            var cols = Physics.OverlapSphere(transform.position, damageRadius);

            if (cols.Length != 0)
            {
                var health = cols[0].GetComponent<Health>();

                if (health != null)
                    health.TakeDamage(damage);

                StopDamaging();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isDamaging ? Color.green : Color.red;

        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
