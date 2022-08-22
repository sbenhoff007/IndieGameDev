using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Slider hpSlider;

    public void SetHUD(Fish fish)
    {
        hpSlider.maxValue = fish.fishMaxHealth;
        hpSlider.value = fish.fishCurrentHealth;
    }

    public void SetHUD(RyanKHawkinsController player)
    {
        hpSlider.maxValue = player.maxHealth;
        hpSlider.value = player.currentHealth;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
