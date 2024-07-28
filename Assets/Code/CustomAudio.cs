using UnityEngine;

namespace Infringed
{
    [System.Serializable]
    public class CustomAudio
    {
        [field: SerializeField] public AudioClip WeaponSwing { get; private set; }
        [field: SerializeField] public AudioClip WeaponHit { get; private set; }
        [field: SerializeField] public AudioClip[] Steps { get; private set; }

        public AudioClip GetRandomStep()
        {
            return Steps[Random.Range(0, Steps.Length)];
        }
    }
}