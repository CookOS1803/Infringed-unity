using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Patroler
{
    [SerializeField, Min(0f)] private float waitTime = 2f;
    [SerializeField, Min(0f)] private float unseeFactor = 0.5f;
    [SerializeField] private Transform[] patrolPoints;
    private int currentPoint;
    private EnemyController enemy;
    private NavMeshAgent agent;

    public void Initialize(EnemyController enemy)
    {
        this.enemy = enemy;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public IEnumerator Patroling()
    {
        float waitClock = 0f;

        while (true)
        {
            if (enemy.isSeeingPlayer)
            {
                yield return LookingAtPlayer();

                continue;
            }

            yield return GoingToPatrolPoint();

            if (!enemy.isSeeingPlayer)
                currentPoint = (currentPoint + 1) % patrolPoints.Length;

            if (patrolPoints.Length == 1)
                yield return new WaitForEndOfFrame();
            else
            {
                while (waitClock < waitTime && !enemy.isSeeingPlayer)
                {
                    waitClock += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                waitClock = 0f;
            }
        }
    }

    private IEnumerator LookingAtPlayer()
    {
        enemy.canMove = false;

        yield return new WaitUntil(() => !enemy.isSeeingPlayer);

        while (enemy.noticeClock > 0f)
        {
            enemy.noticeClock -= Time.deltaTime * unseeFactor;

            yield return new WaitForEndOfFrame();
        }

        enemy.ResetNoticeClock();

        enemy.canMove = true;
    }

    private IEnumerator GoingToPatrolPoint()
    {
        agent.SetDestination(patrolPoints[currentPoint].position);

        yield return new WaitWhile(() => agent.velocity.sqrMagnitude < 0.01f);
        yield return new WaitUntil(
            () => Vector3.Distance(agent.destination, enemy.transform.position) <= agent.stoppingDistance || enemy.isSeeingPlayer
        );

        yield return RotatingTowards(patrolPoints[currentPoint].rotation);
    }

    private IEnumerator RotatingTowards(Quaternion desiredRotation)
    {
        while (enemy.transform.rotation != desiredRotation && !enemy.isSeeingPlayer)
        {
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }
}
