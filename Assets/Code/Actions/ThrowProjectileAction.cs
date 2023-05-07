using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/ThrowProjectileAction", fileName = "ThrowProjectileAction")]
public class ThrowProjectileAction : ItemAction
{
    [SerializeField] private GameObject projectilePrefab;

    public override void Use(Context context)
    {
        var projectile = Instantiate(projectilePrefab, context.actor.position + Vector3.up + context.actor.forward * 0.5f, context.actor.rotation)
                        .GetComponent<Projectile>();
        
        projectile.target = context.target;
    }
}
