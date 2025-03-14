using UnityEngine;

public class QuestNPC: NPC
{
    [Header("Quest Settings")]
    public QuestGiver questGiver;

    public override void Interact()
    {
        if(DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        if(questGiver == null || questGiver.quest == null)
        {
            base.Interact();
            return;
        }

        string[] dialogue = questGiver.GetCurrentDialogue();

        if(dialogue != null && dialogue.Length >0)
        {
            string speakerName = string.IsNullOrEmpty(customName) ? dialogueData.npcName : customName;
            DialogueManager.Instance.StartDialogue(speakerName, dialogue);
            ConfigureDialogueEndActions();

        }


        
    }


    private void ConfigureDialogueEndActions()
    {
        QuestState state = questGiver.GetQuestState();

        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        if(state == QuestState.NotStarted)
        {
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedStartQuest);
        }
        else if(state == QuestState.Complete)
        {
            DialogueManager.Instance.onDialogueEnded.AddListener(OnDialogueEndedGiveRewards);
        }


    }

    private void OnDialogueEndedStartQuest()
    {
        QuestManager.Instance.StartQuests(questGiver.quest);

        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest started: {questGiver.quest.questName}");
    }


    private void OnDialogueEndedGiveRewards()
    {
        QuestManager.Instance.GiveQuestRewards(questGiver.quest);

        DialogueManager.Instance.onDialogueEnded.RemoveAllListeners();

        Debug.Log($"Quest rewards given for: {questGiver.quest.questName}");

    }


}





