using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialogue
{
    public string npcName;
    [TextArea(3, 10)] public List<string> dialogueLines;
}
