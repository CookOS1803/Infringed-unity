using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Infringed.Combat;
using Infringed.Map;
using Infringed.InventorySystem;

namespace Infringed.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IAttacker, IMortal
    {
        public event Action OnHide;
        public event Action OnExitHideout;
        public event Action OnPlayerDeath;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _crouchingSpeedFactor = 0.3f;
        [SerializeField, Min(0f)] private float _itemPickupRadius = 5f;
        [SerializeField, Min(0f)] private float _soundRadius = 6f;
        [SerializeField] private OnPlayerDeathActivator _menu;
        [Inject] private CustomAudio _customAudio;
        [Inject(Id = CustomLayer.Floor)] private LayerMask _floorMask;
        [Inject(Id = CustomLayer.Interactable)] private LayerMask _interactableMask;
        private PlayerAnimator _playerAnimator;
        private CharacterController _characterController;
        private PlayerInput _input;
        private Health _health;
        private Weapon _weapon;
        private Vector3 _moveDirection;
        private float _verticalAcceleration = 0f;
        private bool _isDying = false;
        private bool _isCrouching = false;
        public Hideout CurrentHideout { get; private set; }
        public bool CanMove { get; set; } = true;

        // a nahuya injectit'
        [Inject] public Inventory Inventory { get; private set; }

        // why start
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _health = GetComponent<Health>();
            _playerAnimator = new PlayerAnimator(transform);
            _weapon = GetComponentInChildren<Weapon>();

            _health.OnDeath += _Die;

            _input = GetComponent<PlayerInput>();

            // maybe v OnEnable
            _input.actions["Fire"].performed += _OnAttack;
            _input.actions["UseItem"].performed += _OnUseItem;
            _input.actions["Interact"].performed += _OnInteract;
            _input.actions["SelectItem"].performed += _OnSelectItem;
            _input.actions["SwitchItem"].performed += _OnSwitchItem;
            _input.actions["Crouch"].performed += _OnCrouchPerformed;
            _input.actions["Crouch"].canceled += _OnCrouchCanceled;
            _input.actions["Menu"].performed += _OnMenu;
        }

        private void Update()
        {
            if (CurrentHideout == null)
            {
                _CalculateGravity();

                if (CanMove && !_isDying)
                {
                    _Move();
                    _Turn();
                }
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
                Gizmos.DrawRay(transform.position, _moveDirection);
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

            foreach (Transform c in transform)
            {
                c.gameObject.SetActive(true);
            }

            OnExitHideout?.Invoke();
        }

        public void OnDeathEnd()
        {
            OnPlayerDeath?.Invoke();

            // maybe v OnDisable
            _input.actions["Fire"].performed -= _OnAttack;
            _input.actions["UseItem"].performed -= _OnUseItem;
            _input.actions["Interact"].performed -= _OnInteract;
            _input.actions["SelectItem"].performed -= _OnSelectItem;
            _input.actions["SwitchItem"].performed -= _OnSwitchItem;
            _input.actions["Crouch"].performed -= _OnCrouchPerformed;
            _input.actions["Crouch"].canceled -= _OnCrouchCanceled;

            enabled = false;
        }

        public void PickItem(ItemPickable pickable)
        {
            Inventory.Add(pickable.GetItem());
            pickable.DestroySelf();
        }

        public void Hide(Hideout hideout)
        {
            _input.actions["Fire"].performed -= _OnAttack;
            _input.actions["Interact"].performed -= _OnInteract;
            _input.actions["Interact"].performed += _OnExitHideout;

            CurrentHideout = hideout;
            _characterController.enabled = false;

            foreach (Transform c in transform)
            {
                c.gameObject.SetActive(false);
            }

            transform.position = CurrentHideout.transform.position;

            OnHide?.Invoke();
        }

        public void AttackStarted()
        {
            CanMove = false;
        }

        public void AttackEnded()
        {
            CanMove = true;
        }

        private void _OnAttack(InputAction.CallbackContext obj)
        {
            if (CanMove && !_isDying)
                _Attack();
        }

        private void _OnUseItem(InputAction.CallbackContext obj)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(camRay, out var floorHit, Mathf.Infinity, _floorMask.value);

            Inventory.UseItem(new(transform, floorHit.point));
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
                Inventory.SelectedSlot = value - 1;
        }

        private void _OnSwitchItem(InputAction.CallbackContext obj)
        {
            int value = (int)obj.ReadValue<float>();

            if (value > 0)
            {
                Inventory.SelectedSlot = (Inventory.SelectedSlot + 1) % Inventory.Size;
            }
            else
            {
                Inventory.SelectedSlot = (Inventory.Size + Inventory.SelectedSlot - 1) % Inventory.Size;
            }
        }

        private void _OnCrouchPerformed(InputAction.CallbackContext obj)
        {
            _isCrouching = true;
        }

        private void _OnCrouchCanceled(InputAction.CallbackContext obj)
        {
            _isCrouching = false;
        }

        private void _OnMenu(InputAction.CallbackContext obj)
        {
            _menu.SwitchStatus();
        }

        private void _Move()
        {
            var inputValue = _input.actions["Move"].ReadValue<Vector2>();
            _moveDirection = new Vector3(inputValue.x, 0f, inputValue.y);

            var movement = _moveDirection * _speed * Time.deltaTime;

            if (_isCrouching)
            {
                movement *= _crouchingSpeedFactor;
                _moveDirection *= _crouchingSpeedFactor;
            }
            else if (movement != Vector3.zero)
            {
                SoundUtil.SpawnSound(transform.position, _soundRadius);
            }

            _characterController.Move(movement);

            _playerAnimator.AnimateMovement(_moveDirection);
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
                _playerAnimator.Attack();
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
                _verticalAcceleration -= 9.81f * Time.deltaTime * Time.deltaTime;
                _characterController.Move(new Vector3(0f, _verticalAcceleration, 0f));
            }
        }

        private void _Die()
        {
            gameObject.layer = 0;
            _isDying = true;
            CanMove = false;
            _playerAnimator.Die();
        }

        public void OnSwingEvent()
        {
            AudioSource.PlayClipAtPoint(_customAudio.WeaponSwing, transform.position);
        }

        public void OnAttackStartEvent()
        {
            _weapon.StartDamaging();
        }

        public void OnAttackEndEvent()
        {
            _weapon.StopDamaging();
        }

        public void OnStepEvent(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                AudioSource.PlayClipAtPoint(_customAudio.GetRandomStep(), transform.position);
        }
    }
}
