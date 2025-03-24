using UnityEngine;

[System.Serializable]
public class QuestSaveData
{
    public string questName;
    public QuestState state;
    public int currentAmount;
}

[System.Serializable]
public class QuestSaveList
{
    public QuestSaveData[] quests;
}