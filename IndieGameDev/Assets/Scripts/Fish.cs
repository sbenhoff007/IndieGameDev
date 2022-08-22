using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fish : MonoBehaviour
{
    public Sprite fishSprite;
    public string fishName;
    public string fishDescription;
    public float fishMinLength;
    public float fishMaxLength;
    public float fishCurrentLength;
    public float fishMinWeight;
    public float fishMaxWeight;
    public float fishCurrentWeight;
    public GameObject fishButton;
    public int fishCurrentLevel = 1;
    public int fishCurrentHealth = 10;
    public int fishMaxHealth = 10;
}
