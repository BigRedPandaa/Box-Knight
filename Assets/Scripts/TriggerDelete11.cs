using UnityEngine;

public class TriggerDelete : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
    }
}
