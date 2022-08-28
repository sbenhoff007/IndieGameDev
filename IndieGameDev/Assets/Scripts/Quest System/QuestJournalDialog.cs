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
        Transform questJournalTransform = questJournalPanel.transform;
        GameObject activeQuest = Instantiate(questJournalEntryPrefab, questJournalTransform);
        activeQuestJournalEntry = activeQuest.GetComponent<QuestJournalEntry>();
        activeQuestJournalEntry.activeQuestText.text = questInJournal.title;
        activeQuestJournalEntry.activeQuest = questInJournal;
        Debug.Log("Adding Quest to Journal: " + questInJournal.questId);
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
