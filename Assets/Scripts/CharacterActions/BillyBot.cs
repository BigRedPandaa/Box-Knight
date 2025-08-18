using System.Collections;
using UnityEngine;

public class BillyBot : MonoBehaviour, ICharacterActions
{
   
    public PlayerController player;

    [SerializeField] private bool ListenToActions = true;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Activation Settings")]
    [SerializeField] private GameObject objectToActivate; // The object to toggle
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float activationDistance = 1f;

    private bool isActive = false;

    private void Awake()
    {
        animator = player.animator;
        spriteRenderer = player.spriteRenderer;

        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }

    public void PerformActionOne()
    {

        isActive = !isActive; // Toggle state

        if (ListenToActions == false)
            return;

        else if (objectToActivate != null)
        {
            if (isActive)
                UpdateObjectPositionAndRotation(); // Ensure it spawns correctly

            objectToActivate.SetActive(isActive);
        }

        // Optional animation trigger
        animator.SetTrigger("FlashLightOn");
    }

    private void Update()
    {
        // While active, keep matching facing direction & position
        if (isActive && objectToActivate != null)
            UpdateObjectPositionAndRotation();
    }

    private void UpdateObjectPositionAndRotation()
    {
        Vector3 directionOffset = Vector3.zero;
        Quaternion facingRotation = Quaternion.identity;
        int facingIndex = 0;

        switch (player.FacingDirection)
        {
            case PlayerController.FacingDirection4.Up:
                directionOffset = Vector3.forward;
                facingRotation = Quaternion.Euler(0, 0, 0);
                facingIndex = 2;
                break;
            case PlayerController.FacingDirection4.Left:
                directionOffset = Vector3.left;
                facingRotation = Quaternion.Euler(0, 270, 0);
                facingIndex = 1;
                break;
            case PlayerController.FacingDirection4.Down:
                directionOffset = Vector3.back;
                facingRotation = Quaternion.Euler(0, 180, 0);
                facingIndex = 0;
                break;
            case PlayerController.FacingDirection4.Right:
                directionOffset = Vector3.right;
                facingRotation = Quaternion.Euler(0, 90, 0);
                facingIndex = 3;
                break;
        }

        animator.SetInteger("FacingDirection", facingIndex);

        objectToActivate.transform.position = playerTransform.position + directionOffset * activationDistance;
        objectToActivate.transform.rotation = facingRotation;
    }

    public void PerformActionTwo()
    {
        // Another action if needed
    }

    public void DisableActions()
    {
     objectToActivate.SetActive(false);
        ListenToActions = false;
    }
    public void EnableActions()
    {
        ListenToActions = true;
    }

}
