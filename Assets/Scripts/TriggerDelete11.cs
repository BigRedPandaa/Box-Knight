using UnityEngine;

public class TriggerDelete : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        // Example: check if it's an enemy
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit");
        }
    }
}
