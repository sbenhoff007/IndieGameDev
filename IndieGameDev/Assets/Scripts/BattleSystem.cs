using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public Text dialogueText;
    public Text fishNameText;
    public Text fishLevelText;
    public Text playerNameText;
    public Text playerLevelText;

    GameObject fishObject;
    SpriteRenderer fishObjectSprite;
    string fishName;
    string fishLevel;
    Sprite fishSprite;

    //Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        //inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        // Get the fish
        FishCollection fishCollection = GameObject.FindGameObjectWithTag("FishCollection").GetComponent<FishCollection>();

        if (fishCollection != null)
        {
            int fishCount = fishCollection.GetComponentsInChildren<Fish>().Length;
            int iRandomFish = Random.Range(0, fishCount);
            Fish fish = fishCollection.transform.GetChild(iRandomFish).gameObject.GetComponent<Fish>();
            float iRandomWeight = Random.Range(fish.fishMinWeight, fish.fishMaxWeight);
            float iRandomLength = Random.Range(fish.fishMinLength, fish.fishMaxLength);
            fishName = fish.fishName;
            //fishLevel = fish.fishLevel;
            fishSprite = fish.fishSprite;

            fishObject = GameObject.FindGameObjectWithTag("FishObject");
            Debug.Log(fishObject.name);
            fishObjectSprite = fishObject.GetComponent<SpriteRenderer>();
            fishObjectSprite.sprite = fishSprite;

            Debug.Log("Fish Collection Length: " + fishCount + " Fish ID: " + iRandomFish
                + " Fish Name: " + fish.fishName + " Fish Description: " + fish.fishDescription
                + " Fish Weight: " + iRandomWeight + " Fish Length: " + iRandomLength);

            //fish.AddFishToInventory(fish);
        }

        // Set the HUD text values
        dialogueText.text = "A wild " + fishName + " approaches!";
        fishNameText.text = fishName;
        fishLevelText.text = "Lvl " + "1";
        playerNameText.text = "Player 1";
        playerLevelText.text = "Lvl " + "1";
    }
}
