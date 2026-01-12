using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 메인 메뉴 UI
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(GoToLobby);
        }
    }

    /// <summary>
    /// 로비로 이동
    /// </summary>
    void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}