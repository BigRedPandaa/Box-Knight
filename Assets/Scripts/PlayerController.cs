using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 movementInput;
    public float moveSpeed = 1f;
    
    private FacingDirection4 facingDirection4 = FacingDirection4.Down;
    public FacingDirection4 FacingDirection => facingDirection4;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private float dropCooldown = 0.1f;
    private float dropTimer = 0.1f;

    public GameObject testFollower;

    // 🔥 Add this reference to the character's action script
    private ICharacterActions characterActions;

    public bool IsSwinging { get; private set; }

    public void SetSwinging(bool swinging)
    {
        IsSwinging = swinging;
    }
    
    
    
    public enum FacingDirection4
    {
        Up,
        Down,
        Left,
        Right
    }
    
    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => movementInput = Vector2.zero;
        inputActions.Player.Action_1.performed += OnAction1;
        inputActions.Player.Action_2.performed += OnAction2;

        // 🔥 Try get the actions interface (if this character has one)
        characterActions = GetComponent<ICharacterActions>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (IsSwinging) return; // Block movement while swinging
        
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        if (movementInput.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
            {
                facingDirection4 = movementInput.x > 0 ? FacingDirection4.Right : FacingDirection4.Left;
            }
            else
            {
                facingDirection4 = movementInput.y > 0 ? FacingDirection4.Up : FacingDirection4.Down;
            }
        }
        
        UpdateAnimation(move);
    }

    public void UpdateAnimation(Vector3 input)
    {
        bool isMoving = input.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        if (!isMoving)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.z);
            return;
        }

        Vector3 direction;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.z))
        {
            direction = new Vector3(Mathf.Sign(input.x), 0, 0);
        }
        else
        {
            direction = new Vector3(0, 0, Mathf.Sign(input.z));
        }

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.z);
    }

    private void OnAction1(InputAction.CallbackContext context)
    {
        if (context.performed && characterActions != null)
        {
            characterActions.PerformActionOne();
        }
    }

    private void OnAction2(InputAction.CallbackContext context)
    {
        if (context.performed && characterActions != null)
        {
            characterActions.PerformActionTwo();
        }
    }
}
