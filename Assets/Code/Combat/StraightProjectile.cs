using UnityEngine;

public class StraightProjectile : Projectile
{
    protected override void Move()
    {
        transform.position += (target - transform.position).normalized * speed * Time.deltaTime;
    }
}