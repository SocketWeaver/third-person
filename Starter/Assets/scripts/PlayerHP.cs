using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player Health point. Also updates player health bar when its health point changes
/// </summary>
public class PlayerHP : MonoBehaviour
{
    public int maxHp = 100;
    public Slider hpSlider;
    public GameObject explode;
    public int currentHP;

    void Start()
    {
        hpSlider.minValue = 0;
        hpSlider.maxValue = maxHp;
    }

    public void ResetHP()
    {
        hpSlider.value = maxHp;
    }

    public void GotHit(int damage)
    {
        if (currentHP == 0)
        {
            return;
        }

        if (currentHP > 0)
        {
            currentHP = currentHP - damage;

            if (currentHP < 0)
            {
                currentHP = 0;
            }
        }
    }
}
