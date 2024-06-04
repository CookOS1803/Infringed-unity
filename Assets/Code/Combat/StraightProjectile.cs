using UnityEngine;

namespace Infringed.Combat
{
    public class StraightProjectile : Projectile
    {
        private Vector3 _direction;

        public override void SetTarget(Vector3 target)
        {
            base.SetTarget(target);

            _direction = (Target - transform.position);
            _direction.y = 0f;
            _direction.Normalize();
        }

        protected override void _Move()
        {
            _rigidbody.MovePosition(transform.position + _direction * _speed * Time.fixedDeltaTime);
        }
    }
}
