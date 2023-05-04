using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IMoveable, IMortal
{
    public event Action onHide;
    public event Action onExitHideout;
    public event Action onDeath;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float crouchingSpeedFactor = 0.3f;
    [SerializeField, Min(0f)] private float itemPickupRadius = 5f;
    [SerializeField, Min(0f)] private float soundRadius = 6f;
    [Inject(Id = CustomLayer.Floor)] private LayerMask floorMask;
    [Inject(Id = CustomLayer.Interactable)] private LayerMask interactableMask;
    [Inject(Id = CustomLayer.Enemy)] private LayerMask enemyMask;
    private PlayerAnimator playerAnimator;
    private CharacterController characterController;
    private PlayerInput input;
    private Health health;
    private Weapon weapon;
    private Vector3 moveDirection;
    private float verticalAcceleration = 0f;
    private bool isDying = false;
    private bool isCrouching = false;
    public Hideout currentHideout { get; private set; }
    public bool canMove { get; set; } = true;

    [Inject] public Inventory inventory { get; private set; }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        playerAnimator = new PlayerAnimator(transform);
        weapon = GetComponentInChildren<Weapon>();

        health.onDeath += Die;

        inventory.owner = transform;

        input = GetComponent<PlayerInput>();

        input.actions["Fire"].performed += OnAttack;
        input.actions["UseItem"].performed += OnUseItem;
        input.actions["Interact"].performed += OnInteract;
        input.actions["SelectItem"].performed += OnSelectItem;
        input.actions["SwitchItem"].performed += OnSwitchItem;
        input.actions["Crouch"].performed += OnCrouchPerformed;
        input.actions["Crouch"].canceled += OnCrouchCanceled;
    }

    private void OnAttack(InputAction.CallbackContext obj)
    {
        if (canMove && !isDying)
            Attack();
    }

    private void OnUseItem(InputAction.CallbackContext obj)
    {
        inventory.UseItem();
    }

    private void OnExitHideout(InputAction.CallbackContext obj)
    {
        ExitHideout();
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        Interact();
    }

    private void OnSelectItem(InputAction.CallbackContext obj)
    {
        int value = (int)obj.ReadValue<float>();

        if (value != 0)            
            inventory.selectedSlot = value - 1;            
    }

    private void OnSwitchItem(InputAction.CallbackContext obj)
    {
        int value = (int)obj.ReadValue<float>();

        if (value > 0)
        {
            inventory.selectedSlot = (inventory.selectedSlot + 1) % inventory.size;
        }
        else
        {
            inventory.selectedSlot = (inventory.size + inventory.selectedSlot - 1) % inventory.size;
        }
    }

    private void OnCrouchPerformed(InputAction.CallbackContext obj)
    {
        isCrouching = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext obj)
    {
        isCrouching = false;
    }

    public void ExitHideout()
    {
        input.actions["Interact"].performed -= OnExitHideout;
        input.actions["Fire"].performed += OnAttack;
        input.actions["Interact"].performed += OnInteract;

        transform.position += currentHideout.transform.forward;
        currentHideout = null;
        characterController.enabled = true;

        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(true);
        }

        onExitHideout?.Invoke();
    }

    void Update()
    {
        if (currentHideout == null)
        {
            CalculateGravity();
            
            if (canMove && !isDying)
            {
                Move();
                Turn();
            }
        }
    }

    private void Move()
    {
        var inputValue = input.actions["Move"].ReadValue<Vector2>();
        moveDirection = new Vector3(inputValue.x, 0f, inputValue.y);

        var movement = moveDirection * speed * Time.deltaTime;
        
        if (isCrouching)
        {
            movement *= crouchingSpeedFactor;
            moveDirection *= crouchingSpeedFactor;
        }
        else if (movement != Vector3.zero)
        {
            SoundUtil.SpawnSound(transform.position, soundRadius);
        }

        characterController.Move(movement);
        
        playerAnimator.AnimateMovement(moveDirection);
    }

    private void Turn()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, floorMask.value))
        {
            Vector3 hitPoint = floorHit.point;
            hitPoint.y = transform.position.y;
            transform.LookAt(hitPoint);
        }
    }

    private void Attack()
    {
        if (!UserRaycaster.IsBlockedByUI())
        {
            playerAnimator.Attack();
        }
    }    
    
    public void PickItem(ItemPickable pickable)
    {        
        inventory.Add(pickable.GetItem());
        pickable.DestroySelf();                
    }
    
    public void Hide(Hideout hideout)
    {
        input.actions["Fire"].performed -= OnAttack;
        input.actions["Interact"].performed -= OnInteract;
        input.actions["Interact"].performed += OnExitHideout;

        currentHideout = hideout;
        characterController.enabled = false;

        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(false);
        }

        transform.position = currentHideout.transform.position;

        onHide?.Invoke();
    }

    private void Interact()
    {
        var colliders = Physics.OverlapSphere(transform.position, itemPickupRadius, interactableMask.value);

        if (colliders.Length == 0)
            return;
        
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(camRay, out var hit, Mathf.Infinity, interactableMask.value))
            return;

        foreach (var col in colliders)
        {
            if (col == hit.collider)
            {
                if (!Physics.Linecast(transform.position + Vector3.up, hit.point, ~interactableMask))
                {
                    hit.transform.GetComponent<IInteractable>().Interact(this);
                }

                break;
            }
        }        
    }

    private void CalculateGravity()
    {
        if (characterController.isGrounded)
        {
            verticalAcceleration = 0f;
        }
        else
        {
            verticalAcceleration -= 9.81f * Time.deltaTime * Time.deltaTime;
            characterController.Move(new Vector3(0f, verticalAcceleration, 0f));
        }
    }

    public void OnAttackStartEvent()
    {
        weapon.StartDamaging();
    }

    public void OnAttackEndEvent()
    {
        weapon.StopDamaging();
    }

    void Die()
    {
        gameObject.layer = 0;
        isDying = true;
        canMove = false;
        playerAnimator.Die();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, moveDirection);
        Gizmos.DrawWireSphere(transform.position, itemPickupRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }

    public void OnDeath()
    {   
        onDeath?.Invoke();

        input.actions["Fire"].performed -= OnAttack;
        input.actions["UseItem"].performed -= OnUseItem;
        input.actions["Interact"].performed -= OnInteract;
        input.actions["SelectItem"].performed -= OnSelectItem;
        input.actions["SwitchItem"].performed -= OnSwitchItem;
        input.actions["Crouch"].performed -= OnCrouchPerformed;
        input.actions["Crouch"].canceled -= OnCrouchCanceled;

        enabled = false;
    }
}
