using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConboPreview : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Transform container;

    [SerializeField] private GameObject socketSlotPrefab;

    private static bool isUIOpen = false;
    public static bool IsUIOpen => isUIOpen;

    private List<SocketSlotUI> socketSlotUIs = new List<SocketSlotUI>();


    void Start()
    {
        // UI 초기화
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        isUIOpen = false;
    }
    void Update()
    {
        if (PlayerInfoUI.IsUIOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab ______________________________________________________________________");
            ToggleUI();
        }
    }


#region Open
    public void ToggleUI()
    {
        if (isUIOpen)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    public void OpenUI()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null && !pc.CanOpenUI())
        {
            Debug.Log("Tab을 열수 없습니다");

            return;
        }

        isUIOpen = true;

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        RefreshPreviewUI();

        Debug.Log("[정보창] 열림");
    }

    public void CloseUI()
    {
        isUIOpen = false;
        uiPanel.SetActive(false);
    }

#endregion


private void RefreshPreviewUI()
{
    // 기존 UI 제거
    foreach (var ui in socketSlotUIs)
    {
        if (ui != null)
            Destroy(ui.gameObject);
    }
    socketSlotUIs.Clear();

    // SocketManager 가져오기
    if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null)
    {
        Debug.LogError("[ComboPreview] SocketManager 없음");
        return;
    }

    SocketManager socketManager = RuntimeManager.Instance.socketManager;
    List<ComboSocket> sockets = socketManager.GetAllSockets();

    Debug.Log($"[ComboPreview] 소켓 개수: {sockets.Count}");

    // 소켓 UI 생성
    for (int i = 0; i < sockets.Count; i++)
    {
        GameObject obj = Instantiate(socketSlotPrefab, container);
        SocketSlotUI ui = obj.GetComponent<SocketSlotUI>();

        // 🔹 읽기 전용이므로 managerUI는 null
        ui.Initialize(i, sockets[i], null);

        socketSlotUIs.Add(ui);
    }
}


}
