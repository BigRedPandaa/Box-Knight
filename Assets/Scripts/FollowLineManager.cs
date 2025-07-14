using UnityEngine;
using System.Collections.Generic;

public class FollowLineManager : MonoBehaviour
{
    [SerializeField] private List<Transform> followers = new List<Transform>(); // All objects including leader
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float followSpeed = 5f;

    private int leaderIndex = 0;

    void Update()
    {
        UpdateFollowerPositions();
    }

    private void UpdateFollowerPositions()
    {
        for (int i = 1; i < followers.Count; i++)
        {
            Transform current = followers[i];
            Transform target = followers[i - 1];

            Vector3 targetPosition = target.position - target.forward * followDistance;
            current.position = Vector3.Lerp(current.position, targetPosition, Time.deltaTime * followSpeed);

            // Optional: match rotation smoothly
            Quaternion targetRotation = Quaternion.LookRotation(target.position - current.position);
            current.rotation = Quaternion.Slerp(current.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }

    /// <summary>
    /// Sets a new leader at runtime. The selected object will move to the front of the list.
    /// </summary>
    /// <param name="newLeader">The Transform of the new leader object.</param>
    public void SetNewLeader(Transform newLeader)
    {
        if (!followers.Contains(newLeader))
        {
            Debug.LogWarning("New leader not in followers list.");
            return;
        }

        followers.Remove(newLeader);
        followers.Insert(0, newLeader);
        leaderIndex = 0;
    }

    public Transform GetLeader()
    {
        return followers[leaderIndex];
    }

    public List<Transform> GetFollowerList()
    {
        return followers;
    }

    /// <summary>
    /// Adds a new follower at the end of the line.
    /// </summary>
    public void AddFollower(Transform newFollower)
    {
        if (!followers.Contains(newFollower))
        {
            followers.Add(newFollower);
        }
    }

    /// <summary>
    /// Removes a follower from the list.
    /// </summary>
    public void RemoveFollower(Transform follower)
    {
        if (followers.Contains(follower))
        {
            followers.Remove(follower);
        }
    }
}
