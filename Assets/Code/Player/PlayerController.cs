using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Infringed.Combat;
using Infringed.Map;
using Infringed.InventorySystem;
using Infringed.Actions;
using Infringed.Quests;

namespace Infringed.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IAttacker
    {
        public event Action OnHide;
        public event Action OnExitHideout;
        public event Action OnPlayerAttackStart;
        public event Action<DialogueGiver> OnInitiateDialogue;
        public event Action OnEssentia;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _crouchingSpeedFactor = 0.3f;
        [SerializeField, Min(0f)] private float _itemPickupRadius = 5f;
        [SerializeField, Min(0f)] private float _soundRadius = 6f;
        [SerializeField] private OnPlayerDeathActivator _menu;
        [SerializeField] private GameObject[] _objectsToDisableOnHide;
        [field: SerializeField] public Inventory Inventory { get; private set; }
        [Inject(Id = CustomLayer.Floor)] private LayerMask _floorMask;
        [Inject(Id = CustomLayer.Interactable)] private LayerMask _interactableMask;
        private CharacterController _characterController;
        private PlayerInput _input;
        private Health _health;
        private Weapon _weapon;
        private float _verticalAcceleration = 0f;
        private bool _isDying = false;
        private bool _dontUseItem = false;
        private DialogueGiver _giver;
        private ActionCastMarker _castMarker;
        public Hideout CurrentHideout { get; private set; }
        public Belt Belt { get; private set; }
        public SkillSet SkillSet { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public bool CanMove { get; set; } = true;
        public bool IsCrouching { get; private set; } = false;

        public bool IsHiding => CurrentHideout != null;

        private void Awake()
        {
            Belt = new Belt(Inventory);

            _input = GetComponent<PlayerInput>();
            _characterController = GetComponent<CharacterController>();
            _health = GetComponent<Health>();
            _weapon = GetComponentInChildren<Weapon>();
            SkillSet = GetComponent<SkillSet>();

            _input.actions["Menu"].performed += _OnMenu;
        }

        private void OnEnable()
        {
            _health.OnDeathStart += _Die;

            _Subscribe();
        }

        private void OnDisable()
        {
            _health.OnDeathStart -= _Die;

            _Unsubscribe();
        }

        private void OnDestroy()
        {
            _input.actions["Menu"].performed -= _OnMenu;
        }

        private void Update()
        {
            if (!IsHiding)
            {
                _CalculateGravity();

                if (CanMove && !_isDying && _giver == null)
                {
                    _Move();
                    _Turn();
                }
            }
        }

        private void FixedUpdate()
        {
            if (IsHiding || !CanMove || _isDying || _giver != null)
                return;

            var inputValue = _input.actions["Move"].ReadValue<Vector2>();

            if (inputValue != Vector2.zero && !IsCrouching)
            {
                SoundUtil.SpawnSound(transform.position, _soundRadius);
            }
        }

#if UNITY_EDITOR
        [Header("Debug info")]
        [SerializeField] private bool _showMoveDirection;
        [SerializeField] private Color _moveDirectionColor = Color.blue;
        [SerializeField] private bool _showItemPickupRadius;
        [SerializeField] private Color _itemPickupRadiusColor = Color.blue;
        [SerializeField] private bool _showSoundRadius;
        [SerializeField] private Color _soundRadiusColor = Color.magenta;

        private void OnDrawGizmos()
        {
            if (_showMoveDirection)
            {
                Gizmos.color = _moveDirectionColor;
                Gizmos.DrawRay(transform.position, MoveDirection);
            }

            if (_showItemPickupRadius)
            {
                Gizmos.color = _itemPickupRadiusColor;
                Gizmos.DrawWireSphere(transform.position, _itemPickupRadius);
            }

            if (_showSoundRadius)
            {
                Gizmos.color = _soundRadiusColor;
                Gizmos.DrawWireSphere(transform.position, _soundRadius);
            }
        }
#endif

        public void ExitHideout()
        {
            _input.actions["Interact"].performed -= _OnExitHideout;
            _input.actions["Fire"].performed += _OnAttack;
            _input.actions["Interact"].performed += _OnInteract;

            transform.position += CurrentHideout.transform.forward;
            CurrentHideout = null;
            _characterController.enabled = true;

            foreach (var toDisable in _objectsToDisableOnHide)
            {
                toDisable.SetActive(true);
            }

            OnExitHideout?.Invoke();
        }

        public void PickItem(ItemPickable pickable)
        {
            if (Inventory.AddItem(pickable.GetItem()))
                pickable.DestroySelf();
        }

        public void Hide(Hideout hideout)
        {
            _input.actions["Fire"].performed -= _OnAttack;
            _input.actions["Interact"].performed -= _OnInteract;
            _input.actions["Interact"].performed += _OnExitHideout;

            CurrentHideout = hideout;
            _characterController.enabled = false;

            foreach (var toDisable in _objectsToDisableOnHide)
            {
                toDisable.SetActive(false);
            }

            transform.position = CurrentHideout.transform.position;

            OnHide?.Invoke();
        }

        public void AttackStateStarted()
        {
            CanMove = false;
        }

        public void AttackStateEnded()
        {
            CanMove = true;
        }

        public void InitiateDialogue(DialogueGiver giver)
        {
            giver.OnDialogueEnd += _OnDialogueEnd;
            _giver = giver;

            _Unsubscribe();

            OnInitiateDialogue?.Invoke(giver);
        }

        public void ConsumeEssentia()
        {
            OnEssentia?.Invoke();
        }

        private void _OnDialogueEnd(DialogueGiver giver)
        {
            giver.OnDialogueEnd -= _OnDialogueEnd;
            _giver = null;

            _Subscribe();
        }

        private void _OnAttack(InputAction.CallbackContext obj)
        {
            if (CanMove && !_isDying)
                _Attack();
        }

        private void _OnUseItemPerformed(InputAction.CallbackContext context)
        {
            _dontUseItem = true;

            if (UserRaycaster.IsBlockedByUI())
                return;

            var item = Belt.SelectedItem;

            if (item == null)
                return;

            if (!Belt.HasItemsInInventoty(Belt.SelectedSlot))
                return;

            var castMarkerPrefab = item.Data.Action?.CastMarkerPrefab;

            if (castMarkerPrefab == null)
            {
                _dontUseItem = false;
                return;
            }

            _castMarker = Instantiate(castMarkerPrefab);

            _castMarker.Actor = transform;
            _castMarker.Action = item.Data.Action;
            _castMarker.GetTargetPosition = () =>
            {
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(camRay, out var floorHit, Mathf.Infinity, _floorMask.value);

                return floorHit.point;
            };

            _dontUseItem = false;
        }

        private void _OnUseItemCanceled(InputAction.CallbackContext obj)
        {
            Destroy(_castMarker?.gameObject);
            _castMarker = null;

            if (_dontUseItem)
            {
                _dontUseItem = false;
                return;
            }

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(camRay, out var floorHit, Mathf.Infinity, _floorMask.value);

            Belt.UseItem(new(transform, floorHit.point));
        }

        private void _OnExitHideout(InputAction.CallbackContext obj)
        {
            ExitHideout();
        }

        private void _OnInteract(InputAction.CallbackContext obj)
        {
            _Interact();
        }

        private void _OnSelectItem(InputAction.CallbackContext obj)
        {
            int value = (int)obj.ReadValue<float>();

            if (value != 0)
                Belt.SelectedSlot = value - 1;
        }

        private void _OnSwitchItem(InputAction.CallbackContext obj)
        {
            int value = (int)obj.ReadValue<float>();

            if (value > 0)
            {
                Belt.SelectedSlot = (Belt.SelectedSlot + 1) % Belt.Size;
            }
            else
            {
                Belt.SelectedSlot = (Belt.Size + Belt.SelectedSlot - 1) % Belt.Size;
            }
        }

        private void _OnCrouchPerformed(InputAction.CallbackContext obj)
        {
            IsCrouching = true;
        }

        private void _OnCrouchCanceled(InputAction.CallbackContext obj)
        {
            IsCrouching = false;
        }

        private void _OnMenu(InputAction.CallbackContext obj)
        {
            _menu.SwitchStatus();
        }

        private void _Move()
        {
            var inputValue = _input.actions["Move"].ReadValue<Vector2>();
            MoveDirection = new Vector3(inputValue.x, 0f, inputValue.y);

            var movement = MoveDirection * _speed * Time.deltaTime;

            if (IsCrouching)
            {
                movement *= _crouchingSpeedFactor;
                MoveDirection *= _crouchingSpeedFactor;
            }

            _characterController.Move(movement);
        }

        private void _Turn()
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, _floorMask.value))
            {
                Vector3 hitPoint = floorHit.point;
                hitPoint.y = transform.position.y;
                transform.LookAt(hitPoint);
            }
        }

        private void _Attack()
        {
            if (!UserRaycaster.IsBlockedByUI())
            {
                OnPlayerAttackStart?.Invoke();
            }
        }

        private void _Interact()
        {
            var colliders = Physics.OverlapSphere(transform.position, _itemPickupRadius, _interactableMask.value);

            if (colliders.Length == 0)
                return;

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(camRay, out var hit, Mathf.Infinity, _interactableMask.value))
                return;

            foreach (var col in colliders)
            {
                if (col == hit.collider)
                {
                    if (!Physics.Linecast(transform.position + Vector3.up, hit.point, ~_interactableMask))
                    {
                        hit.transform.GetComponent<IInteractable>().Interact(this);
                    }

                    break;
                }
            }
        }

        private void _CalculateGravity()
        {
            if (_characterController.isGrounded)
            {
                _verticalAcceleration = 0f;
            }
            else
            {
                _verticalAcceleration += Physics.gravity.y * Time.deltaTime * Time.deltaTime;
                _characterController.Move(new Vector3(0f, _verticalAcceleration, 0f));
            }
        }

        private void _Die()
        {
            gameObject.layer = 0;
            _isDying = true;
            CanMove = false;

            _Unsubscribe();
        }

        private void _Subscribe()
        {
            _input.actions["Fire"].performed += _OnAttack;
            _input.actions["Interact"].performed += _OnInteract;
            _input.actions["SelectItem"].performed += _OnSelectItem;
            _input.actions["SwitchItem"].performed += _OnSwitchItem;
            _input.actions["Crouch"].performed += _OnCrouchPerformed;
            _input.actions["Crouch"].canceled += _OnCrouchCanceled;
            _input.actions["UseItem"].performed += _OnUseItemPerformed;
            _input.actions["UseItem"].canceled += _OnUseItemCanceled;
        }

        private void _Unsubscribe()
        {
            _input.actions["Fire"].performed -= _OnAttack;
            _input.actions["Interact"].performed -= _OnInteract;
            _input.actions["SelectItem"].performed -= _OnSelectItem;
            _input.actions["SwitchItem"].performed -= _OnSwitchItem;
            _input.actions["Crouch"].performed -= _OnCrouchPerformed;
            _input.actions["Crouch"].canceled -= _OnCrouchCanceled;
            _input.actions["UseItem"].performed -= _OnUseItemPerformed;
            _input.actions["UseItem"].canceled -= _OnUseItemCanceled;

            _dontUseItem = true;
            _OnUseItemCanceled(default);
        }

        public void OnAttackStartEvent()
        {
            _weapon.StartDamaging();
        }

        public void OnAttackEndEvent()
        {
            _weapon.StopDamaging();
        }
    }
}
