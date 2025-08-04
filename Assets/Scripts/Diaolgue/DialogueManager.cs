using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueUIRoot;          // Parent container of all UI
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    [Header("Typewriter")]
    public float typeSpeed = 0.03f;

    private Story story;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool canContinue = false;

    public bool IsDialoguePlaying { get; private set; } = false;

    private InputSystem_Actions input;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.UI.Submit.performed += ctx => OnContinuePressed();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Start()
    {
        HideDialogueUI();
    }

    public void StartStory(TextAsset inkAsset, DialogueCharacter character)
    {
        if (inkAsset == null || character == null) return;

        story = new Story(inkAsset.text);
        ShowDialogueUI();

        nameText.text = character.characterName;
        portraitImage.sprite = character.portrait;
        dialogueText.color = character.textColor;

        IsDialoguePlaying = true;
        ContinueStory();
    }

    void OnContinuePressed()
    {
        if (!IsDialoguePlaying) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            isTyping = false;
            canContinue = true;
        }
        else if (canContinue)
        {
            ContinueStory();
        }
    }

    void ContinueStory()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            HandleTags(story.currentTags);
            typingCoroutine = StartCoroutine(TypeText(text));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;
        isTyping = true;
        canContinue = false;

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        canContinue = true;
    }

    void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            string[] split = tag.Split(':');
            if (split.Length != 2) continue;

            string key = split[0].Trim();
            string value = split[1].Trim();

            if (key == "speaker")
            {
                // You could dynamically update character portrait/color here
                nameText.text = value;
            }
        }
    }

    void ShowDialogueUI()
    {
        dialogueUIRoot.SetActive(true);
    }

    void HideDialogueUI()
    {
        dialogueUIRoot.SetActive(false);
    }

    void EndDialogue()
    {
        HideDialogueUI();
        IsDialoguePlaying = false;
    }
}
