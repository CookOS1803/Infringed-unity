using UnityEngine;

public static class SoundUtil
{
    public static void SpawnSound(Vector3 soundSource, float radius)
    {        
        var listeners = Physics.OverlapSphere(soundSource, radius);

        foreach (var collider in listeners)
        {
            var listener = collider.GetComponent<ISoundListener>();
            if (listener == null)
                continue;
            
            if (Physics.Linecast(soundSource, collider.transform.position, out var hit) && hit.collider != collider)           
            {
                if (Vector3.Distance(soundSource, collider.transform.position) < radius * 0.7f)
                    listener.RespondToSound(soundSource);
            }
            else
                listener.RespondToSound(soundSource);
        }
    }
}
