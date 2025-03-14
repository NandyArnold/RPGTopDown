using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestData quest;

    public QuestState GetQuestState()
    {
        if (quest == null || QuestManager.Instance == null)
            return QuestState.NotStarted;

        QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
        return status != null ? status.state : QuestState.NotStarted;

    }

    public string[] GetCurrentDialogue()
    {
        if (quest == null)
            return null;

        QuestState state = GetQuestState();

        switch(state)
        {
            case QuestState.NotStarted:
                return quest.questOfferDialogue;

            case QuestState.Active:
                QuestStatus status = QuestManager.Instance.GetQuestStatus(quest);
                if(status != null && status.IsCompleted())
                {
                    return quest.questCompletedDialogue;
                }
                return quest.questActiveDialogue;

            case QuestState.Complete:
                return quest.questCompletedDialogue;

            case QuestState.Rewarded:
                return quest.questCompletedDialogue;

            default:
                return null;

        }



    }

   




}
