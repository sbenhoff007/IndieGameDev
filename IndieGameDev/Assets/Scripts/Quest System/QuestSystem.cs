using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestSystem : MonoBehaviour
{
    public Quest quest;
    public QuestGiverDialog questGiverDialog;
    public QuestJournalDialog questJournalDialog;
    
    GameObject playerObject;
    RyanKHawkinsController player;
    CanvasGroup questGiverDialogCanvasGroup;
    QuestJournalEntry questJournalEntry;
    
    GamePlaySystem gamePlaySystem;
    Inventory inventory;
    bool isActive = false;
    bool isComplete = false;
    bool hasQuestItem = false;
    int inventorySlotWithQuestItem;
    Vector3 questGiverDialogPosition;
    
    public void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RyanKHawkinsController>();
        questGiverDialogCanvasGroup = questGiverDialog.questDialog.GetComponent<CanvasGroup>();
        questJournalEntry = questJournalDialog.questJournalEntryPrefab.GetComponent<QuestJournalEntry>();   
        gamePlaySystem = GameObject.FindGameObjectWithTag("GameplaySystem").GetComponent<GamePlaySystem>();
        inventory = playerObject.GetComponent<Inventory>();
        
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
        }

        if (PlayerPrefs.GetInt("QuestIsComplete-1") == 0)
        {
            //Starting Quest is Active
            questGiverDialogPosition = new Vector2(-300, 0);
            OpenQuestWindow(questGiverDialogPosition);
        }
        else
        {
            questJournalDialog.AddActiveQuestsToJournal();
        }
    }

    public void OpenQuestWindow(Vector3 questGiverDialogPosition)
    { 
        isActive = quest.isActive;
        isComplete = quest.isDone;
        hasQuestItem = HasQuestItem();
        string questTitle = quest.title;

        if (isComplete)
        {
            questTitle = "QUEST COMPLETE: " + quest.title;
            questGiverDialog.acceptQuestButton.gameObject.SetActive(false);
            questGiverDialog.completeQuestButton.gameObject.SetActive(false);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }
        else if (!isComplete && !isActive && !hasQuestItem)
        {
            questGiverDialog.acceptQuestButton.gameObject.SetActive(true);
            questGiverDialog.completeQuestButton.gameObject.SetActive(false);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }
        else if (!isComplete && isActive && hasQuestItem)
        {
            questGiverDialog.acceptQuestButton.gameObject.SetActive(false);
            questGiverDialog.completeQuestButton.gameObject.SetActive(true);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }
        else if (!isComplete && isActive && !hasQuestItem)
        {
            questGiverDialog.acceptQuestButton.gameObject.SetActive(true);
            questGiverDialog.completeQuestButton.gameObject.SetActive(false);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }
        else
        {
            questGiverDialog.acceptQuestButton.gameObject.SetActive(false);
            questGiverDialog.completeQuestButton.gameObject.SetActive(false);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }

        questGiverDialog.questId.text = quest.questId.ToString();
        questGiverDialog.titleText.text = questTitle;
        questGiverDialog.descriptionText.text = quest.description;
        questGiverDialog.experienceRewardText.text = "Exp " + quest.experienceReward.ToString();
        questGiverDialog.goldRewardText.text = "Gold " + quest.goldReward.ToString();
        if (quest.questItem != null)
        {
            questGiverDialog.questItemSprite.sprite = quest.questItem.fishSprite;
        } 
        else
        {
            questGiverDialog.questItemSprite.enabled = false;
        }

        RectTransform rt = questGiverDialog.GetComponent<RectTransform>();
        //rt.offsetMin = new Vector2(questGiverDialogPosition.x, -1); //left-bottom
        //rt.offsetMax = new Vector2(0, questGiverDialogPosition.y);//right-top
        rt.anchoredPosition = questGiverDialogPosition;
        Debug.Log($"questGiverDialogPosition:{rt.anchoredPosition}");
        Debug.Log($"questGiverDialog x={rt.anchoredPosition.x} | y={rt.anchoredPosition.y}");

        questGiverDialogCanvasGroup.alpha = 1;
        questGiverDialogCanvasGroup.interactable = true;
        questGiverDialogCanvasGroup.blocksRaycasts = true;
    }

    public void CloseQuestWindow()
    {
        questGiverDialogCanvasGroup.alpha = 0;
        questGiverDialogCanvasGroup.interactable = false;
        questGiverDialogCanvasGroup.blocksRaycasts = false;
    }

    public void AcceptQuest()
    {
        Debug.Log("Current Active Quest: AcceptQuest() " + quest.questId);
        
        if (quest.questItem == null)
        {
            hasQuestItem = false;
        }
        else
        {
            hasQuestItem = HasQuestItem();
        }        

        if (!quest.isDone && !quest.isActive)
        {
            quest.isActive = true;
            PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 1);
            gamePlaySystem.ShowInfoDialog("Quest accepted!", 2f);
            questJournalDialog.questInJournal = quest;
            questJournalDialog.AddQuestToJournal();
        }
        else if (!quest.isDone && quest.isActive && hasQuestItem)
        {
            gamePlaySystem.ShowInfoDialog("You have the item needed to turn in this quest!", 2f);
        }
        else if (!quest.isDone && quest.isActive && quest.questId != 1)
        {
            gamePlaySystem.ShowInfoDialog("You are already on that quest.", 2f);
        }
        else
        {
            quest.isActive = false;
            PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 0);
            gamePlaySystem.ShowInfoDialog("You already completed that quest.", 2f);
        }

        CloseQuestWindow();
    }

    public bool HasQuestItem()
    {
        Fish questItem = quest.questItem;
        bool hasQuestItem = false;

        if (quest.questId == 0 || inventory == null || inventory.slots == null)
        {
            return false;
        }

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            //Iterate through the slots to see if the quest item is present
            if (inventory.slots[i] != null && inventory.slots[i].transform != null)
            {
                if (inventory.slots[i].transform.childCount > 0 && inventory.slots[i].transform.GetChild(0).gameObject != null)
                {
                    bool hasFish = inventory.slots[i].transform.GetChild(0).CompareTag("Fish");
                    GameObject fishItem = inventory.slots[i].transform.GetChild(0).gameObject;

                    Debug.Log("hasFish=" + hasFish);
                    Debug.Log("hasQuestItem=" + hasQuestItem + " | questItemName=" + questItem.fishButton.name
                                + " | inventoryItemName=" + fishItem.name);

                    if (hasFish)
                    {                        
                        if (fishItem.name.Contains(questItem.fishButton.name))
                        {
                            hasQuestItem = true;
                            inventorySlotWithQuestItem = i;
                            break;
                        }
                    }
                }
            }
        }

        return hasQuestItem;
    }

    public void CompleteQuest()
    {
        // Add the exp and coins to the player's save data
        int rewardExp = quest.experienceReward;
        int coins = quest.goldReward;

        int newExp = player.currentExperiencePoints += rewardExp;
        int newGold = player.currentGold += coins;
        quest.isActive = false;
        quest.isDone = true;

        if (player.currentExperiencePoints >= player.maxExperiencePoints)
        {
            player.LevelUp();
        }

        // Remove the reward item from the player's inventory
        Transform inventorySlotTransform = inventory.slots[inventorySlotWithQuestItem].transform;

        foreach (Transform child in inventorySlotTransform)
        {
            Debug.Log("Inventory Item Deleted: " + child.gameObject.name);
            GameObject.Destroy(child.gameObject);
        }

        questJournalDialog.questInJournal = quest;
        questJournalDialog.RemoveQuestFromJournal();

        PlayerPrefs.DeleteKey("InventoryItem" + inventorySlotWithQuestItem);

        // Save all data
        PlayerPrefs.SetInt("PlayerCurrentExperiencePoints", newExp);
        PlayerPrefs.SetInt("PlayerCurrentGold", newGold);
        PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 0);
        PlayerPrefs.SetInt("QuestIsComplete-" + quest.questId, 1);

        CloseQuestWindow();
        quest.questGiver.HideQuestAvailable();
        gamePlaySystem.ShowInfoDialog("Quest Complete! " + quest.title, 2f);
    }

    public IEnumerator OpenQuestWindowCoroutine()
    {
        yield return new WaitForSeconds(3f);
        questGiverDialogPosition = player.transform.position;
        OpenQuestWindow(questGiverDialogPosition);
    }
}
