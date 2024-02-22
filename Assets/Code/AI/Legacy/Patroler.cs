using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Infringed.Legacy
{
    [System.Serializable, System.Obsolete]
    public class Patroler
    {
        [SerializeField, Min(0f)] private float waitTime = 2f;
        [SerializeField] private Transform[] patrolPoints;
        private int currentPoint;
        private EnemyController enemy;
        private NavMeshAgent agent;
        private float waitClock = 0f;

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
                yield return GoingToPatrolPoint();

                currentPoint = (currentPoint + 1) % patrolPoints.Length;

                if (patrolPoints.Length == 1)
                    yield return new WaitForEndOfFrame();
                else
                {
                    while (waitClock < waitTime)
                    {
                        waitClock += Time.deltaTime;

                        yield return new WaitForEndOfFrame();
                    }

                    waitClock = 0f;
                }
            }
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
                () => Vector3.Distance(agent.destination, enemy.transform.position) <= agent.stoppingDistance
            );
        }

        private IEnumerator RotatingTowards(Quaternion desiredRotation)
        {
            while (enemy.transform.rotation != desiredRotation)
            {
                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
