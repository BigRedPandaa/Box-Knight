using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineupManager : MonoBehaviour
{
    public List<GameObject> followers;

    [SerializeField] PlayerController leaderControls;
    public GameObject leaderObject;
    public List<GameObject> leaderPositions;

    public float followerSpeed;
    [SerializeField] float followerDistance;


    private void Update()
    {
        CheckNumberKeys();
    }

    private void CheckNumberKeys()
    {
        for (int i = 1; i <= followers.Count; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                SwapLeader(i);
            }
        }
    }

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

    public void AddFollower(GameObject follower, int index = -1)
    {
        // Clamp index to valid range
        if (index < 0 || index > followers.Count)
        {
            index = followers.Count; // Add to end
        }

        followers.Insert(index, follower);

        FollowerController controller = follower.GetComponent<FollowerController>();
        if (controller.lineupManager == null)
        {
            controller.lineupManager = this;
        }

        // Set followerLead of the new follower
        if (index == 0)
        {
            controller.followerLead = leaderObject;
        }
        else
        {
            controller.followerLead = followers[index - 1];
        }

        // Update followerLead for all followers after the inserted one
        for (int i = index + 1; i < followers.Count; i++)
        {
            FollowerController nextController = followers[i].GetComponent<FollowerController>();
            nextController.followerLead = followers[i - 1];
        }
    }

    public void RemoveFollower(int index)
    {
        // Validate index
        if (index < 0 || index >= followers.Count)
        {
            Debug.LogWarning("RemoveFollower: Index out of range");
            return;
        }

        GameObject followerToRemove = followers[index];

        // Remove the follower from the list
        followers.RemoveAt(index);

        // Optionally destroy the GameObject
        // Destroy(followerToRemove);

        // Update the followerLead for the follower who came after the removed one
        for (int i = index; i < followers.Count; i++)
        {
            FollowerController controller = followers[i].GetComponent<FollowerController>();
            if (i == 0)
            {
                controller.followerLead = leaderObject;
            }
            else
            {
                controller.followerLead = followers[i - 1];
            }
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

    private void SwapLeader(int charIndex)
    {
        PlayerController leaderController = leaderObject.GetComponent<PlayerController>();
        PlayerInput leaderInputs = leaderObject.GetComponent<PlayerInput>();
        FollowerController leaderFollowerBrain = leaderObject.GetComponent<FollowerController>();
        Vector3 leaderPos = leaderObject.transform.position;

        GameObject FollowerObject = followers[charIndex - 1].gameObject;
        PlayerController FollowerController = followers[charIndex - 1].GetComponent<PlayerController>();
        PlayerInput FollowerInputs = followers[charIndex - 1].GetComponent<PlayerInput>();
        FollowerController thisFollowerBrain = followers[charIndex - 1].GetComponent<FollowerController>();
        Vector3 followerPos = followers[charIndex - 1].transform.position;

        Camera mainCamera;
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log("Main camera found: " + mainCamera.name);
        }
        else
        {
            Debug.LogWarning("No main camera found!");
        }

        RemoveFollower(charIndex - 1);
        AddFollower(leaderObject, charIndex - 1);

        // Set followerLead of old leader (now a follower)
        FollowerController oldLeaderFollower = leaderObject.GetComponent<FollowerController>();
        if (charIndex - 1 == 0)
        {
            oldLeaderFollower.followerLead = FollowerObject;
        }
        else
        {
            oldLeaderFollower.followerLead = followers[charIndex - 2];
        }


        leaderController.enabled = false;
        leaderInputs.enabled = false;
        leaderFollowerBrain.enabled = true;

        FollowerController.enabled = true;
        FollowerInputs.enabled = true;
        thisFollowerBrain.enabled = false;

        leaderObject.transform.position = followerPos;
        FollowerObject.transform.position = leaderPos;

        CameraController cameraControls = mainCamera.GetComponent<CameraController>();
        cameraControls.SetTarget(FollowerObject.transform);

        leaderControls = FollowerObject.GetComponent<PlayerController>();
        leaderObject = FollowerObject;
    }

    public void SpawnFollowerPositionAt(Vector3 position)
    {
        GameObject followerPosition = new GameObject("followerPosition");
        GameObject newPos = Instantiate(followerPosition, position, Quaternion.identity);
        leaderPositions.Add(newPos);
    }
}