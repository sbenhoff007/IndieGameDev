using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestJournalEntry : MonoBehaviour
{
    public Quest activeQuest;
    public Text activeQuestText;

    QuestSystem questSystem;
    QuestJournalDialog questJournalDialog;
    GameObject player;

    public void Start()
    {
        questSystem = GameObject.FindGameObjectWithTag("QuestSystem").GetComponent<QuestSystem>();
        questJournalDialog = GameObject.FindGameObjectWithTag("QuestJournalDialog").GetComponent<QuestJournalDialog>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public IEnumerator OpenActiveQuestWindowCoroutine()
    {
        yield return new WaitForSeconds(3f);
        OpenActiveQuestWindow();
    }

    public void OpenActiveQuestWindow()
    {
        Debug.Log($"questSystem={questSystem.name} | activeQuest={activeQuest.title} | questJournal={questJournalDialog.name}");
        RectTransform questJournalTransform = questJournalDialog.GetComponent<RectTransform>();
        Vector3 questJournalPosition = questJournalTransform.anchoredPosition;
        Vector2 offsetMax = questJournalTransform.offsetMax;
        Vector2 offsetMin = questJournalTransform.offsetMin;
        questJournalTransform.offsetMin = new Vector2(offsetMin.x, 0);
        questJournalPosition.y = offsetMax.y;

        Debug.Log($"questJournalPosition={questJournalPosition}");
        Debug.Log($"questJournalDialog x={questJournalPosition.x} | y={questJournalPosition.y} | offsetMin={questJournalTransform.offsetMin}");
        
        questSystem.quest = activeQuest;
        questSystem.OpenQuestWindow(questJournalPosition);
    }
}
