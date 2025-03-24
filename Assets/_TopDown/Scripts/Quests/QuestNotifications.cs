using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;




public class QuestNotifications : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notificationPrefab;
    public Transform notificationParent;

    [Header("Animation Settings")]
    public float displayTime = 3f;
    public float fadeInTime = 0.5F;
    public float fadeOutTime = 0.5f;
    public float riseDistance = 150F;
    public int maxNotifications = 3;

    [Header("Notification Grouping")]
    public float groupingDelay = 0.5f;


    private Queue<QuestNotificationInfo> notificationQueue = new Queue<QuestNotificationInfo>();
    private int activeNotifications = 0;

    private Dictionary<string, QuestUpdateTracker> questUpdates = new Dictionary<string, QuestUpdateTracker>();

    private class QuestNotificationInfo
    {
        public string message;
        public Color color;

        public QuestNotificationInfo(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }
    }

    private class QuestUpdateTracker
    {

        public QuestStatus status;
        public float lastUpdateTime;
        public bool isProcessing;
        public bool isCompleted;

        public QuestUpdateTracker(QuestStatus status)
        {
            this.status = status;
            this.lastUpdateTime=  Time.time;
            this.isProcessing = false;
            this.isCompleted = status.IsCompleted();
        }
    }


    private void Start()
    {
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.onQuestStarted.AddListener(OnQuestStarted);
            QuestManager.Instance.onQuestUpdated.AddListener(OnQuestUpdated);
            QuestManager.Instance.onQuestCompleted.AddListener(OnQuestCompleted);
        }
    }


    private void Update()
    {
        CheckPendingUpdates();
    }


    private void OnDestroy()
    {
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.onQuestStarted.RemoveListener(OnQuestStarted);
            QuestManager.Instance.onQuestUpdated.RemoveListener(OnQuestUpdated);
            QuestManager.Instance.onQuestCompleted.RemoveListener(OnQuestCompleted);
        }

        DOTween.KillAll();
        StopAllCoroutines();
    }


    private void OnQuestStarted(QuestStatus questStatus)
    {
        QueueNotification($"New quest: {questStatus.quest.questName}", Color.cyan);
    }

    private void OnQuestUpdated(QuestStatus questStatus)
    {
        string questName = questStatus.quest.questName;

        if(!questUpdates.ContainsKey(questName))
        {
            questUpdates[questName] = new QuestUpdateTracker(questStatus);
        }
        else
        {
            questUpdates[questName].status = questStatus;
            questUpdates[questName].lastUpdateTime = Time.time;
            questUpdates[questName].isCompleted = questStatus.IsCompleted();
        }
    }

    private void OnQuestCompleted(QuestStatus questStatus)
    {
        string questName = questStatus.quest.questName;

        if(!questUpdates.ContainsKey(questName))
        {
            questUpdates[questName] = new QuestUpdateTracker(questStatus);
        }

        questUpdates[questName].isCompleted = true;
        questUpdates[questName].status = questStatus;
        questUpdates[questName].lastUpdateTime = Time.time;
    }


    private void CheckPendingUpdates()
    {
        List<string> questsToProcess = new List<string>();

        foreach (var entry in questUpdates)
        {
            if(!entry.Value.isProcessing && (Time.time - entry.Value.lastUpdateTime > groupingDelay))
            {
                questsToProcess.Add(entry.Key);
            }
        }


        foreach (string questName in questsToProcess)
        {
            ProcessQuestUpdate(questName);
        }
    }

    private void ProcessQuestUpdate(string questName)
    {
        if (!questUpdates.ContainsKey(questName))
            return;

        QuestUpdateTracker tracker = questUpdates[questName];
        tracker.isProcessing = true;

        QuestStatus status = tracker.status;

        if(tracker.isCompleted)
        {
            QueueNotification($"Completed Quest: {questName}\n" + "Return to quest giver", Color.green);
        }
        else
        {
            string progressText = GetProgressText(status);

            if(status.currentAmount > 0 || status.currentAmount % 5 == 0 )
            {
                QueueNotification($"Updated Quest: {questName}\n{progressText}",Color.white);
            }
        }

        questUpdates.Remove(questName);
    }



    private string GetProgressText(QuestStatus questStatus)
    {
        switch(questStatus.quest.questType)
        {
            case QuestType.KillEnemies:
                return $"Kill: {questStatus.currentAmount}:{questStatus.quest.requiredAmount} enemies";
            case QuestType.CollecItems:
                string itemName = questStatus.quest.requiredItem != null ?
                    questStatus.quest.requiredItem.itemName : "items";
                return $"Collect: {questStatus.currentAmount}/{questStatus.quest.requiredAmount} {itemName}";
            case QuestType.TalkToNPC:
                return "Talk to NPC";

            default:
                return $"Progression: {questStatus.currentAmount}/{questStatus.quest.requiredAmount}";
        }
    }


    private void QueueNotification(string message, Color color)
    {
        notificationQueue.Enqueue(new QuestNotificationInfo(message, color));

        if(activeNotifications < maxNotifications)
        {
            StartCoroutine(ProcessNotificationQueue());
        }
    }


    private IEnumerator ProcessNotificationQueue()
    {
        while(notificationQueue.Count>0 && activeNotifications<maxNotifications)
        {
            activeNotifications++;
            QuestNotificationInfo info = notificationQueue.Dequeue();
            yield return StartCoroutine(ShowNotification(info.message, info.color));

            activeNotifications--;
        }
    }


    private IEnumerator ShowNotification(string message, Color textColor)
    {
        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        RectTransform rectTransform = notification.GetComponent<RectTransform>();
        TextMeshProUGUI text = notification.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();

        if(!rectTransform || !text || !canvasGroup)
        {
            Destroy(notification);
            yield break;
        }

        text.text = message;
        text.color = textColor;
        canvasGroup.alpha = 0;

        Vector2 targetPos = rectTransform.anchoredPosition + new Vector2(0, riseDistance);

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1, fadeInTime));
        seq.Join(rectTransform.DOAnchorPos(targetPos, displayTime).SetEase(Ease.OutQuad));
        seq.Append(canvasGroup.DOFade(0, fadeOutTime));

        yield return seq.WaitForCompletion();

        if(notification)
        {
            Destroy(notification);
        }
    }
}

























