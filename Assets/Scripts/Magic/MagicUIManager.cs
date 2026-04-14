using UnityEngine;
using TMPro; // TextMeshPro 사용 시
using UnityEngine.UI;

public class MagicUIManager : MonoBehaviour
{
    public TextMeshProUGUI modeText;
    public Image modeIcon;
    public Color magicColor = Color.cyan;
    public Color meleeColor = Color.white;

    public void UpdateModeUI(PlayerController.PlayerMode mode)
    {
        if (mode == PlayerController.PlayerMode.Magic)
        {
            modeText.text = "MAGIC MODE";
            modeText.color = magicColor;
            modeIcon.color = magicColor;
        }
        else
        {
            modeText.text = "MELEE MODE";
            modeText.color = meleeColor;
            modeIcon.color = meleeColor;
        }
    }
}