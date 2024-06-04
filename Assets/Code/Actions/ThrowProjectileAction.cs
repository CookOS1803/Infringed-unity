using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using Infringed.InventorySystem;
using ModestTree;
using UnityEngine;

namespace Infringed.Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/ThrowProjectileAction", fileName = "ThrowProjectileAction")]
    public class ThrowProjectileAction : ItemAction
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private Ability StunAbility;
        [Zenject.Inject] private Zenject.DiContainer _diContainer;

        public override void Use(Context context)
        {
            var projectile = _diContainer.InstantiatePrefab(_projectilePrefab, context.actor.position + Vector3.up + context.actor.forward * 0.5f, context.actor.rotation, null)
                            .GetComponent<Projectile>();

            projectile.SetTarget(context.target);

            var abilities = context.actor.GetComponent<AbilitySet>();
            var a = abilities.GetAbilityInstance(StunAbility);
            if (a != null && a.IsLearned)
                projectile.Stun = true;

            if (_audioClip != null)
                AudioSource.PlayClipAtPoint(_audioClip, context.actor.position);
        }
    }
}
