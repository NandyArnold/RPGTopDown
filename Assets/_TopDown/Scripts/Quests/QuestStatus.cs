using UnityEngine;
[System.Serializable]
public class QuestStatus
{
    public QuestData quest;
    public QuestState state;
    public int currentAmount;

    public QuestStatus (QuestData quest)
        {
        this.quest = quest;
        state = QuestState.NotStarted;
        currentAmount = 0;
        
    }

    public bool IsCompleted() 
        {
        return currentAmount >= quest.requiredAmount;    
    }




}


// ENUM MUST BE OUTSIDE OF THE CLASS
public enum QuestState
{
    NotStarted,
    Active,
    Complete,
    Rewarded

}
