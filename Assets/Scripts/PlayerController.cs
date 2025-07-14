using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 movementInput;
    public float moveSpeed = 1f;

    public FollowLineManager followLineManager;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        followLineManager.SetNewLeader(this.gameObject.transform);
        inputActions = new InputSystem_Actions();

        // Bind movement input
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => movementInput = Vector2.zero;
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
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        UpdateAnimation(move);
    }

    public void UpdateAnimation(Vector3 input)
    {
        bool isMoving = input.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        if (!isMoving)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.z); // 🔁 FIXED: use Z for forward/back
            return;
        }

        Vector3 direction;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.z))
        {
            direction = new Vector3(Mathf.Sign(input.x), 0, 0);
        }
        else
        {
            direction = new Vector3(0, 0, Mathf.Sign(input.z)); // 🔁 FIXED
        }

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.z); // 🔁 FIXED
    }

}
