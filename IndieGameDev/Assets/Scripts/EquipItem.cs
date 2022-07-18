using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    public GameObject effect;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //This is where we will equip the item & change the player's sprite
    public void Use()
    {
        Instantiate(effect, player.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
