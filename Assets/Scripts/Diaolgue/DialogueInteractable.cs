using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueInteractable : MonoBehaviour
{
    public TextAsset inkJSON; // Ink file specific to this character
    public DialogueCharacter characterData; // ScriptableObject with portrait, name, color
}