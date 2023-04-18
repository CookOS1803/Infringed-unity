using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemyController : MonoBehaviour, IMoveable, IMortal, ISoundListener
{
    [SerializeField, Min(0f)] private float calmSpeed = 1.5f;
    [SerializeField, Min(0f)] private float alarmedSpeed = 3.5f;
    [SerializeField, Min(0f)] private float attackRange = 1f;
    [SerializeField, Min(0f)] private float _distanceOfView = 10f;
    [SerializeField, Range(0f, 360f)] private float _fieldOfView = 90f;
    [SerializeField, Min(0f)] private float noticeTime = 2f;
    [SerializeField, Min(0f)] private float forgetTime = 0.5f;
    [SerializeField, Min(0f)] private float stunTime = 2f;
    [SerializeField] private ItemData droppedItem;
    [SerializeField] private Patroler patroler;
    [SerializeField] private PlayerSeeker playerSeeker;
    [Inject(Id = CustomLayer.Player)] private LayerMask playerLayer;
    [Inject(Id = CustomLayer.Interactable)] private LayerMask hideoutLayer;
    [Inject] private AIManager aiManager;
    [Inject] private PlayerController playerRef;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;
    private Weapon weapon;
    private float _noticeClock = 0f;
    public float noticeClock
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
    private bool _isSeeingPlayer = false;
    private bool isDying = false;
    private bool hasSeenPlayerHiding = false;
    
    public Transform player { get; private set; }
    public bool isStunned { get; private set; } = false;
    public bool canMove { get => !agent.isStopped; set => agent.isStopped = !value; }
    public float normalizedNoticeClock => noticeClock / noticeTime;
    public float distanceOfView => _distanceOfView;
    public float fieldOfView => _fieldOfView;
    public bool isSeeingPlayer => _isSeeingPlayer;

    public event Action onNoticeClockChange;
    public event Action onNoticeClockReset;

    public void ResetNoticeClock()
    {
        noticeClock = 0f;

        onNoticeClockReset?.Invoke();
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<Weapon>();
        patroler.Initialize(this);
        playerSeeker.Initialize(this, aiManager);

        health.onDeath += Die;
    }

    void Start()
    {
        aiManager.enemies.Add(this);

        agent.speed = calmSpeed;

        Patrol();
    }

    void Update()
    {
        if (isStunned || isDying)
            return;

        _isSeeingPlayer = NoticePlayer();

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
                _isSeeingPlayer = false;
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

    private Coroutine findingPlayerRoutine;
    public void FindPlayer()
    {
        if (findingPlayerRoutine != null)
            StopCoroutine(findingPlayerRoutine);

        findingPlayerRoutine = StartCoroutine(playerSeeker.FindingPlayer());
    }

    private Coroutine patrolingRoutine;
    public void Patrol()
    {
        if (patrolingRoutine != null)
            StopCoroutine(patrolingRoutine);

        patrolingRoutine = StartCoroutine(patroler.Patroling());
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

    public void ReactToSound()
    {
        Debug.Log(name + " heard player");
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