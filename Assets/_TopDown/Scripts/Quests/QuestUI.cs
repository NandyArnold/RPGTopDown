using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using NUnit.Framework.Constraints;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questLogPanel;
    public GameObject questEntryPrefab;
    public Transform questContent;
    public Button closeButton;

    [Header("Input Settings")]
    public InputActionReference toggleQuestLogAction;

    [Header("UI Settings")]
    public Color activeQuestColor = Color.white;
    public Color completeQuestColor = Color.green;
    public Color returnInQuestColor = Color.yellow;

    private List<GameObject> questEntries = new List<GameObject>();
    private bool isQuestLogOpen;

    private void Awake()
    {
        if(questLogPanel != null)
        {
            questLogPanel.SetActive(false);
        }
        if(toggleQuestLogAction != null)
        {
            toggleQuestLogAction.action.started += OnToggleQuestLog;
        }

        if(closeButton != null)
        {
            closeButton.onClick.AddListener(ToggleQuestLog);
        }
    }


    private void OnEnable()
    {
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.onQuestStarted.AddListener(OnQuestStatusChanged);
            QuestManager.Instance.onQuestUpdated.AddListener(OnQuestStatusChanged);
            QuestManager.Instance.onQuestCompleted.AddListener(OnQuestStatusChanged);
        }

        if(toggleQuestLogAction != null)
        {
            toggleQuestLogAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.onQuestStarted.RemoveListener(OnQuestStatusChanged);
            QuestManager.Instance.onQuestUpdated.RemoveListener(OnQuestStatusChanged);
            QuestManager.Instance.onQuestCompleted.RemoveListener(OnQuestStatusChanged);

        }

        if(toggleQuestLogAction != null)
        {
            toggleQuestLogAction.action.Disable();
        }
    }


    private void OnDestroy()
    {
        if(toggleQuestLogAction != null)
        {
            toggleQuestLogAction.action.started -= OnToggleQuestLog;
        }

        if(closeButton != null)
        {
            closeButton.onClick.RemoveListener(ToggleQuestLog);
        }
    }


    private void OnToggleQuestLog(InputAction.CallbackContext context)
    {
        ToggleQuestLog();
    }


    private void ToggleQuestLog()
    {
        isQuestLogOpen = !isQuestLogOpen;
        if(questLogPanel != null)
        {
            questLogPanel.SetActive(isQuestLogOpen);

            if(isQuestLogOpen)
            {
                RefreshQuestList();
            }
        }
    }



    private void OnQuestStatusChanged(QuestStatus questStatus)
    {
        if(isQuestLogOpen)
        {
            RefreshQuestList();
        }
    }


    private void RefreshQuestList()
    {
        foreach(GameObject entry in questEntries)
        {
            Destroy(entry);
        }

        questEntries.Clear();


        List<QuestStatus> activeQuests = GetActiveQuests();

        foreach(QuestStatus quest in activeQuests)
        {
            if(quest.state == QuestState.Active || quest.state == QuestState.Complete)
            {
                CreateQuestEntry(quest);
            }
        }

        if(questEntries.Count == 0)
        {
            CreateEmptyQuestMessage();
        }
    }


    private void CreateEmptyQuestMessage()
    {
        if (questEntryPrefab == null || questContent == null)
            return;

        GameObject entryObject = Instantiate(questEntryPrefab, questContent);
        questEntries.Add(entryObject);

        TextMeshProUGUI questNameText = entryObject.transform.Find("QuestName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questDescriptionText = entryObject.transform.Find("QuestDescription")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questProgressText = entryObject.transform.Find("QuestProgress")?.GetComponent<TextMeshProUGUI>();

        if(questNameText !=null)
        {
            questNameText.text = "No active quests";
        }

        if(questDescriptionText != null)
        {
            questDescriptionText.text = "Talk to an NPC to receive a quest";
        }


        if(questProgressText != null)
        {
            questProgressText.text = "";
        }
    }


    private List<QuestStatus> GetActiveQuests()
    {

        if(QuestManager.Instance != null)
        {
            return QuestManager.Instance.GetActiveQuests();
        }

        return new List<QuestStatus>();
    }


    private void CreateQuestEntry(QuestStatus questStatus)
    {
        if (questEntryPrefab == null || questContent == null)
            return;

        GameObject entryObject = Instantiate(questEntryPrefab, questContent);
        questEntries.Add(entryObject);

        TextMeshProUGUI questNameText = entryObject.transform.Find("QuestName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questDescriptionText = entryObject.transform.Find("QuestDescription")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questProgressText = entryObject.transform.Find("QuestProgress")?.GetComponent<TextMeshProUGUI>();
        Image questStatusImage = entryObject.transform.Find("QuestStatusIcon")?.GetComponent<Image>();

        if(questNameText != null)
        {
            questNameText.text = questStatus.quest.questName;

            if(questStatus.quest.isMainQuest)
            {
                questNameText.text += "[Main Quest]";
                questNameText.color = Color.yellow;
            }
        }

        if(questDescriptionText != null)
        {
            questDescriptionText.text = questStatus.quest.questDescription;
        }

        if(questDescriptionText != null)
        {
            questDescriptionText.text = questStatus.quest.questDescription;
        }

        if(questProgressText != null)
        {
            string progressText = "";
            
            switch(questStatus.quest.questType)
            {
                case QuestType.KillEnemies:
                    progressText = $"Kill: {questStatus.currentAmount}/{questStatus.quest.requiredAmount} enemies";
                    break;

                case QuestType.CollecItems:
                    string itemName = questStatus.quest.requiredItem != null ? questStatus.quest.requiredItem.itemName : "objects";
                    progressText = $"Collect: {questStatus.currentAmount}/{questStatus.quest.requiredAmount} {itemName}";
                    break;

                case QuestType.TalkToNPC:
                    progressText = "Talk to NPC";
                    break;

                default:
                    progressText = $"Progression. {questStatus.currentAmount}/{questStatus.quest.requiredAmount}";
                    break;
            }

            questProgressText.text = progressText;
        }


        if(questStatusImage != null)
        {
            if(questStatus.state == QuestState.Complete)
            {
                questStatusImage.color = completeQuestColor;
            }
            else if(questStatus.IsCompleted())
            {
                questStatusImage.color = returnInQuestColor;
            }
            else
            {
                questStatusImage.color = activeQuestColor;
            }
        }
    }


    private void OnQuestEntryClicked(QuestStatus questStatus)
    {
        Debug.Log($"Quest entry clicked: {questStatus.quest.questName}");
    }






    


}
