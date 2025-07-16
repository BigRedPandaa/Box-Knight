using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 movementInput;
    public float moveSpeed = 1f;

    public LineupManager lineupManager;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float dropCooldown = 0.1f; // Interval in seconds
    private float dropTimer = 0.1f;

    public GameObject testFollower;

    private void Awake()
    {
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
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            lineupManager.addFollower(testFollower);
        }

        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        UpdateAnimation(move);

        if (lineupManager.followers.Count > 0)
        {
            if (movementInput.x != 0 || movementInput.y != 0)
            {
                dropTimer += Time.deltaTime;

                if (dropTimer >= dropCooldown)
                {
                    Vector3 lastDropPos = Vector3.positiveInfinity;

                    // Check if there are any leader positions dropped
                    if (lineupManager.leaderPositions.Count > 0)
                    {
                        // Get the position of the last dropped leader position
                        lastDropPos = lineupManager.leaderPositions[lineupManager.leaderPositions.Count - 1].transform.position;
                    }

                    // Only drop if player is 0.2f or more away from last drop
                    if (Vector3.Distance(transform.position, lastDropPos) >= 0.2f)
                    {
                        lineupManager.dropLeaderPosition();
                        dropTimer = 0f;
                    }
                }
            }
            else
            {
                // Reset drop timer if not moving
                dropTimer = dropCooldown;
            }
        }
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
