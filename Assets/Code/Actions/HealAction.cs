using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/HealAction", fileName = "HealAction")]
public class HealAction : ItemAction
{
    [SerializeField] private int amount = 20;
    [SerializeField] private AudioClip audioClip;

    override public void Use(Context context)
    {
        context.actor.GetComponent<Health>().TakeHealing(amount);
        AudioSource.PlayClipAtPoint(audioClip, context.actor.position);
    }
}
