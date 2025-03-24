using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using JetBrains.Annotations;
using UnityEditor.Build.Content;
using System;

public class QuestManager : MonoBehaviour
{
  public static QuestManager Instance { get; private set; }

    public List<QuestStatus> playerQuests = new List<QuestStatus>();

    public UnityEvent<QuestStatus> onQuestStarted;
    public UnityEvent<QuestStatus> onQuestUpdated;
    public UnityEvent<QuestStatus> onQuestCompleted;

    private void Awake()
        {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (onQuestStarted == null)
            onQuestStarted = new UnityEvent<QuestStatus>();

        if (onQuestUpdated == null)
            onQuestUpdated = new UnityEvent<QuestStatus>();

        if (onQuestCompleted == null)
            onQuestCompleted = new UnityEvent<QuestStatus>();


    }

    public bool HasQuest(QuestData quest) 
        {
        return GetQuestStatus(quest) != null;
    }

    public QuestStatus GetQuestStatus(QuestData quest) 
        {
        return playerQuests.Find(q => q.quest == quest);
    }

    public void StartQuests(QuestData quest) 
    {
        if (HasQuest(quest)) 
         {
            return;
        }

        QuestStatus newQuest = new QuestStatus(quest);
        newQuest.state = QuestState.Active;
        playerQuests.Add(newQuest);

        onQuestStarted?.Invoke(newQuest);

        Debug.Log("Started quest: " + quest.questName);
    }


    public void UpdateQuestProgress(QuestData quest, int amount) 
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        if (questStatus == null || questStatus.state != QuestState.Active)
            return;
            
        questStatus.currentAmount += amount;
        Debug.Log($"Quest {quest.questName} progress: {questStatus.currentAmount}/{quest.requiredAmount}");

        onQuestUpdated?.Invoke(questStatus);

        if (questStatus.IsCompleted()) 
        {
            CompleteQuest(quest);
        }
    }



    public void EnemyKilled() 
    {
        foreach (QuestStatus quest in playerQuests) 
        {
            if (quest.state == QuestState.Active && quest.quest.questType == QuestType.KillEnemies) 
            {
                UpdateQuestProgress(quest.quest, 1);
            }
        }
    }


    public void ItemCollected(Item item)
    {
        foreach(QuestStatus quest in playerQuests)
        {
            if(quest.state == QuestState.Active &&
                quest.quest.questType == QuestType.CollecItems &&
                quest.quest.requiredItem == item)
            {
                UpdateQuestProgress(quest.quest, 1);
            }

        }

    }




    private void CompleteQuest(QuestData quest) 
    {
        QuestStatus questStatus = GetQuestStatus(quest);

        if (questStatus == null)
            return;

        questStatus.state = QuestState.Complete;

        onQuestCompleted?.Invoke(questStatus);

        Debug.Log("Completed Quest: " + quest.questName);
    }



    public void GiveQuestRewards(QuestData quest) 
    {
        QuestStatus questStatus = GetQuestStatus(quest);

            if (questStatus == null || questStatus.state != QuestState.Complete)
            return;

        if (quest.rewardItems != null && quest.rewardItems.Length > 0) 
        {
            foreach (Item item in quest.rewardItems) 
            {
                if (item != null && InventoryManager.Instance != null) 
                {
                    InventoryManager.Instance.AddItem(item);
                }
            }    
        }

        if (quest.experienceReward > 0) 
        {
            Debug.Log("Received" + quest.experienceReward + "experience");
            
        }

        //if (quest.isMainQuest && GameManager.Instance != null) 
        //{
        //    GameManager.Instance.TriggerVictory();
        //}

        questStatus.state = QuestState.Rewarded;

        Debug.Log("Received rewards for quest: " + quest.questName);
        

    
    }

    public List<QuestStatus> GetActiveQuests()
    {
        return playerQuests;
    }

    public void SaveQuestData()
    {
        List<QuestSaveData> saveDataList = new List<QuestSaveData>();

        foreach(QuestStatus questStatus in playerQuests)
        {
            if(questStatus.quest)
            {
                QuestSaveData saveData = new QuestSaveData
                {
                    questName = questStatus.quest.name, state = questStatus.state, currentAmount = questStatus.currentAmount
                };
                saveDataList.Add(saveData);
            }
        }

        QuestSaveList saveList = new QuestSaveList { quests = saveDataList.ToArray() };
        string json = JsonUtility.ToJson(saveList);

        PlayerPrefs.SetString("QuestData", json);
        PlayerPrefs.Save();

        Debug.Log($"saved Quests: {saveDataList.Count} quests");
    }

    public void LoadQuestData()
    {
        if(!PlayerPrefs.HasKey("QuestData"))
        {
            Debug.Log("No quest data found");
            return;
        }

        string json = PlayerPrefs.GetString("QuestData");
        QuestSaveList saveList = JsonUtility.FromJson<QuestSaveList>(json);

        if(saveList == null || saveList.quests == null)
        {
            Debug.LogWarning("Failed to load quest data");
            return;
        }

        playerQuests.Clear();

        foreach(QuestSaveData saveData in saveList.quests)
        {
            QuestData questData = Resources.Load<QuestData>("Quests/" + saveData.questName);

            if(questData)
            {
                QuestStatus questStatus = new QuestStatus(questData);
                questStatus.state = saveData.state;
                questStatus.currentAmount = saveData.currentAmount;

                playerQuests.Add(questStatus);

                Debug.Log($"Quests Loaded: {questData.questName}, State: {questStatus.state}, " +
                    $"Progression: {questStatus.currentAmount}/{questData.requiredAmount}");     
            }
            else
            {
                Debug.LogWarning($"Can't find the following quest: {saveData.questName}");
            }
        }

        NotifyQuestsLoaded();
    }

    private void NotifyQuestsLoaded()
    {
        foreach(QuestStatus quest in playerQuests)
        {
            switch(quest.state)
            {
                case QuestState.Active:
                    onQuestStarted?.Invoke(quest);
                    break;
                case QuestState.Complete:
                    onQuestCompleted?.Invoke(quest);
                    break;
            }
        }
    }


    public void LocationVisited(string locationName)
    {
        foreach(QuestStatus quest in playerQuests)
        {
            if(quest.state == QuestState.Active &&
                quest.quest.questType == QuestType.VisitLocation &&
                quest.quest.locationName == locationName)
            {
                UpdateQuestProgress(quest.quest, quest.quest.requiredAmount);

                Debug.Log($"Visiting quest completed :{quest.quest.questName}, at the following location: {locationName}");
            }
        }
    }

}






