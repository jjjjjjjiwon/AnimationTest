using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHpBacUI : MonoBehaviour
{
    public Slider slider;

    public void SetHP(float current, float max)
    {
        slider.value = current / max;
    }

}

