using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(Health), typeof(StunController), typeof(VisionController))]
public class EnemyController : MonoBehaviour, IMoveable, IMortal, ISoundListener
{
    [SerializeField, Min(0f)] private float calmSpeed = 1.5f;
    [SerializeField, Min(0f)] private float alarmedSpeed = 3.5f;
    [SerializeField, Min(0f)] private float attackRange = 1f;
    [SerializeField] private ItemData droppedItem;
    [SerializeField] private Patroler patroler;
    [SerializeField] private PlayerSeeker playerSeeker;
    [SerializeField] private SoundResponder soundResponder;
    [Inject(Id = CustomLayer.Player)] private LayerMask playerLayer;
    [Inject(Id = CustomLayer.Interactable)] private LayerMask hideoutLayer;
    [Inject] private PlayerController playerRef;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;
    private Weapon weapon;
    private bool isDying = false;
    
    [Inject] public AIManager aiManager { get; private set; }
    public StunController stunController { get; private set; }
    public VisionController visionController { get; private set; }
    public bool canMove { get => !agent.isStopped; set => agent.isStopped = !value; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        visionController = GetComponent<VisionController>();

        stunController = GetComponent<StunController>();
        stunController.onStunStart += () => {
            canMove = false;
            animator.SetBool("isStunned", true);
            StopBehavior();
            visionController.StopWatching();
        };
        stunController.onStunEnd += () => {
            canMove = true;
            animator.SetBool("isStunned", false);
            visionController.StartWatching();
            Alert();
        };

        weapon = GetComponentInChildren<Weapon>();
        patroler.Initialize(this);
        playerSeeker.Initialize(this);
        soundResponder.Initialize(this);

        health.onDeath += Die;
    }

    void Start()
    {
        aiManager.enemies.Add(this);

        agent.speed = calmSpeed;

        visionController.StartWatching();

        Patrol();
    }

    void Update()
    {
        if (stunController.isStunned || isDying)
            return;

        AttackPlayer();
        Move();
        UnhidePlayer();
    }

    private void UnhidePlayer()
    {
        if (visionController.hasSeenPlayerHiding)
        {
            var cols = Physics.OverlapSphere(transform.position, attackRange, hideoutLayer.value);
            
            if (cols.Length != 0)
            {
                foreach (var h in cols)
                {
                    if (h.GetComponent<Hideout>() == playerRef.currentHideout)
                    {
                        playerRef.ExitHideout();
                        visionController.hasSeenPlayerHiding = false;
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
        StopBehavior();

        visionController.ResetNoticeClock();

        if (!stunController.isStunned && !isDying)
            agent.isStopped = false;

        agent.speed = alarmedSpeed;
        animator.SetBool("isAlarmed", true);
    }

    public void UnsetAlarmedState()
    {
        StopBehavior();

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

        StopBehavior();
        visionController.StopWatching();

        animator.SetTrigger("death");
    }

    public void Alert()
    {
        visionController.player = playerRef.transform;
        visionController.forgetClock = 0f;
        aiManager.SoundTheAlarm();
    }

    public void RespondToSound(Vector3 source)
    {
        if (!aiManager.alarm)
        {
            StopBehavior();

            soundResponder.soundSource = source;
            StartCoroutine(soundResponder.RespondingToSound());
        }
        else if (aiManager.lookingForPlayer)
        {
            agent.SetDestination(source);
        }
    }

    public void StopBehavior()
    {
        StopAllCoroutines();
    }

    public void ResumeBehavior()
    {
        if (!aiManager.alarm)
        {
            Patrol();
        }
        else if (aiManager.lookingForPlayer)
        {
            FindPlayer();
        }
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
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void OnDeath()
    {
        if (droppedItem != null)
            Instantiate(droppedItem.prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}