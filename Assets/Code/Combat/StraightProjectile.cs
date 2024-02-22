using UnityEngine;

namespace Infringed.Combat
{
    public class StraightProjectile : Projectile
    {
        protected override void _Move()
        {
            transform.position += (Target - transform.position).normalized * _speed * Time.deltaTime;
        }
    }
}
