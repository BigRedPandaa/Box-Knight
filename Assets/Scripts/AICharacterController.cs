using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform targetToFollow; // Set this to the leader/player in FollowLineManager

    private FollowLineManager followLineManager;
    private Vector3 lastDirection;
    bool isMoving;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        followLineManager = FindObjectOfType<FollowLineManager>();

        if (followLineManager != null)
        {
            followLineManager.AddFollower(this.transform);
        }
        else
        {
            Debug.LogWarning("FollowLineManager not found.");
        }
    }

    private void Update()
    {
        if (targetToFollow == null) return;

        Vector3 direction = (targetToFollow.position - transform.position);
        direction.y = 0f;

        if (direction.magnitude > 0.3f)
        {
            Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
            lastDirection = direction.normalized;
            isMoving = true;
            UpdateAnimation(lastDirection);
        }
        else
        {
            isMoving = false;
            UpdateAnimation(lastDirection);
        }

        
    }

    private void UpdateAnimation(Vector3 input)
    {
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

    public void SetTarget(Transform newTarget)
    {
        targetToFollow = newTarget;
    }
}

