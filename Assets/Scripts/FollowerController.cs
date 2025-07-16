using UnityEngine;

public class FollowerController : MonoBehaviour
{
    public LineupManager lineupManager;
    public GameObject followerLead;

    private Animator animator;
    public float moveSpeed = 1f;
    public float stopDistanceFromLead = 1.2f;

    private int currentTargetIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (lineupManager.leaderPositions.Count == 0) return;

        // ✅ Stop if close to lead
        if (followerLead != null)
        {
            float distanceToLead = Vector3.Distance(transform.position, followerLead.transform.position);
            if (distanceToLead < stopDistanceFromLead)
            {
                UpdateAnimation(Vector3.zero);
                return;
            }
        }

        // Clamp index
        currentTargetIndex = Mathf.Clamp(currentTargetIndex, 0, lineupManager.leaderPositions.Count - 1);
        GameObject targetObj = lineupManager.leaderPositions[currentTargetIndex];
        if (targetObj == null) return;

        Vector3 targetPos = targetObj.transform.position;
        Vector3 moveDir = (targetPos - transform.position).normalized;

        UpdateAnimation(moveDir);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            // ✅ Last follower deletes the position
            if (IsLastFollower())
            {
                lineupManager.DeleteLeaderPositionAt(currentTargetIndex);
                // Don't increment index — it now points to the next one
            }
            else
            {
                currentTargetIndex++;
            }
        }
    }

    private bool IsLastFollower()
    {
        return lineupManager.followers.Count > 0 &&
               lineupManager.followers[lineupManager.followers.Count - 1] == this.gameObject;
    }

    private void UpdateAnimation(Vector3 input)
    {
        if (animator == null) return;

        bool isMoving = input.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        if (!isMoving)
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            return;
        }

        Vector3 direction;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.z))
            direction = new Vector3(Mathf.Sign(input.x), 0, 0);
        else
            direction = new Vector3(0, 0, Mathf.Sign(input.z));

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.z);
    }
}
