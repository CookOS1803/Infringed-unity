using UnityEngine;
using Zenject;
using Infringed.AI;
using Infringed.Combat;

namespace Infringed.Player
{
    public class PlayerWeapon : Weapon
    {
        [SerializeField, Range(0f, 180f)] private float _backstabAngle = 90f;
        [SerializeField] private Transform _player;

        protected override void _OnHit(Collider collider)
        {
            var enemy = collider.GetComponent<EnemyController>();

            if (enemy != null)
            {
                var isStunned = enemy.GetComponent<StunController>().IsStunned;
                var to = (enemy.transform.position - _player.position).normalized;
                var angle = Vector3.Angle(enemy.transform.forward, to);

                if (isStunned || !enemy.SpottedPlayer && angle < _backstabAngle / 2f)
                {
                    enemy.Die();
                }
                else
                {
                    enemy.Alarm(_player.position);
                }
            }

            base._OnHit(collider);
        }
    }
}
