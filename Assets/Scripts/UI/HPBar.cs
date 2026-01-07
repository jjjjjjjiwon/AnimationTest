using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;

    public void SetHP(float current, float max)
    {
        slider.value = current / max;
    }
}