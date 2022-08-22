using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlaySystem : MonoBehaviour
{
    public GameObject infoDialog;
    public Text infoText;

    CanvasGroup canvasGroup;
    string newInfoText;

    void Start()
    {
        canvasGroup = infoDialog.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowInfoDialog(string dialogText)
    {
        infoText.text = dialogText;
        canvasGroup.alpha = 1;
    }

    public void HideInfoDialog() 
    {
        canvasGroup.alpha = 0;
    }
}
