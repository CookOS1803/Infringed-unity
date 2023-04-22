using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[System.Serializable]
public class PlayerSeeker
{
    [SerializeField, Min(0f)] private float waitTime = 2f;
    [SerializeField, Min(0f)] private float findingRadius = 6f;
    [Inject] private AIManager aiManager;
    private EnemyController enemy;
    private NavMeshAgent agent;

    public void Initialize(EnemyController enemy)
    {
        this.enemy = enemy;
        agent = enemy.GetComponent<NavMeshAgent>();     
    }

    public IEnumerator FindingPlayer()
    {
        float seekClock = 0f;

        while (true)
        {
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                while (seekClock < waitTime)
                {
                    seekClock += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                seekClock = 0f;

                GoToRandomPoint();

                yield return new WaitUntil (() => agent.velocity.sqrMagnitude < 0.01f);
            }
            else
                yield return new WaitForEndOfFrame();
        }
    }

    private void GoToRandomPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * findingRadius;
        randomDirection += aiManager.playerLastKnownPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, agent.height * 2, 1);
        Vector3 finalPosition = hit.position;

        agent.SetDestination(finalPosition);
    }
}
