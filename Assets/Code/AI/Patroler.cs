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
    private float waitClock = 0f;

    public Vector3 soundSource { get; set; }
    public bool heardSound { get; set; } = false;

    public void Initialize(EnemyController enemy)
    {
        this.enemy = enemy;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public IEnumerator Patroling()
    {
        waitClock = 0f;

        while (true)
        {
            if (heardSound)
            {
                yield return ReactingToSound();

                heardSound = false;
            }

            if (enemy.visionController.isSeeingPlayer)
            {
                yield return LookingAtPlayer();

                continue;
            }

            yield return GoingToPatrolPoint();

            if (!enemy.visionController.isSeeingPlayer)
                currentPoint = (currentPoint + 1) % patrolPoints.Length;

            if (patrolPoints.Length == 1)
                yield return new WaitForEndOfFrame();
            else
            {
                while (waitClock < waitTime && !enemy.visionController.isSeeingPlayer && !heardSound)
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

        yield return new WaitUntil(() => !enemy.visionController.isSeeingPlayer);

        while (enemy.visionController.noticeClock > 0f)
        {
            enemy.visionController.noticeClock -= Time.deltaTime * unseeFactor;

            yield return new WaitForEndOfFrame();
        }

        enemy.visionController.ResetNoticeClock();

        enemy.canMove = true;
    }

    private IEnumerator GoingToPatrolPoint()
    {
        yield return GoingToPoint(patrolPoints[currentPoint].position);

        yield return RotatingTowards(patrolPoints[currentPoint].rotation);
    }

    private IEnumerator GoingToPoint(Vector3 destination)
    {
        agent.SetDestination(destination);

        yield return new WaitWhile(() => agent.velocity.sqrMagnitude < 0.01f);
        yield return new WaitUntil(
            () => Vector3.Distance(agent.destination, enemy.transform.position) <= agent.stoppingDistance || enemy.visionController.isSeeingPlayer || heardSound
        );
    }

    private IEnumerator RotatingTowards(Quaternion desiredRotation)
    {
        while (enemy.transform.rotation != desiredRotation && !enemy.visionController.isSeeingPlayer && !heardSound)
        {
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ReactingToSound()
    {
        agent.SetDestination(soundSource);

        yield return new WaitWhile(() => agent.velocity.sqrMagnitude < 0.01f);

        while (Vector3.Distance(soundSource, enemy.transform.position) > agent.stoppingDistance)
        {
            if (enemy.visionController.isSeeingPlayer)
            {
                yield return LookingAtPlayer();

                continue;
            }

            agent.SetDestination(soundSource);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);
    }
}
