using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestJournalDialog : MonoBehaviour
{
    public QuestGiverDialog questGiverDialog;
    public GameObject questJournalEntryPrefab;
    public Quest questInJournal;
    public GameObject questJournalPanel;
    public QuestDB questDB;

    CanvasGroup questJournalDialogCanvasGroup;
    QuestSystem questSystem;
    QuestJournalEntry activeQuestJournalEntry;
    QuestJournalEntry[] questJournalEntries;

    public void Start()
    {
        questJournalDialogCanvasGroup = GetComponent<CanvasGroup>();
        questSystem = GameObject.FindGameObjectWithTag("QuestSystem").GetComponent<QuestSystem>();
        
        Debug.Log($"QuestJournalDialog Start() position={GetComponent<RectTransform>().anchoredPosition.ToString()}");
    }

    public void OpenQuestJournalWindow()
    {
        questJournalDialogCanvasGroup.alpha = 1;
        questJournalDialogCanvasGroup.interactable = true;
        questJournalDialogCanvasGroup.blocksRaycasts = true;
    }

    public void CloseQuestJournalWindow()
    {
        questJournalDialogCanvasGroup.alpha = 0;
        questJournalDialogCanvasGroup.interactable = false;
        questJournalDialogCanvasGroup.blocksRaycasts = false;
    }

    public void AddQuestToJournal()
    {
        Debug.Log($"AddQuestToJournal quest={questInJournal.name}");
        Transform questJournalTransform = questJournalPanel.transform;
        GameObject activeQuest = Instantiate(questJournalEntryPrefab, questJournalTransform);
        activeQuestJournalEntry = activeQuest.GetComponent<QuestJournalEntry>();
        activeQuestJournalEntry.activeQuestText.text = questInJournal.title;
        activeQuestJournalEntry.activeQuest = questInJournal;
        Debug.Log("Adding Quest to Journal: " + questInJournal.questId);
    }

    public void AddActiveQuestsToJournal()
    {
        Debug.Log($"AddActiveQuestsToJournal Started | questDB={questDB.name} | transform={questDB.GetComponent<Transform>()}");
        for (int i = 0; i < questDB.GetComponent<Transform>().childCount; i++)
        {
            Transform child = questDB.transform.GetChild(i);
            if (child != null)
            {
                Quest quest = child.GetComponent<Quest>();
                Debug.Log($"quest={quest.name} | isActive={quest.isActive}");
                
                if (PlayerPrefs.GetInt("QuestIsActive-" + quest.questId) == 1)
                {
                    quest.isActive = true;
                }

                if (PlayerPrefs.GetInt("QuestIsComplete-" + quest.questId) == 1)
                {
                    quest.isDone = true;

                    quest.isActive = false;
                    PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 0);
                }

                if (quest.isActive)
                {
                    questInJournal = quest;
                    AddQuestToJournal();
                }
            }
        }
    }

    public void RemoveQuestFromJournal()
    {
        Transform questJournalTransform = questJournalPanel.transform;

        foreach(QuestJournalEntry obj in questJournalPanel.GetComponentsInChildren<QuestJournalEntry>())
        {
            if (obj.activeQuest == questInJournal)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
