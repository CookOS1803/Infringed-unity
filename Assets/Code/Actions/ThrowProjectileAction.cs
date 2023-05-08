using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/ThrowProjectileAction", fileName = "ThrowProjectileAction")]
public class ThrowProjectileAction : ItemAction
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private AudioClip audioClip;
    [Zenject.Inject] private Zenject.DiContainer diContainer;

    public override void Use(Context context)
    {
        var projectile = diContainer.InstantiatePrefab(projectilePrefab, context.actor.position + Vector3.up + context.actor.forward * 0.5f, context.actor.rotation, null)
                        .GetComponent<Projectile>();
        
        projectile.target = context.target;

        AudioSource.PlayClipAtPoint(audioClip, context.actor.position);
    }
}
