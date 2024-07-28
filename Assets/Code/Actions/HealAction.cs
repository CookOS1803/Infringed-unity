using Infringed.Combat;
using UnityEngine;

namespace Infringed.Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/HealAction", fileName = "HealAction")]
    public class HealAction : ItemAction
    {
        [SerializeField] private int _amount = 20;
        [SerializeField] private AudioClip _audioClip;

        override public void Use(Context context)
        {
            context.actor.GetComponent<Health>().TakeHealing(_amount);
            AudioSource.PlayClipAtPoint(_audioClip, context.actor.position);
        }
    }
}
