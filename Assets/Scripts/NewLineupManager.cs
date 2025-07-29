using System.Collections.Generic;
using UnityEngine;

public class NewLineupManager : MonoBehaviour
{

    [Header("Leader & Followers")]
    public Transform leader;
    public List<Transform> followers;

    [Header("Spacing")]
    public float leaderSpacing = 1.5f;
    public float followerSpacing = 1.0f;

    [Header("Movement")]
    public float smoothing = 10f;

    [Header("Camera")]
    public Camera mainCamera;
    public CameraController cameraFollowScript; // Your camera follow script that tracks leader

    private List<Vector3> positionHistory = new List<Vector3>();
    private Vector3[] followerVelocities;
    private Animator[] animators;

    void Start()
    {
        int count = followers.Count;
        followerVelocities = new Vector3[count];
        animators = new Animator[count];

        for (int i = 0; i < count; i++)
        {
            animators[i] = followers[i].GetComponent<Animator>();
        }

        SetupPlayerController();
    }

    void Update()
    {
        HandleShuffleInput();

        if (positionHistory.Count == 0 || Vector3.Distance(positionHistory[0], leader.position) > 0.05f)
        {
            positionHistory.Insert(0, leader.position);
        }

        for (int i = 0; i < followers.Count; i++)
        {
            float spacingMultiplier = (i == 0) ? leaderSpacing : leaderSpacing + i * followerSpacing;
            int index = Mathf.Min((int)(spacingMultiplier * 10f), positionHistory.Count - 1);
            Vector3 targetPos = positionHistory[index];
            Transform follower = followers[i];

            Vector3 currentPos = follower.position;
            Vector3 directionToTarget = targetPos - currentPos;

            Vector3 newPos = Vector3.SmoothDamp(currentPos, targetPos, ref followerVelocities[i], 1f / smoothing);
            follower.position = newPos;

            UpdateAnimation(animators[i], directionToTarget);
        }

        int maxHistory = (int)((leaderSpacing + followers.Count * followerSpacing) * 10f);
        if (positionHistory.Count > maxHistory)
        {
            positionHistory.RemoveRange(maxHistory, positionHistory.Count - maxHistory);
        }
    }

    private void HandleShuffleInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShuffleLineupForward();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            ShuffleLineupBackward();
        }
    }

    private void ShuffleLineupForward()
    {
        if (followers.Count == 0) return;

        Transform oldLeader = leader;
        Transform firstFollower = followers[0];

        Vector3 oldLeaderPos = oldLeader.position;
        Vector3 firstFollowerPos = firstFollower.position;

        // Swap positions
        oldLeader.position = firstFollowerPos;
        firstFollower.position = oldLeaderPos;

        // Update followers and leader ref
        followers.RemoveAt(0);
        followers.Add(oldLeader);
        leader = firstFollower;

        RefreshAnimatorsAndControllers();

        Debug.Log($"Shuffled forward! New Leader: {leader.name}");
    }

    private void ShuffleLineupBackward()
    {
        if (followers.Count == 0) return;

        Transform oldLeader = leader;
        Transform lastFollower = followers[followers.Count - 1];

        Vector3 oldLeaderPos = oldLeader.position;
        Vector3 lastFollowerPos = lastFollower.position;

        // Swap positions
        oldLeader.position = lastFollowerPos;
        lastFollower.position = oldLeaderPos;

        // Update followers and leader ref
        followers.RemoveAt(followers.Count - 1);
        followers.Insert(0, oldLeader);
        leader = lastFollower;

        RefreshAnimatorsAndControllers();

        Debug.Log($"Shuffled backward! New Leader: {leader.name}");
    }

    private void RefreshAnimatorsAndControllers()
    {
        // Update animators array
        animators = new Animator[followers.Count];
        for (int i = 0; i < followers.Count; i++)
        {
            animators[i] = followers[i].GetComponent<Animator>();
        }

        SetupPlayerController();

        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetTarget(leader);
        }
    }

    private void SetupPlayerController()
    {
        // Toggle PlayerController on new leader, off old followers
        // Enable PlayerController on leader
        var leaderController = leader.GetComponent<PlayerController>();
        if (leaderController != null) leaderController.enabled = true;

        // Disable PlayerController on all followers
        foreach (var follower in followers)
        {
            var controller = follower.GetComponent<PlayerController>();
            if (controller != null) controller.enabled = false;
        }
    }

    private void UpdateAnimation(Animator animator, Vector3 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        if (!isMoving) return;

        Vector3 cleanDir;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            cleanDir = new Vector3(Mathf.Sign(direction.x), 0, 0);
        else
            cleanDir = new Vector3(0, 0, Mathf.Sign(direction.z));

        animator.SetFloat("MoveX", cleanDir.x);
        animator.SetFloat("MoveY", cleanDir.z);
    }



}
