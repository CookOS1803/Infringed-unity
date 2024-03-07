using UnityEngine;
using Zenject;
using Infringed.Player;
using Infringed.AI;

namespace Infringed.Combat
{
    public class PlayerWeapon : Weapon
    {
        [SerializeField] private float _backstabDot = 0.5f;
        [SerializeField] private Transform _player;

        protected override void _OnHit(Collider collider)
        {
            var enemy = collider.GetComponent<EnemyController>();

            if (enemy != null)
            {
                var isStunned = enemy.GetComponent<StunController>().IsStunned;
                var dot = Vector3.Dot(enemy.transform.forward, enemy.transform.position - _player.position);

                if (isStunned || !enemy.SpottedPlayer && dot >= _backstabDot)
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
