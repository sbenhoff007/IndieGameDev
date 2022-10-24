using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RyanKHawkinsController : MonoBehaviour
{
    public float speed = 3.0f;
    public GameObject catchInfoPrefab;
    public int currentLevel = 1;
    public int maxLevel = 99;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public int currentExperiencePoints = 0;
    public int maxExperiencePoints = 100;
    public int currentGold = 0;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    SpriteRenderer fishing;
    Vector2 lookDirection = new Vector2(1, 0);

    bool hasFishingRod = false;
    bool isWater = false;
    bool isFishing = false;
    bool isCatching = false;
    int iRandomWait = 0;
    int iSuccessWait = 250;
    //Inventory inventory;
    GamePlaySystem gamePlaySystem;
    //QuestSystem questSystem;
    Coroutine fishingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        fishing = GetComponent<SpriteRenderer>();
        //inventory = GetComponent<Inventory>();
        gamePlaySystem = GameObject.FindGameObjectWithTag("GameplaySystem").GetComponent<GamePlaySystem>();
        //questSystem = GameObject.FindGameObjectWithTag("QuestSystem").GetComponent<QuestSystem>();

        LoadPlayerPrefs();
    }

    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("PlayerCurrentHealth"))
        {
            this.currentHealth = PlayerPrefs.GetInt("PlayerCurrentHealth");
        }

        if (PlayerPrefs.HasKey("PlayerCurrentLevel"))
        {
            this.currentLevel = PlayerPrefs.GetInt("PlayerCurrentLevel");
        }

        if (PlayerPrefs.HasKey("PlayerCurrentExperiencePoints"))
        {
            this.currentExperiencePoints = PlayerPrefs.GetInt("PlayerCurrentExperiencePoints");
        }

        if (PlayerPrefs.HasKey("PlayerMaxExperiencePoints"))
        {
            this.maxExperiencePoints = PlayerPrefs.GetInt("PlayerMaxExperiencePoints");
        }

        if (PlayerPrefs.HasKey("PlayerMaxHealth"))
        {
            this.maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth");
        }

        if (PlayerPrefs.HasKey("PlayerCurrentGold"))
        {
            this.currentGold = PlayerPrefs.GetInt("PlayerCurrentGold");
        }

        if (PlayerPrefs.HasKey("PlayerCurrentPositionX") && PlayerPrefs.HasKey("PlayerCurrentPositionY"))
        {
            
            if (!SceneManager.GetActiveScene().name.Equals("BattleScene"))
            {
                float positionX = PlayerPrefs.GetFloat("PlayerCurrentPositionX");
                float positionY = PlayerPrefs.GetFloat("PlayerCurrentPositionY");
                transform.position = new Vector2(positionX, positionY);
            }
        }

        Debug.Log("Player Info: CurrentHealth: " + this.currentHealth + "; CurrentLevel: " + this.currentLevel +
            "; CurrentExpPoints: " + this.currentExperiencePoints + "; MaxExpPoints: " + this.maxExperiencePoints +
            "; MaxHealth: " + this.maxHealth + "; CurrentGold: " + this.currentGold);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFishing && !isCatching)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }

        //for (int i = 0; i < inventory.slots.Length; i++)
        //{
        //    //Iterate through the slots to see if the FishingRod tag is present on an object
        //    if (inventory.slots[i] != null && inventory.slots[i].transform != null)
        //    {
        //        if (inventory.slots[i].transform.childCount > 0 && inventory.slots[i].transform.GetChild(0).gameObject != null)
        //        {
        //            hasFishingRod = inventory.slots[i].transform.GetChild(0).CompareTag("FishingRod");
        //            if (hasFishingRod)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}

        if (!isCatching && hasFishingRod && isWater && Input.GetKeyDown(KeyCode.F))
        {        
            animator.SetTrigger("Fishing");
            isFishing = isFishing ? false : true;

            if (isFishing)
            {
                fishingCoroutine = StartCoroutine(FishingCoroutine());
            }
            else
            {
                StopCoroutine(fishingCoroutine);
            }
        }
        else if (!isCatching && !hasFishingRod && isWater && Input.GetKeyDown(KeyCode.F))
        {
            gamePlaySystem.ShowInfoDialog("You need a fishing rod to fish!", 2f);
        }

        //if (!isCatching && !isFishing && Input.GetKeyDown(KeyCode.X))
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
        //    if (hit.collider != null)
        //    {
        //        QuestGiver questGiver = hit.collider.gameObject.GetComponentInChildren<QuestGiver>();
        //        if (questGiver != null)
        //        {
        //            questGiver.enabled = true;
        //            QuestGiver startingQuestGiver = GameObject.FindGameObjectWithTag("StartingQuestGiver").GetComponent<QuestGiver>();
        //            if (startingQuestGiver.quest.isActive)
        //            {
        //                questSystem.quest = startingQuestGiver.quest;
        //                questSystem.CompleteQuest();
        //                StartCoroutine(questGiver.OpenQuestWindowCoroutine());
        //            }
        //            else
        //            {
        //                questGiver.OpenQuestGiverWindow();
        //            }                    
        //        }
        //    }
        //}
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        PlayerPrefs.SetFloat("PlayerCurrentPositionX", position.x);
        PlayerPrefs.SetFloat("PlayerCurrentPositionY", position.y);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        isWater = other.gameObject.CompareTag("Water");
    }

    void OnCollisionExit2D(Collision2D other)
    {
        isWater = false;
    }

    IEnumerator FishingCoroutine()
    {
        if (isFishing && !isCatching)
        {
            //Print the time of when the function is first called.
            Debug.Log("Started Coroutine at timestamp : " + Time.time);

            //Wait a random # of seconds & show the info animation
            iRandomWait = Random.Range(1, 10);
            yield return new WaitForSeconds(iRandomWait);

            //Show the info sprite
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector2 playerPos = new Vector2(player.position.x, player.position.y + 1);
            GameObject catchInfoObject = Instantiate(catchInfoPrefab, playerPos, Quaternion.identity);
            isCatching = true;

            while (!Input.GetKeyDown(KeyCode.C))
            {
                iSuccessWait = iSuccessWait > 0 ? iSuccessWait - 1 : 0;
                Debug.Log("iSuccessWait while C is not pressed = " + iSuccessWait);

                if (iSuccessWait <= 0)
                {
                    animator.SetTrigger("Fishing");
                    isFishing = false;
                    isCatching = false;
                    GameObject.Destroy(catchInfoObject);
                    iSuccessWaitReset();
                    gamePlaySystem.ShowInfoDialog("The fish got away!");
                    yield return new WaitForSeconds(5);
                    gamePlaySystem.HideInfoDialog();
                    yield break;
                }

                yield return null;
            }

            if (Input.GetKeyDown(KeyCode.C) && iSuccessWait > 0)
            {
                isCatching = false;
                isFishing = false;

                //Switch to Battle Scene
                SceneManager.LoadSceneAsync("BattleScene");
            }
        }

        iSuccessWaitReset();
        isCatching = false;
        isFishing = false;
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    int iSuccessWaitReset()
    {
        iSuccessWait = 250;
        return iSuccessWait;
    }

    public void LevelUp()
    {
        currentLevel = currentLevel + 1;
        currentHealth = maxHealth * currentLevel;
        maxExperiencePoints = maxExperiencePoints * currentLevel;
        maxHealth = currentHealth;

        PlayerPrefs.SetInt("PlayerCurrentLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerCurrentHealth", currentHealth);
        PlayerPrefs.SetInt("PlayerMaxExperiencePoints", maxExperiencePoints);
        PlayerPrefs.SetInt("PlayerMaxHealth", maxHealth);

        Debug.Log("Player Info: CurrentHealth: " + this.currentHealth + "; CurrentLevel: " + this.currentLevel +
            "; CurrentExpPoints: " + this.currentExperiencePoints + "; MaxExpPoints: " + this.maxExperiencePoints +
            "; MaxHealth: " + this.maxHealth + "; CurrentGold: " + this.currentGold);
    }

    public void UseHealingItem(Item item)
    {
        bool isItem = item.gameObject.CompareTag("Item");

        if (isItem)
        {
            Item healingItem = item.gameObject.GetComponent<Item>();
            if (healingItem != null)
            {
                bool isHealing = healingItem.type.Equals(Item.ItemType.HEALING);
                if (isHealing)
                {
                    int healAmt = healingItem.healAmt;
                    currentHealth = ChangeHealth(healAmt);

                    foreach (Transform child in transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    Debug.Log($"Healing Item Found! Current Health:{currentHealth}");
                }
            }
        }
    }
    public int ChangeHealth(int amount)
    {
        //if (amount < 0)
        //{
        //    if (isInvincible)
        //        return;

        //    isInvincible = true;
        //    invincibleTimer = timeInvincible;
        //}

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
        return currentHealth;
    }
}
