using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Infringed.Legacy
{
    [System.Serializable]
    public enum EnemyState
    {
        Patroling, LookingAtPlayer, HearingSound, RespondingToSound, ChasingPlayer, SeekingPlayer
    }

    [System.Obsolete]
    [RequireComponent(typeof(Combat.Health), typeof(Combat.StunController), typeof(VisionController))]
    public class EnemyController : MonoBehaviour, IAttacker, IMortal, ISoundListener
    {
        [SerializeField, Min(0f)] private float calmSpeed = 1.5f;
        [SerializeField, Min(0f)] private float alarmedSpeed = 3.5f;
        [SerializeField, Min(0f)] private float attackRange = 1f;
        [SerializeField] private InventorySystem.ItemData droppedItem;
        [SerializeField] private Patroler patroler;
        [SerializeField] private PlayerSeeker playerSeeker;
        [SerializeField] private SoundResponder soundResponder;
        [Inject(Id = CustomLayer.Player)] private LayerMask playerLayer;
        [Inject(Id = CustomLayer.Interactable)] private LayerMask hideoutLayer;
        [Inject] private Player.PlayerController playerRef;
        [Inject] private CustomAudio customAudio;
        private EnemyState _enemyState;
        private NavMeshAgent agent;
        private Animator animator;
        private Combat.Health health;
        private Weapon weapon;
        private bool isDying = false;

        [Inject] public AIManager aiManager { get; private set; }
        public EnemyState enemyState
        {
            get => _enemyState;
            set
            {
                _enemyState = value;

                onStateChange?.Invoke();
            }
        }
        public Combat.StunController stunController { get; private set; }
        public VisionController visionController { get; private set; }
        public bool CanMove { get => !agent.isStopped; set => agent.isStopped = !value; }

        public event System.Action onStateChange;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Combat.Health>();
            visionController = GetComponent<VisionController>();

            stunController = GetComponent<Combat.StunController>();
            stunController.OnStunStart += () =>
            {
                CanMove = false;
                animator.SetBool("isStunned", true);
                StopBehavior();
                visionController.StopWatching();
            };
            stunController.OnStunEnd += () =>
            {
                CanMove = true;
                animator.SetBool("isStunned", false);
                visionController.StartWatching();
                Alert();
            };

            weapon = GetComponentInChildren<Weapon>();
            patroler.Initialize(this);
            playerSeeker.Initialize(this);
            soundResponder.Initialize(this);

            health.OnDeath += Die;
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
            if (stunController.IsStunned || isDying)
                return;

            AttackPlayer();
            Move();
            UnhidePlayer();
        }

        void OnDestroy()
        {
            aiManager.enemies.Remove(this);
        }

        void OnTriggerEnter(Collider collider)
        {
            var door = collider.GetComponent<Map.Door>();

            door?.OpenTemporarily();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, attackRange);
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
                        if (h.GetComponent<Map.Hideout>() == playerRef.CurrentHideout)
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
            if (aiManager.player != null && CanMove && Physics.OverlapSphere(transform.position, attackRange, playerLayer.value).Length != 0)
            {
                transform.LookAt(aiManager.player);
                animator.SetTrigger("attack");
            }
        }

        public void AttackStarted()
        {
            CanMove = false;
        }

        public void AttackEnded()
        {
            CanMove = true;
        }

        public void SeekPlayer()
        {
            StopBehavior();
            StartCoroutine(playerSeeker.FindingPlayer());
            enemyState = EnemyState.SeekingPlayer;
        }

        public void Patrol()
        {
            StopBehavior();
            StartCoroutine(patroler.Patroling());
            enemyState = EnemyState.Patroling;
        }

        public void SetAlarmedState()
        {
            if (!aiManager.alarm)
            {
                StopBehavior();

                visionController.ResetNoticeClock();

                if (!stunController.IsStunned && !isDying)
                    agent.isStopped = false;

                agent.speed = alarmedSpeed;
                animator.SetBool("isAlarmed", true);
            }

            enemyState = EnemyState.ChasingPlayer;
        }

        public void UnsetAlarmedState()
        {
            StopBehavior();

            agent.speed = calmSpeed;
            animator.SetBool("isAlarmed", false);
        }

        public void OnSwingEvent()
        {
            AudioSource.PlayClipAtPoint(customAudio.WeaponSwing, transform.position);
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

            CanMove = false;
            isDying = true;

            StopBehavior();
            visionController.StopWatching();

            animator.SetTrigger("death");
        }

        public void Alert()
        {
            if (isDying)
                return;

            visionController.player = playerRef.transform;
            visionController.forgetClock = 0f;
            aiManager.SoundTheAlarm();
        }

        public void RespondToSound(Vector3 source)
        {
            if (isDying)
                return;

            soundResponder.soundSource = source;

            if (!aiManager.alarm)
            {
                if (enemyState != EnemyState.LookingAtPlayer &&
                    enemyState != EnemyState.HearingSound &&
                    enemyState != EnemyState.RespondingToSound)
                {
                    StopBehavior();

                    StartCoroutine(soundResponder.HearingSound(() =>
                    {
                        StopBehavior();

                        StartCoroutine(soundResponder.RespondingToSound());
                        enemyState = EnemyState.RespondingToSound;
                    }));
                    enemyState = EnemyState.HearingSound;
                }
                else if (enemyState == EnemyState.RespondingToSound)
                {
                    StopBehavior();

                    StartCoroutine(soundResponder.RespondingToSound());
                }
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
                SeekPlayer();
            }
        }

        public void OnDeathEnd()
        {
            if (droppedItem != null)
                Instantiate(droppedItem.Prefab, transform.position + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
        }

        public void OnStep()
        {
            AudioSource.PlayClipAtPoint(customAudio.GetRandomStep(), transform.position);
        }
    }
}
