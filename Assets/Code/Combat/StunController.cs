using System;
using System.Collections;
using UnityEngine;

public class StunController : MonoBehaviour
{
    public bool isStunned { get; private set; } = false;
    private float stunClock = 0f;
    private float currentStunTime = 0f;

    public event Action onStunStart;
    public event Action onStunEnd;

    public void Stun(float stunTime)
    {
        if (!isStunned)
        {
            currentStunTime = stunTime;            
            isStunned = true;

            StartCoroutine(StunRoutine());
        }
        else if (stunTime >= currentStunTime - stunClock)
        {
            stunClock = 0f;
            currentStunTime = stunTime;
        }
    }

    private IEnumerator StunRoutine()
    {
        onStunStart?.Invoke();

        while (stunClock < currentStunTime)
        {
            stunClock += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        stunClock = 0f;
        isStunned = false;
        
        onStunEnd?.Invoke();
    }
}
