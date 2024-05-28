using Infringed.Combat;
using UnityEngine;

namespace Infringed.Actions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Actions/BlinkAction", fileName = "BlinkAction")]
    public class BlinkAction : ItemAction
    {
        [field: SerializeField, Min(0f)] public float MaxBlinkDistance { get; private set; } = 4f;
        [SerializeField] private AudioClip _audioClip;

        override public void Use(Context context)
        {
            var actor = context.actor;

            var newPosition = GetBlinkPosition(actor, context.target, MaxBlinkDistance);

            actor.GetComponent<CharacterController>().Move(newPosition - actor.position);
        }

        public static Vector3 GetBlinkPosition(Transform actor, Vector3 target, float blinkDistane)
        {
            var vector = target - actor.position;
            var direction = vector.normalized;

            var magnitude = vector.magnitude;

            var distance = magnitude > blinkDistane ? blinkDistane : magnitude;

            if (Physics.Raycast(actor.position + Vector3.up, direction, out var hit, distance, ~LayerMask.GetMask("Floor")))
            {
                return hit.point - Vector3.up;
            }
            else
            {
                return actor.position + direction * distance;
            }
        }
    }
}
