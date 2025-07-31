using System.Collections;
using UnityEngine;

public class BoxKnight : MonoBehaviour, ICharacterActions
{
    public PlayerController player;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sword Attack Settings")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float animationDuration = 0.5f; // Match your sword swing animation length
    [SerializeField] private float hitboxDistance = 1f; // How far from the player to spawn the hitbox

    private void Awake()
    {
        animator = player.animator;
        spriteRenderer = player.spriteRenderer;
    }

    public void PerformActionOne()
    {
        StartCoroutine(DoSwordSwing());
    }

    private IEnumerator DoSwordSwing()
    {
        player.SetSwinging(true); // Disable movement

        Vector3 directionOffset = Vector3.zero;
        int facingIndex = 0;

        switch (player.FacingDirection)
        {
            case PlayerController.FacingDirection4.Up:
                directionOffset = Vector3.forward;
                facingIndex = 2;
                break;
            case PlayerController.FacingDirection4.Left:
                directionOffset = Vector3.left;
                facingIndex = 1;
                break;
            case PlayerController.FacingDirection4.Down:
                directionOffset = Vector3.back;
                facingIndex = 0;
                break;
            case PlayerController.FacingDirection4.Right:
                directionOffset = Vector3.right;
                facingIndex = 3;
                break;
        }

        animator.SetInteger("FacingDirection", facingIndex);
        animator.SetTrigger("SwordSwingTrigger");

        Vector3 spawnPos = playerTransform.position + directionOffset * hitboxDistance;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(animationDuration);

        Destroy(hitbox);
        player.SetSwinging(false); // Re-enable movement
    }

    public void PerformActionTwo()
    {
        // Another action
    }
}