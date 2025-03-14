using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "RPG/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    [TextArea(3, 5)]
    public string questDescription;

    [Header("Quest Requirements")]
    public QuestType questType;
    public int requiredAmount = 1;
    public Item requiredItem;

    [Header("Quest Rewards")]
    public bool isMainQuest = false;
    public Item[] rewardItems;
    public int experienceReward;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] questOfferDialogue;
    [TextArea(2,5)]
    public string[] questActiveDialogue;
    [TextArea(2, 5)]
    public string[] questCompletedDialogue;
}

public enum QuestType 
    {
    KillEnemies,
    CollecItems,
    TalkToNPC
}


