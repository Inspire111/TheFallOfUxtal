using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    [Header("UI Elements")]
    public GameObject speechBubblePanel; // Speech bubble styled panel in HUD
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;

    private Queue<string> sentences;
    private System.Action onDialogueEnd;

    [HideInInspector] public bool isDialogueActive = false; // Public readonly control for blocking movement

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        sentences = new Queue<string>();
        speechBubblePanel.SetActive(false); // Hide at start
        continueButton.onClick.AddListener(DisplayNextSentence);
    }

    /// <summary>
    /// Starts dialogue and takes a callback on end if needed.
    /// </summary>
    public void StartDialogue(NPCDialogue npcDialogue, System.Action onEnd = null)
    {
        Debug.Log("Starting dialogue with: " + npcDialogue.npcName);

        isDialogueActive = true;
        onDialogueEnd = onEnd;

        speechBubblePanel.SetActive(true);
        npcNameText.text = npcDialogue.npcName;

        sentences.Clear();
        foreach (string sentence in npcDialogue.dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    /// <summary>
    /// Goes to the next sentence or ends dialogue.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log("Dialogue line: " + sentence);
        dialogueText.text = sentence;
    }

    /// <summary>
    /// Ends the current dialogue and re-enables movement.
    /// </summary>
    private void EndDialogue()
    {
        Debug.Log("Ending dialogue.");
        speechBubblePanel.SetActive(false);
        isDialogueActive = false;

        onDialogueEnd?.Invoke(); // Call any custom function on end
    }
}
