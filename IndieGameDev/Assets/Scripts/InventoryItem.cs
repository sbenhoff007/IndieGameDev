using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public int count = 1;
    public Sprite sprite;

    //GameModel model = Schedule.GetModel<GameModel>();

    void Reset()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        //MessageBar.Show($"You collected: {name} x {count}");
        //model.AddInventoryItem(this);
        //UserInterfaceAudio.OnCollect();
        gameObject.SetActive(false);
    }
}
