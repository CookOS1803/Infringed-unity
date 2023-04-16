using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemyController : MonoBehaviour, IMoveable, IMortal
{
    [SerializeField, Min(0f)] private float calmSpeed = 1.5f;
    [SerializeField, Min(0f)] private float alarmedSpeed = 3.5f;
    [SerializeField, Min(0f)] private float attackRange = 1f;
    [SerializeField, Min(0f)] private float _distanceOfView = 10f;
    [SerializeField, Range(0f, 360f)] private float _fieldOfView = 90f;
    [SerializeField, Min(0f)] private float noticeTime = 2f;
    [SerializeField, Min(0f)] private float unseeFactor = 0.5f;
    [SerializeField, Min(0f)] private float waitTime = 2f;
    [SerializeField, Min(0f)] private float forgetTime = 0.5f;
    [SerializeField, Min(0f)] private float stunTime = 2f;
    [SerializeField, Min(0f)] private float findingRadius = 6f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private ItemData droppedItem;
    [Inject(Id = CustomLayer.Player)] private LayerMask playerLayer;
    [Inject(Id = CustomLayer.Interactable)] private LayerMask hideoutLayer;
    [Inject] private AIManager aiManager;
    [Inject] private PlayerController playerRef;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;
    private Weapon weapon;
    private int currentPoint;
    private float _noticeClock = 0f;
    private float noticeClock
    {
        get => _noticeClock;
        set
        {
            _noticeClock = value;

            onNoticeClockChange?.Invoke();
        }
    }
    private float forgetClock = 0f;
    private float stunClock = 0f;
    private bool isSeeingPlayer = false;
    private bool isDying = false;
    private bool hasSeenPlayerHiding = false;
    
    public Transform player { get; private set; }
    public bool isStunned { get; private set; } = false;
    public bool canMove { get => !agent.isStopped; set => agent.isStopped = !value; }
    public float normalizedNoticeClock => noticeClock / noticeTime;
    public float distanceOfView => _distanceOfView;
    public float fieldOfView => _fieldOfView;

    public event Action onNoticeClockChange;
    public event Action onNoticeClockReset;

    void ResetNoticeClock()
    {
        noticeClock = 0f;

        onNoticeClockReset?.Invoke();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<Weapon>();

        health.onDeath += Die;

        aiManager.enemies.Add(this);

        agent.speed = calmSpeed;

        Patrol();
    }

    void Update()
    {
        if (isStunned || isDying)
            return;

        isSeeingPlayer = NoticePlayer();

        AttackPlayer();
        Move();
        UnhidePlayer();

        if (aiManager.alarm && player != null)
        {
            if (forgetClock < forgetTime)
                forgetClock += Time.deltaTime;
            else
            {
                if (player != null)
                {
                    playerRef.onHide -= OnPlayerHide;
                    playerRef.onExitHideout -= OnPlayerExitHideout;
                }
                player = null;
                isSeeingPlayer = false;
            }
        }
    }

    private void UnhidePlayer()
    {
        if (hasSeenPlayerHiding)
        {
            var cols = Physics.OverlapSphere(transform.position, attackRange, hideoutLayer.value);
            
            if (cols.Length != 0)
            {
                foreach (var h in cols)
                {
                    if (h.GetComponent<Hideout>() == playerRef.currentHideout)
                    {
                        playerRef.ExitHideout();
                        hasSeenPlayerHiding = false;
                    }
                }
            }
        }
    }

    private void Move()
    {
        if (aiManager.player != null)
            agent.SetDestination(aiManager.playerLastKnownPosition);

        animator.SetBool("isMoving", agent.velocity != Vector3.zero);
    }

    private bool NoticePlayer()
    {
        var cols = Physics.OverlapSphere(transform.position, distanceOfView, playerLayer.value);

        // Player is too far
        if (cols.Length == 0)
            return false;

        float angle = Vector3.Angle(transform.forward, cols[0].transform.position - transform.position);

        // Player is not in field of view
        if (angle > fieldOfView * 0.5f)
            return false;

        return CanSeePlayer(cols[0]);              
        
    }

    private bool CanSeePlayer(Collider col)
    {
        RaycastHit hit;
        Physics.Linecast(transform.position + Vector3.up, col.transform.position + Vector3.up, out hit);

        // Can see player clearly
        if (hit.collider != null && hit.collider.GetComponent<PlayerController>() != null)
        {
            if (!aiManager.alarm && noticeClock < noticeTime)
            {
                transform.LookAt(col.transform);

                noticeClock += Time.deltaTime * (distanceOfView / Vector3.Distance(transform.position, col.transform.position));
            }
            else
            {
                if (player == null)
                {
                    playerRef.onHide += OnPlayerHide;
                    playerRef.onExitHideout += OnPlayerExitHideout;
                }
                player = hit.transform;
                forgetClock = 0f;
                aiManager.SoundTheAlarm();
            }
            return true;
        }
        else
            return false;
    }

    private void OnPlayerHide()
    {
        hasSeenPlayerHiding = true;
    }

    private void OnPlayerExitHideout()
    {
        hasSeenPlayerHiding = false;
    }
    
    private void AttackPlayer()
    {
        if (aiManager.player != null && canMove && Physics.OverlapSphere(transform.position, attackRange, playerLayer.value).Length != 0)
        {
            transform.LookAt(aiManager.player);
            animator.SetTrigger("attack");
        }
    }

    public void FindPlayer()
    {
        if (findingPlayerRoutine != null)
            StopCoroutine(findingPlayerRoutine);

        findingPlayerRoutine = StartCoroutine(FindingPlayer());
    }

    private Coroutine findingPlayerRoutine;
    private IEnumerator FindingPlayer()
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

                if (aiManager.player == null)
                {
                    GoToRandomPoint();

                    yield return new WaitUntil (() => agent.velocity.sqrMagnitude < 0.01f);
                }
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

    public void Patrol()
    {
        if (patrolingRoutine != null)
            StopCoroutine(patrolingRoutine);

        patrolingRoutine = StartCoroutine(Patroling());
    }

    private Coroutine patrolingRoutine;
    private IEnumerator Patroling()
    {
        float waitClock = 0f;

        while (true)
        {
            if (isSeeingPlayer)
            {
                yield return LookingAtPlayer();

                continue;
            }

            yield return GoingToPatrolPoint();

            if (!isSeeingPlayer)
                currentPoint = (currentPoint + 1) % patrolPoints.Length;

            if (patrolPoints.Length == 1)
                yield return new WaitForEndOfFrame();
            else
            {
                while (waitClock < waitTime && !isSeeingPlayer)
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
        agent.isStopped = true;

        yield return new WaitUntil(() => !isSeeingPlayer);

        while (noticeClock > 0f)
        {
            noticeClock -= Time.deltaTime * unseeFactor;

            yield return new WaitForEndOfFrame();
        }

        ResetNoticeClock();

        agent.isStopped = false;
    }

    private IEnumerator GoingToPatrolPoint()
    {
        agent.SetDestination(patrolPoints[currentPoint].position);

        yield return new WaitWhile(() => agent.velocity.sqrMagnitude < 0.01f);
        yield return new WaitUntil(
            () => Vector3.Distance(agent.destination, transform.position) <= agent.stoppingDistance || isSeeingPlayer
        );

        yield return RotatingTowards(patrolPoints[currentPoint].rotation);
    }

    private IEnumerator RotatingTowards(Quaternion desiredRotation)
    {
        while (transform.rotation != desiredRotation && !isSeeingPlayer && !aiManager.alarm && !isDying && !isStunned)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetAlarmedState()
    {
        if (patrolingRoutine != null)
            StopCoroutine(patrolingRoutine);

        ResetNoticeClock();

        if (!isStunned && !isDying)
            agent.isStopped = false;

        agent.speed = alarmedSpeed;
        animator.SetBool("isAlarmed", true);
    }

    public void UnsetAlarmedState()
    {
        if (findingPlayerRoutine != null)
            StopCoroutine(findingPlayerRoutine);

        agent.speed = calmSpeed;
        animator.SetBool("isAlarmed", false);
    }

    public void OnAttackStartEvent()
    {
        weapon.StartDamaging();
    }

    public void OnAttackEndEvent()
    {
        weapon.StopDamaging();
    }

    public void Die()
    {
        if (isDying)
            return;

        canMove = false;
        isDying = true;

        StopAllCoroutines();

        animator.SetTrigger("death");
    }

    public void Stun()
    {
        if (isStunned)
            stunClock = 0f;
        else
        {
            StopAllCoroutines();

            stunRoutine = StartCoroutine(StunRoutine());         
        }
    }

    private Coroutine stunRoutine;
    private IEnumerator StunRoutine()
    {
        isStunned = true;
        canMove = false;

        animator.SetBool("isStunned", true);

        while (stunClock < stunTime)
        {
            stunClock += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }        
        
        animator.SetBool("isStunned", false);

        stunClock = 0f;

        canMove = true;
        isStunned = false;

        Alert();
    }

    public void Alert()
    {
        player = playerRef.transform;
        forgetClock = 0f;
        aiManager.SoundTheAlarm();
    }

    void OnDestroy()
    {
        aiManager.enemies.Remove(this);
    }

    void OnTriggerEnter(Collider collider)
    {
        var door = collider.GetComponent<Door>();

        door?.OpenTemporarily();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, distanceOfView);

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, fieldOfView / 2f, 0f)  * (transform.forward) * distanceOfView);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, -fieldOfView / 2f, 0f) * (transform.forward) * distanceOfView);
    }

    public void OnDeath()
    {
        if (droppedItem != null)
            Instantiate(droppedItem.prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}