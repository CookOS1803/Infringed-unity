using UnityEngine;

namespace Infringed.Actions
{
    abstract public class ItemAction : ScriptableObject
    {
        [field: SerializeField] public ActionCastMarker CastMarkerPrefab { get; private set; }

        abstract public void Use(Context context);

        public struct Context
        {
            public Transform actor;
            public Vector3 target;

            public Context(Transform actor, Vector3 target)
            {
                this.actor = actor;
                this.target = target;
            }
        }
    }
}
