using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { HEALING, EQUIPMENT };
    public ItemType type;
    public int healAmt;
}
