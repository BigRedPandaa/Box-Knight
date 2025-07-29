using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FollowerController : MonoBehaviour
{
    public bool showDebugLine = true;
    public Color debugLineColor = Color.green;

    public LineupManager lineupManager;
    public GameObject followerLead;

    private Animator animator;
    public float baseSpeed = 1f;
    public float stopDistanceFromLead = 0.3f;
    public float maxSpeedMultiplier = 2f;
    public float speedRampDistance = 5f; // How far to reach max speed

    private int currentTargetIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (lineupManager.leaderPositions.Count == 0) return;

        currentTargetIndex = Mathf.Clamp(currentTargetIndex, 0, lineupManager.leaderPositions.Count - 1);
        GameObject targetObj = lineupManager.leaderPositions[currentTargetIndex];
        if (targetObj == null) return;

        Vector3 targetPos = targetObj.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPos);

        // ✅ Dynamic speed based on distance
        float speedMultiplier = Mathf.Clamp01(distanceToTarget / speedRampDistance);
        float moveSpeed = Mathf.Lerp(baseSpeed, baseSpeed * maxSpeedMultiplier, speedMultiplier);

        // ✅ Never fully stop unless too close to leader directly
        if (followerLead != null)
        {
            float distanceToLead = Vector3.Distance(transform.position, followerLead.transform.position);
            if (distanceToLead < stopDistanceFromLead && distanceToTarget < stopDistanceFromLead)
            {
                UpdateAnimation(Vector3.zero);
                return;
            }
        }

        Vector3 moveDir = (targetPos - transform.position).normalized;
        UpdateAnimation(moveDir);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        float threshold = 0.1f;

        if (distanceToTarget < threshold)
        {
            if (IsLastFollower())
            {
                lineupManager.DeleteLeaderPositionAt(currentTargetIndex);
            }
            else
            {
                currentTargetIndex++;
            }
        }
        else if (IsLastFollower() && currentTargetIndex < lineupManager.leaderPositions.Count - 1)
        {
            // If you're getting farther from the target, skip it
            Vector3 nextTargetPos = lineupManager.leaderPositions[currentTargetIndex].transform.position;
            Vector3 directionToTarget = (nextTargetPos - transform.position).normalized;
            Vector3 velocity = (transform.position - transform.position) / Time.deltaTime; // This is always 0, so instead...

            Vector3 futurePos = transform.position + directionToTarget * baseSpeed * Time.deltaTime;
            float futureDist = Vector3.Distance(futurePos, nextTargetPos);

            if (futureDist > distanceToTarget + 0.1f)
            {
                // Getting further → skip this point
                lineupManager.DeleteLeaderPositionAt(currentTargetIndex);
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

        Vector3 direction = Mathf.Abs(input.x) > Mathf.Abs(input.z)
            ? new Vector3(Mathf.Sign(input.x), 0, 0)
            : new Vector3(0, 0, Mathf.Sign(input.z));

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.z);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying || !showDebugLine) return;
        if (lineupManager == null || lineupManager.leaderPositions.Count == 0) return;

        int targetIndex = Mathf.Clamp(currentTargetIndex, 0, lineupManager.leaderPositions.Count - 1);
        GameObject target = lineupManager.leaderPositions[targetIndex];
        if (target == null) return;

        Handles.color = debugLineColor;
        Handles.DrawLine(transform.position + Vector3.up * 0.2f, target.transform.position + Vector3.up * 0.2f);
#endif
    }

}

