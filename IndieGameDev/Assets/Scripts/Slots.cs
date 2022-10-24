using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    private Inventory inventory;
    public int i = 0;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        if (PlayerPrefs.HasKey("InventoryItem" + i))
        {
            //Debug.Log("PlayerPrefs has key InventoryItem" + i + "=" + PlayerPrefs.GetString("InventoryItem" + i));
            
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString("InventoryItem" + i)))
                {
                    //Item can be loaded from the saved inventory!
                    inventory.isFull[i] = true;
                    GameObject inventoryItem = GameObject.Find(PlayerPrefs.GetString("InventoryItem" + i));
                    
                    if (inventoryItem != null && inventoryItem.CompareTag("Fish")){
                        //Debug.Log("inventory item tag is Fish: " + inventoryItem.name);
                        GameObject fishButton = inventoryItem.GetComponent<Fish>().fishButton;
                        Instantiate(fishButton, inventory.slots[i].transform, false);
                    }
                    else
                    {
                        //Debug.Log("inventory item tag is not Fish: " + inventoryItem.name);
                        GameObject itemButton = inventoryItem.GetComponent<Pickup>().itemButton;
                        Instantiate(itemButton, inventory.slots[i].transform, false);
                        Destroy(inventoryItem);
                    }                    
                }
            }
        }
    }

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inventory.isFull[i] = false;
            PlayerPrefs.DeleteKey("InventoryItem" + i);
        }
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("Fish"))
            {
                child.GetComponent<Spawn>().SpawnDroppedItem();
            }
            
            GameObject.Destroy(child.gameObject);
        }
    }
}
