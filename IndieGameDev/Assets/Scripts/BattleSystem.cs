using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, FISHTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public Text dialogueText;
    public Text infoText;
    public Text fishNameText;
    public Text fishLevelText;
    public Text playerNameText;
    public Text playerLevelText;
    public BattleHUD fishHUD;
    public BattleHUD playerHUD;
    public BattleState state;
    public GameObject playerActionButtons;

    GameObject fishObject;
    SpriteRenderer fishObjectSprite;
    Fish fish;
    RyanKHawkinsController player;
    string fishName;
    int fishLevel;
    Sprite fishSprite;

    Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RyanKHawkinsController>();

        fish = GetRandomFish();

        // Set the HUD text values
        infoText.gameObject.SetActive(false);
        dialogueText.text = "A wild " + fish.fishName + " approaches!";
        fishNameText.text = fish.fishName;
        fishLevelText.text = "Lvl " + fish.fishCurrentLevel;
        playerNameText.text = player.name;

        player.LoadPlayerPrefs();

        playerLevelText.text = "Lvl " + player.currentLevel;
        playerHUD.SetHUD(player);

        state = BattleState.PLAYERTURN;
        StartCoroutine(SetupPlayerTurn());
    }

    Fish GetRandomFish()
    {
        // Get the fish
        FishCollection fishCollection = GameObject.FindGameObjectWithTag("FishCollection").GetComponent<FishCollection>();

        if (fishCollection != null)
        {
            int fishCount = fishCollection.GetComponentsInChildren<Fish>().Length;
            int iRandomFish = Random.Range(0, fishCount);
            fish = fishCollection.transform.GetChild(iRandomFish).gameObject.GetComponent<Fish>();
            fish.fishCurrentWeight = Random.Range(fish.fishMinWeight, fish.fishMaxWeight);
            fish.fishCurrentLength = Random.Range(fish.fishMinLength, fish.fishMaxLength);

            fishObject = GameObject.FindGameObjectWithTag("FishObject");
            fishObjectSprite = fishObject.GetComponent<SpriteRenderer>();
            fishObjectSprite.sprite = fish.fishSprite;
            fishHUD.SetHUD(fish);

            Debug.Log("Fish Collection Length: " + fishCount + " Fish ID: " + iRandomFish
                + " Fish Name: " + fish.fishName + " Fish Description: " + fish.fishDescription
                + " Fish Weight: " + fish.fishCurrentWeight + " Fish Length: " + fish.fishCurrentLength
                + " Fish Level: " + fish.fishCurrentLevel);

            return fish;
        }

        return null;
    }

    public void CatchFish()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        DisableActionButtons();

        bool fishAdded = AddFishToInventory(fish);
        bool leveledUp = false;
        string text;

        if (!fishAdded)
        {
            text = "No inventory slots available! You release the fish.";
            state = BattleState.LOST;
        }
        else
        {
            text = fish.fishName + " added to inventory!";
            player.currentExperiencePoints = player.currentExperiencePoints + 100;
            if (player.currentExperiencePoints >= player.maxExperiencePoints)
            {
                player.currentLevel = player.currentLevel + 1;
                player.currentHealth = player.maxHealth * player.currentLevel;
                text = text + " You leveled up! Lvl " + player.currentLevel;
                leveledUp = true;
            }
            infoText.gameObject.SetActive(true);
            state = BattleState.WON;
        }

        dialogueText.text = text;

        PlayerPrefs.SetInt("PlayerCurrentHealth", player.currentHealth);
        PlayerPrefs.SetInt("PlayerCurrentLevel", player.currentLevel);
        PlayerPrefs.SetInt("PlayerCurrentExperiencePoints", player.currentExperiencePoints);

        if (leveledUp)
        {
            PlayerPrefs.SetInt("PlayerMaxExperiencePoints", player.maxExperiencePoints * player.currentLevel);
            PlayerPrefs.SetInt("PlayerMaxHealth", player.maxHealth * player.currentLevel);
        }

        dialogueText.text = text;
        StartCoroutine(FinishBattle());
    }

    public void ReelFish()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        DisableActionButtons();
        StartCoroutine(PlayerAttack());
    }

    IEnumerator SetupPlayerTurn()
    {
        StopCoroutine(FishAttack());
        yield return new WaitForSeconds(3);
        string text = "It's your turn!";
        dialogueText.text = text;
        EnableActionButtons();
        ShowPlayerActions();
        yield return new WaitForSeconds(3);
    }
    IEnumerator PlayerAttack()
    {
        int amount = -5;
        fish.fishCurrentHealth = ChangeHealth(fish, amount);
        string text = amount + " points of damage to " + fish.fishName + "!";
        dialogueText.text = text;
        Debug.Log(fish.fishName + " health: " + fish.fishCurrentHealth + "/" + fish.fishMaxHealth);
        fishHUD.SetHP(fish.fishCurrentHealth);

        yield return new WaitForSeconds(3);

        if (fish.fishCurrentHealth <= 0)
        {
            text = "The fish died before you could catch it!";
            state = BattleState.LOST;
        }
        else
        {
            PlayerPrefs.SetInt("FishCurrentHealth", fish.fishCurrentHealth);
            state = BattleState.FISHTURN;
        }

        PlayerPrefs.SetInt("PlayerCurrentHealth", player.currentHealth);

        dialogueText.text = text;

        yield return new WaitForSeconds(3);

        while (!Input.GetKeyDown(KeyCode.C))
        {
            if (state == BattleState.FISHTURN)
            {
                SetupFishTurn();
                yield break;
            }
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state == BattleState.LOST)
            {
                SceneManager.LoadSceneAsync("SampleScene");
                yield break;
            }
        }
    }

    void SetupFishTurn()
    {
        if (state != BattleState.FISHTURN)
        {
            return;
        }

        //Hide the player's attack buttons
        HidePlayerActions();

        dialogueText.text = fish.fishName + " is attacking!";

        StartCoroutine(FishAttack());
    }

    IEnumerator FishAttack()
    {
        if (state != BattleState.FISHTURN)
        {
            yield break;
        }

        yield return new WaitForSeconds(3);
        int amount = -5;
        player.currentHealth = ChangeHealth(player, amount);
        dialogueText.text = amount + " points of damage to " + player.name + "!";
        playerHUD.SetHP(player.currentHealth);
        yield return new WaitForSeconds(3);

        Debug.Log(player.name + " health: " + player.currentHealth + "/" + player.maxHealth);

        if (player.currentHealth <= 0)
        {
            dialogueText.text = "You died!";
            state = BattleState.LOST;
            player.currentHealth = player.maxHealth;
            PlayerPrefs.SetInt("PlayerCurrentHealth", player.currentHealth);
            //yield return new WaitForSeconds(3);            
        }
        else
        {
            PlayerPrefs.SetInt("PlayerCurrentHealth", player.currentHealth);
            state = BattleState.PLAYERTURN;
            StartCoroutine(SetupPlayerTurn());
            yield break;
        }

        while (!Input.GetKeyDown(KeyCode.C))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state == BattleState.LOST)
            {
                SceneManager.LoadSceneAsync("SampleScene");
            }

            yield break;
        }

        Debug.Log("state=" + state);
        Debug.Log("Exited FishTurn Coroutine at timestamp : " + Time.time);
    }

    bool AddFishToInventory(Fish fish)
    {
        for (int i = 0; i <= 4; i++)
        {
            if (i > 3)
            {
                // Set the HUD text values
                string text = "No inventory slots available! You release the fish.";
                Debug.Log("i > 3! " + text);
                //StartCoroutine(ShowDialogueMessage(text));
                return false;
            }
            else
            {
                //Debug.Log("PlayerPrefs: InventoryItem" + i + " = " + PlayerPrefs.GetString("InventoryItem" + i));
                string playerPrefs = PlayerPrefs.GetString("InventoryItem" + i);
                if (string.IsNullOrEmpty(playerPrefs))
                {
                    //Item can be added to inventory!
                    inventory.isFull[i] = true;
                    PlayerPrefs.SetString("InventoryItem" + i, fish.fishName);
                    //Debug.Log(fish.fishName + " Added to Inventory Slot " + i);
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator FinishBattle()
    {
        Debug.Log("Started ShowDialogueMessage Coroutine at timestamp : " + Time.time);

        while (!Input.GetKeyDown(KeyCode.C))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadSceneAsync("SampleScene");
            yield break;
        }

        Debug.Log("Exited ShowDialogueMessage Coroutine at timestamp : " + Time.time);
    }

    public int ChangeHealth(RyanKHawkinsController player, int amount)
    {
        //if (amount < 0)
        //{
        //    if (isInvincible)
        //        return;

        //    isInvincible = true;
        //    invincibleTimer = timeInvincible;
        //}

        player.currentHealth = Mathf.Clamp(player.currentHealth + amount, 0, player.maxHealth);
        Debug.Log(player.currentHealth + "/" + player.maxHealth);
        return player.currentHealth;
    }

    public int ChangeHealth(Fish fish, int amount)
    {
        //if (amount < 0)
        //{
        //    if (isInvincible)
        //        return;

        //    isInvincible = true;
        //    invincibleTimer = timeInvincible;
        //}

        fish.fishCurrentHealth = Mathf.Clamp(fish.fishCurrentHealth + amount, 0, fish.fishMaxHealth);
        Debug.Log(fish.fishCurrentHealth + "/" + fish.fishMaxHealth);
        return fish.fishCurrentHealth;
    }

    void HidePlayerActions()
    {
        CanvasGroup canvasGroup;
        canvasGroup = playerActionButtons.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void ShowPlayerActions()
    {
        CanvasGroup canvasGroup;
        canvasGroup = playerActionButtons.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void DisableActionButtons()
    {
        CanvasGroup canvasGroup;
        canvasGroup = playerActionButtons.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void EnableActionButtons()
    {
        CanvasGroup canvasGroup;
        canvasGroup = playerActionButtons.GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
