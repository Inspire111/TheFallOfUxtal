using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [Header("Dialogue Data")]
    public string npcName;
    [TextArea(3, 10)] public List<string> firstInteraction;
    [TextArea(3, 10)] public List<string> repeatInteraction;

    private bool hasTalked = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !DialogueSystem.instance.isDialogueActive)
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
