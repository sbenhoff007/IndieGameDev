using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Sprite fishSprite;
    public string fishName;
    public string fishDescription;
    public float fishMinLength;
    public float fishMaxLength;
    public float fishMinWeight;
    public float fishMaxWeight;
    public GameObject fishButton;

    Inventory inventory;    

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public void AddFishToInventory(Fish fish)
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] == false)
            {
                //Item can be added to inventory!
                inventory.isFull[i] = true;
                Instantiate(fishButton, inventory.slots[i].transform, false);
                Debug.Log("Fish Added to Inventory Slot " + i);
                break;
            }
        }
    }
}
