using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public Color textColor = Color.white;
}