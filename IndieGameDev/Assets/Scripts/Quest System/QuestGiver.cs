using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public GameObject questAvailablePrefab;

    GamePlaySystem gamePlaySystem;
    QuestSystem questSystem;
    bool isActive = false;
    bool isComplete = false;
    bool hasQuestItem = false;
    int inventorySlotWithQuestItem;
    GameObject questAvailableObject;

    public void Start()
    {
        gamePlaySystem = GameObject.FindGameObjectWithTag("GameplaySystem").GetComponent<GamePlaySystem>();
        questSystem = GameObject.FindGameObjectWithTag("QuestSystem").GetComponent<QuestSystem>();
        
        if (PlayerPrefs.GetInt("QuestIsActive-" + quest.questId) == 1)
        {
            isActive = true;
            quest.isActive = true;
        }

        if (PlayerPrefs.GetInt("QuestIsComplete-" + quest.questId) == 1)
        {
            isComplete = true;
            quest.isDone = true;

            isActive = false;
            quest.isActive = false;
            PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 0);
            HideQuestAvailable();
        } 
        
        if (!isComplete)
        {
            ShowQuestAvailable();
        }
    }

    public IEnumerator OpenQuestWindowCoroutine()
    {
        yield return new WaitForSeconds(3f);
        OpenQuestGiverWindow();
    }

    public void OpenQuestGiverWindow()
    {
        Transform questGiverTransform = GetComponent<Transform>();
        Vector3 questGiverPosition = questGiverTransform.position;
        questSystem.quest = quest;
        questSystem.OpenQuestWindow(questGiverPosition);
    }

    public void ShowQuestAvailable()
    {
        //Show the info sprite
        Transform npc = GetComponent<Transform>();
        Vector2 npcPos = new Vector2(npc.position.x, npc.position.y + 1);
        questAvailableObject = Instantiate(questAvailablePrefab, npcPos, Quaternion.identity);
    }

    public void HideQuestAvailable()
    {
        Destroy(questAvailableObject);
    }
}
