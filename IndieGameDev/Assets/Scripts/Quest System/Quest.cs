using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : MonoBehaviour
{
    public bool isActive = false;
    public bool isDone = false;

    public int questId;
    public string title;
    public string description;
    public int experienceReward;
    public int goldReward;
    public Fish questItem;
    public QuestGiver questGiver;
}
