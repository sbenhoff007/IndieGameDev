using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevFunctions : MonoBehaviour
{
    GamePlaySystem gamePlaySystem;

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowControls()
    {
        gamePlaySystem = GameObject.FindGameObjectWithTag("GameplaySystem").GetComponent<GamePlaySystem>();
        string infoText = "F: Fishing | C: Catching | X: Interacting \r\n Arrows: Moving";
        gamePlaySystem.ShowInfoDialog(infoText, 3f);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
