using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable, System.Obsolete]
public class SoundResponder
{
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float hearingTime = 2f;
    private float waitClock = 0f;
    private float hearClock = 0f;
    private EnemyController enemy;
    private NavMeshAgent agent;
    public Vector3 soundSource { get; set; }

    public void Initialize(EnemyController enemy)
    {
        this.enemy = enemy;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public IEnumerator HearingSound(System.Action callback)
    {
        hearClock = 0f;

        enemy.CanMove = false;

        while (hearClock < hearingTime)
        {
            hearClock += Time.deltaTime;

            var desiredRotation = Quaternion.FromToRotation(Vector3.forward, (soundSource - enemy.transform.position).normalized);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime); 

            yield return new WaitForEndOfFrame();
        }

        enemy.CanMove = true;

        callback?.Invoke();
    }

    public IEnumerator RespondingToSound()
    {
        waitClock = 0f;

        agent.SetDestination(soundSource);

        yield return new WaitWhile(() => agent.velocity.sqrMagnitude < 0.01f);
        yield return new WaitUntil(
            () => Vector3.Distance(agent.destination, enemy.transform.position) <= agent.stoppingDistance
        );

        while (waitClock < waitTime)
        {
            waitClock += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        enemy.ResumeBehavior();
    }
}
