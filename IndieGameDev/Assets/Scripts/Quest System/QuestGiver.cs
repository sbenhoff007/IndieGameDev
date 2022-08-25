using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public RyanKHawkinsController player;

    public QuestGiverDialog questGiverDialog;
    public Text titleText;
    public Text descriptionText;
    public Text experienceRewardText;
    public Text goldRewardText;
    public Image questItemSprite;

    CanvasGroup canvasGroup;
    GamePlaySystem gamePlaySystem;
    Inventory inventory;
    bool isActive = false;
    bool isComplete = false;
    bool hasQuestItem = false;
    int inventorySlotWithQuestItem;

    public void Start()
    {
        canvasGroup = questGiverDialog.questDialog.GetComponent<CanvasGroup>();
        gamePlaySystem = GetComponent<GamePlaySystem>();
        inventory = player.GetComponent<Inventory>();

        if (PlayerPrefs.GetInt("QuestIsActive-" + quest.questId) == 1)
        {
            isActive = true;
            quest.isActive = true;
        }

        if (PlayerPrefs.GetInt("QuestIsComplete-" + quest.questId) == 1)
        {
            isComplete = true;
            quest.isDone = true;
        }
    }

    public void OpenQuestWindow()
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
            Debug.Log("Switch to quest complete button in the quest dialog...");

            questGiverDialog.acceptQuestButton.gameObject.SetActive(false);
            questGiverDialog.completeQuestButton.gameObject.SetActive(true);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }
        else
        {
            questGiverDialog.acceptQuestButton.gameObject.SetActive(true);
            questGiverDialog.completeQuestButton.gameObject.SetActive(false);
            questGiverDialog.declineQuestButton.gameObject.SetActive(true);
        }

        titleText.text = questTitle;
        descriptionText.text = quest.description;
        experienceRewardText.text = "Exp " + quest.experienceReward.ToString();
        goldRewardText.text = "Gold " + quest.goldReward.ToString();
        questItemSprite.sprite = quest.questItem.fishSprite;        
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseQuestWindow()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void AcceptQuest()
    {
        hasQuestItem = HasQuestItem();

        if (!quest.isDone && !quest.isActive)
        {
            quest.isActive = true;
            gamePlaySystem.ShowInfoDialog("Quest accepted!", 2f);
        }
        else if (!quest.isDone && quest.isActive && hasQuestItem)
        {
            gamePlaySystem.ShowInfoDialog("You have the item needed to turn in this quest!", 2f);
        }
        else if (!quest.isDone && quest.isActive)
        {
            gamePlaySystem.ShowInfoDialog("You are already on that quest.", 2f);
        }
        else
        {
            quest.isActive = false;
            gamePlaySystem.ShowInfoDialog("You already completed that quest.", 2f);
        }

        PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 1);

        CloseQuestWindow();
    }

    public bool HasQuestItem()
    {
        Fish questItem = quest.questItem;
        bool hasQuestItem = false;

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
        
        PlayerPrefs.DeleteKey("InventoryItem" + inventorySlotWithQuestItem);

        // Save all data
        PlayerPrefs.SetInt("PlayerCurrentExperiencePoints", newExp);
        PlayerPrefs.SetInt("PlayerCurrentGold", newGold);
        PlayerPrefs.SetInt("QuestIsActive-" + quest.questId, 0);
        PlayerPrefs.SetInt("QuestIsComplete-" + quest.questId, 1);

        Debug.Log("Quest Completed!");
        CloseQuestWindow();
    }
}
