using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogueInteractor : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask interactableLayer;
    public DialogueManager dialogueManager;

    private InputSystem_Actions input;
    private DialogueInteractable currentTarget;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        FindClosestInteractable();
    }

    void FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        currentTarget = null;

        float closest = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closest)
            {
                var interactable = hit.GetComponent<DialogueInteractable>();
                if (interactable)
                {
                    currentTarget = interactable;
                    closest = dist;
                }
            }
        }
    }

    void TryInteract()
    {
        if (currentTarget != null && !dialogueManager.IsDialoguePlaying)
        {
            dialogueManager.StartStory(currentTarget.inkJSON, currentTarget.characterData);
        }
    }
}