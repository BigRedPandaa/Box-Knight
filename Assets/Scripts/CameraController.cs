using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("The target player to follow")]
    public Transform player;

    [Tooltip("Offset from the player position")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Tooltip("Speed at which the camera catches up to the lagged position")]
    public float smoothSpeed = 5f;

    [Tooltip("How quickly the lag position catches up to the actual player")]
    public float lagSpeed = 2f;

    // Internally tracked lagged player position
    private Vector3 laggedPlayerPosition;

    void Start()
    {
        if (player != null)
        {
            laggedPlayerPosition = player.position;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Slowly update the lagged position to follow the player
        laggedPlayerPosition = Vector3.Lerp(laggedPlayerPosition, player.position, lagSpeed * Time.deltaTime);

        // Calculate desired camera position using lagged position
        Vector3 desiredPosition = laggedPlayerPosition + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Look at the lagged player position for a more natural feel
        transform.LookAt(laggedPlayerPosition);
    }
}
