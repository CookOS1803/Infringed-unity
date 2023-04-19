using UnityEngine;

public static class SoundUtil
{
    public static void SpawnSound(Vector3 soundSource, float radius)
    {        
        var listeners = Physics.OverlapSphere(soundSource, radius);

        foreach (var listener in listeners)
        {
            listener.GetComponent<ISoundListener>()?.ReactToSound(soundSource);
        }
    }
}
