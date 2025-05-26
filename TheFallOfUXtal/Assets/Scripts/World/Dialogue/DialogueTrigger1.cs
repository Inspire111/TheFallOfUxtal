using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [Header("Dialogue Data")]
    public string npcName;
    [TextArea(3, 10)] public List<string> firstInteraction;
    [TextArea(3, 10)] public List<string> repeatInteraction;

    private bool hasTalked = false;
    private InputSystem_Actions inputActions;
    private bool playerInRange = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Update()
    {
        if (playerInRange && inputActions.Player.Interact.WasPressedThisFrame() && !DialogueSystem.instance.isDialogueActive)
        {
            StartConversation();
        }
    }

    void StartConversation()
    {
        List<string> dialogueToUse = hasTalked ? GetRandomRepeatDialogue() : firstInteraction;
        hasTalked = true;

        NPCDialogue dialogue = new NPCDialogue
        {
            npcName = npcName,
            dialogueLines = dialogueToUse
        };

        DialogueSystem.instance.StartDialogue(dialogue);
    }

    List<string> GetRandomRepeatDialogue()
    {
        List<string> options = new List<string>(repeatInteraction);
        string randomLine = options[Random.Range(0, options.Count)];
        return new List<string> { randomLine };
    }

}
