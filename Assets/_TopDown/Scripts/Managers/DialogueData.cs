using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "RPG/DialogueData")]
public class DialogueData : ScriptableObject
{
    [Header("NPC Settings")]

    public string npcName;

    [Header("Dialogue")]

    [TextArea(3, 10)]

    public string[] dialogueLines;
}
