using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    [Header("Custom Settings(Optional)")]
    [Tooltip("")]

    public string customName;

    public void Interact()
    {
        if(dialogueData == null)
        {
            Debug.LogError("DialogueData not assigned to NPC: " + gameObject.name);
            return;
        }
        if(DialogueManager.Instance != null)
        {
            string speakerName = string.IsNullOrEmpty(customName) ?
                dialogueData.npcName : customName;

            DialogueManager.Instance.StartDialogue(speakerName, dialogueData.dialogueLines);
            
            
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene!");
        }

    }

    public string GetInteractionPrompt()
    {
        
       if (dialogueData == null)
        {
            return "Press E to interact";
        }
        string name = string.IsNullOrEmpty(customName) ?
             dialogueData.npcName : customName;
        return "Press E to talk to " + name;

        

    }

   
           

}
