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
    [SerializeField] private float itemPickupRadius = 5f;
    [Inject(Id = CustomLayer.Floor)] private LayerMask floorMask;
    [Inject(Id = CustomLayer.Interactable)] private LayerMask interactableMask;
    private PlayerAnimator playerAnimator;
    private CharacterController characterController;
    private PlayerInput input;
    private Health health;
    private Weapon weapon;
    private Vector3 moveDirection;
    private float verticalAcceleration = 0f;
    private bool isDying = false;
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
        input.actions["UseItem"].performed += obj => inventory.UseItem();
        input.actions["Interact"].performed += OnInteract;

        input.actions["SelectItem"].performed += obj => {
            int value = (int)obj.ReadValue<float>();

            if (value != 0)            
                inventory.selectedSlot = value - 1;
            
        };
        input.actions["SwitchItem"].performed += obj => {
            int value = (int)obj.ReadValue<float>();

            if (value > 0)
            {
                inventory.selectedSlot = (inventory.selectedSlot + 1) % inventory.size;
            }
            else
            {
                inventory.selectedSlot = (inventory.size + inventory.selectedSlot - 1) % inventory.size;
            }
        };
    }

    private void OnAttack(InputAction.CallbackContext obj)
    {
        if (canMove && !isDying)
            Attack();
    }

    private void OnExitHideout(InputAction.CallbackContext obj)
    {
        ExitHideout();
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        Interact();
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
        var movement = input.actions["Move"].ReadValue<Vector2>();
        moveDirection = new Vector3(movement.x, 0f, movement.y);

        characterController.Move(moveDirection * speed * Time.deltaTime);
        
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
        if (Physics.OverlapSphere(transform.position, itemPickupRadius, interactableMask.value).Length != 0)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(camRay, out hit, Mathf.Infinity, interactableMask.value))
            {
                if (!Physics.Linecast(transform.position + Vector3.up, hit.point, ~interactableMask))
                {
                    hit.transform.GetComponent<IInteractable>().Interact(this);
                }
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
    }

    public void OnDeath()
    {   
        onDeath?.Invoke();

        input.actions.Disable();
        enabled = false;
    }
}
