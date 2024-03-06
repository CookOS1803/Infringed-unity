using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using ModestTree;
using UnityEngine;

namespace Infringed.Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/ThrowProjectileAction", fileName = "ThrowProjectileAction")]
    public class ThrowProjectileAction : ItemAction
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private AudioClip _audioClip;
        [Zenject.Inject] private Zenject.DiContainer _diContainer;

        public override void Use(Context context)
        {
            var projectile = _diContainer.InstantiatePrefab(_projectilePrefab, context.actor.position + Vector3.up + context.actor.forward * 0.5f, context.actor.rotation, null)
                            .GetComponent<Projectile>();

            projectile.SetTarget(context.target);

            AudioSource.PlayClipAtPoint(_audioClip, context.actor.position);
        }
    }
}
