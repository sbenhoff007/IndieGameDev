using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverDialog : MonoBehaviour
{
    public GameObject questDialog;
    public Quest quest;
    public Text questId;
    public Text titleText;
    public Text descriptionText;
    public Text experienceRewardText;
    public Text goldRewardText;
    public Image questItemSprite;
    public Button acceptQuestButton;
    public Button declineQuestButton;
    public Button completeQuestButton;
}
