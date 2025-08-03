using Unity.VisualScripting;
using UnityEngine;

public class SwordHitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);
        // Example: check if it's an enemy
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit");
            Destroy(other.gameObject);
        }
    }
}

