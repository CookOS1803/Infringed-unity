using UnityEngine;
using Zenject;
using Infringed.Player;
using Infringed.AI;

namespace Infringed.Combat
{
    public class PlayerWeapon : Weapon
    {
        //[SerializeField] private float _backstabDot = 0.5f;
        private Transform _player;

        [Inject]
        void SetPlayer(PlayerController controller)
        {
            _player = controller.transform;
        }

        protected override void _OnHit(Collider collider)
        {
            /* Legacy code
            var enemy = collider.GetComponent<Legacy.EnemyController>();

            if (enemy != null)
            {
                if (enemy.stunController.isStunned || _aiManager.player == null &&
                    Vector3.Dot(enemy.transform.forward, enemy.transform.position - player.position) >= _backstabDot)
                {
                    enemy.Die();
                }
                else
                {
                    enemy.Alert();
                }

            }
            */

            base._OnHit(collider);
        }
    }
}
