using UnityEngine;

abstract public class ItemAction : ScriptableObject
{
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

    abstract public void Use(Context context);
}
