using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using JetBrains.Annotations;
using UnityEditor.Build.Content;

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
        }
        else 
        {
            Instance = this;
        }

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






    

    
        


        



    







}
