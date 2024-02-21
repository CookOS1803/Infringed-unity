using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class SoundResponder : MonoBehaviour, ISoundListener
    {
        public Vector3 LastHeardSound { get; set; }

        public System.Action<Vector3> OnSound;

        public void RespondToSound(Vector3 source)
        {
            LastHeardSound = source;
            OnSound?.Invoke(source);
        }
    }
}
