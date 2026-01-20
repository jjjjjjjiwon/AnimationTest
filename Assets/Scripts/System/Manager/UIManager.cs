using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI 생명주기 관리자
/// - Flags Enum 기반으로 씬 타입에 따라 UI 자동 표시/숨김
/// - DontDestroyOnLoad로 씬 전환 시에도 유지
/// </summary>
public class UIManager : MonoBehaviour
{
    // ========================================
    // Singleton
    // ========================================
    
    public static UIManager Instance { get; private set; }
    
    // ========================================
    // Scene Types (Flags)
    // ========================================
    
    [System.Flags]
    public enum SceneType
    {
        None = 0,
        Lobby = 1 << 0,      // 1 (0001)
        Stage = 1 << 1,      // 2 (0010)
        BossStage = 1 << 2,  // 4 (0100)
        
        // 조합
        Game = Stage | BossStage,           // 6 (0110) - 모든 게임 씬
        All = Lobby | Stage | BossStage     // 7 (0111) - 모든 씬
    }
    
    // ========================================
    // UI Elements
    // ========================================
    
    [System.Serializable]
    public class UIElement
    {
        public string name;             // UI 이름 (디버그용)
        public GameObject uiObject;     // UI GameObject
        public SceneType sceneType;     // 표시될 씬 타입 (Flags)
    }
    
    [Header("UI Elements")]
    [SerializeField] private UIElement[] uiElements;
    
    // 현재 씬 타입
    private SceneType currentSceneType = SceneType.Lobby;
    
    // ========================================
    // Initialization
    // ========================================
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("[UIManager] 생성 완료");
        }
        else
        {
            Debug.LogWarning("[UIManager] 중복 인스턴스 파괴");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 초기 씬 타입 설정
        UpdateSceneType(SceneManager.GetActiveScene().name);
        
        // UI 표시 업데이트
        UpdateUIVisibility();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    // ========================================
    // Scene Management
    // ========================================
    
    /// <summary>씬 로드 시 호출</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIManager] 씬 로드됨: {scene.name}");
        
        // 씬 타입 업데이트
        UpdateSceneType(scene.name);
        
        // UI 표시 업데이트
        UpdateUIVisibility();
    }
    
    /// <summary>씬 이름으로 씬 타입 판단</summary>
    private void UpdateSceneType(string sceneName)
    {
        sceneName = sceneName.ToLower();
        
        if (sceneName.Contains("lobby"))
        {
            currentSceneType = SceneType.Lobby;
        }
        else if (sceneName.Contains("boss"))
        {
            currentSceneType = SceneType.BossStage;
        }
        else if (sceneName.Contains("stage") || sceneName.Contains("cave") || sceneName.Contains("volcano"))
        {
            currentSceneType = SceneType.Stage;
        }
        else
        {
            currentSceneType = SceneType.Lobby;
        }
        
        Debug.Log($"[UIManager] 씬 타입: {currentSceneType}");
    }
    
    /// <summary>현재 씬 타입에 맞게 UI 표시/숨김</summary>
    private void UpdateUIVisibility()
    {
        foreach (var element in uiElements)
        {
            if (element.uiObject == null)
                continue;
            
            // Flags Enum 비트 연산으로 체크
            bool shouldShow = (element.sceneType & currentSceneType) != 0;
            element.uiObject.SetActive(shouldShow);
            
            Debug.Log($"[UIManager] {element.name}: {(shouldShow ? "표시" : "숨김")} (SceneType: {element.sceneType})");
        }
    }
    
    // ========================================
    // Public API - 특정 UI 표시 (수동)
    // ========================================
    
    /// <summary>보상 UI 표시</summary>
    public void ShowRewardUI(StageData stage, bool isBoss)
    {
        // ⭐ UI Elements 배열에서 찾기
        RewardUI rewardUI = GetUI<RewardUI>();
        
        if (rewardUI == null)
        {
            Debug.LogError("[UIManager] RewardUI를 찾을 수 없습니다!");
            return;
        }
        
        rewardUI.Setup(stage, isBoss);
        rewardUI.gameObject.SetActive(true);
        
        Debug.Log($"[UIManager] RewardUI 표시 - {stage.stageName}");
    }
    
    /// <summary>보스 강화 UI 표시</summary>
    public void ShowBossUpgradeUI()
    {
        // ⭐ UI Elements 배열에서 찾기
        BossUpgradeUI bossUpgradeUI = GetUI<BossUpgradeUI>();
        
        if (bossUpgradeUI == null)
        {
            Debug.LogError("[UIManager] BossUpgradeUI를 찾을 수 없습니다!");
            return;
        }
        
        bossUpgradeUI.Setup();
        bossUpgradeUI.gameObject.SetActive(true);
        
        Debug.Log("[UIManager] BossUpgradeUI 표시");
    }
    
    /// <summary>플레이어 정보 UI 토글</summary>
    public void TogglePlayerInfoUI()
    {
        PlayerInfoUI playerInfoUI = GetUI<PlayerInfoUI>();
        
        if (playerInfoUI != null)
        {
            playerInfoUI.gameObject.SetActive(!playerInfoUI.gameObject.activeSelf);
        }
    }
    
    // ========================================
    // Utility
    // ========================================
    
    /// <summary>
    /// UI Elements 배열에서 특정 타입의 UI 찾기
    /// </summary>
    public T GetUI<T>() where T : MonoBehaviour
    {
        foreach (var element in uiElements)
        {
            if (element.uiObject == null)
                continue;
            
            T component = element.uiObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }
        
        Debug.LogWarning($"[UIManager] {typeof(T).Name}을 찾을 수 없습니다!");
        return null;
    }
    
    /// <summary>
    /// 이름으로 UI 찾기 (대안)
    /// </summary>
    public GameObject GetUIByName(string uiName)
    {
        foreach (var element in uiElements)
        {
            if (element.name == uiName)
            {
                return element.uiObject;
            }
        }
        
        Debug.LogWarning($"[UIManager] '{uiName}' UI를 찾을 수 없습니다!");
        return null;
    }
    
    /// <summary>현재 씬 타입 가져오기</summary>
    public SceneType GetCurrentSceneType()
    {
        return currentSceneType;
    }
}