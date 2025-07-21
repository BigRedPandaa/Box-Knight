using System.Collections.Generic;
using UnityEngine;

public class LineupManager : MonoBehaviour
{
    public List<GameObject> followers;

    [SerializeField] PlayerController leaderControls;
    public GameObject leaderObject;
    public List<GameObject> leaderPositions;

    public float followerSpeed;
    [SerializeField] float followerDistance;



    private void Start()
    {
        if (leaderControls != null)
        {
            leaderObject = leaderControls.gameObject;
        }
        else if (leaderObject != null)
        {
            leaderControls = leaderObject.GetComponent<PlayerController>();

            if (leaderControls == null)
            {
                Debug.LogWarning("new leader had no controller added bespoke one");
                leaderObject.AddComponent<PlayerController>();
            }
        }

        followerSpeed = leaderControls.moveSpeed;
        followerDistance = 1f;
    }

    public void dropLeaderPosition()
    {
        GameObject leaderPosition = new GameObject("LeaderPositon");
        Vector3 dropPostion = leaderObject.transform.position;

        leaderPosition.transform.position = dropPostion;
        leaderPositions.Add(leaderPosition);
    }

    public void addFollower(GameObject follower)
    {
        // Add the new follower to the list
        followers.Add(follower);

        // Set the lineupManager if not already set
        FollowerController controller = follower.GetComponent<FollowerController>();
        if (controller.lineupManager == null)
        {
            controller.lineupManager = this;
        }

        // If this is not the first follower, set the previous follower as the leader
        if (followers.Count > 1)
        {
            GameObject previousFollower = followers[followers.Count - 2];
            controller.followerLead = previousFollower.gameObject;
        }
        else
        {
            controller.followerLead = leaderObject; // Or set it to the leader/player if needed
        }
    }

    public void DeleteLeaderPositionAt(int index)
    {
        if (index >= 0 && index < leaderPositions.Count)
        {
            GameObject toDelete = leaderPositions[index];
            leaderPositions.RemoveAt(index);
            Destroy(toDelete);
        }
    }

}
