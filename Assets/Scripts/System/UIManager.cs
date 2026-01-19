using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// 씬 타입 정의
/// </summary>
public enum SceneType
{
    None = 0,
    MainMenu = 1,      // 메인 메뉴
    Lobby = 2,         // 로비 (허브)
    Stage = 4,         // 일반 스테이지
    BossStage = 8,     // 보스 스테이지
    
    // 조합도 가능 (Flags처럼 사용)
    AllGame = Stage | BossStage,  // 모든 게임 씬
    All = MainMenu | Lobby | Stage | BossStage  // 모든 씬
}

/// <summary>
/// UI가 표시될 씬을 정의하는 Attribute
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
public class SceneVisibilityAttribute : System.Attribute
{
    public SceneType VisibleInScenes { get; private set; }
    
    public SceneVisibilityAttribute(SceneType scenes)
    {
        VisibleInScenes = scenes;
    }
}

/// <summary>
/// UI 정보를 담는 클래스
/// </summary>
[System.Serializable]
public class UIElement
{
    [Tooltip("UI 이름 (식별용)")]
    public string name;
    
    [Tooltip("UI GameObject")]
    public GameObject uiObject;
    
    [Tooltip("어떤 씬에서 보일지")]
    public SceneType visibleInScenes;
    
    [Tooltip("초기 상태 (활성화/비활성화)")]
    public bool startActive = false;
}

/// <summary>
/// Enum 기반 UI 관리자
/// 씬 타입에 따라 자동으로 UI를 표시/숨김
/// </summary>
public class UIManager : MonoBehaviour
{
    // ========================================
    // 싱글톤
    // ========================================
    
    public static UIManager Instance { get; private set; }
    
    // ========================================
    // 씬 타입 설정
    // ========================================
    
    [Header("Scene Configuration")]
    [Tooltip("씬 이름 → 씬 타입 매핑")]
    [SerializeField] private List<SceneMapping> sceneMappings = new List<SceneMapping>()
    {
        new SceneMapping("MainMenu", SceneType.MainMenu),
        new SceneMapping("Lobby", SceneType.Lobby),
        new SceneMapping("Stage", SceneType.Stage),
        new SceneMapping("BossStage", SceneType.BossStage)
    };
    
    [System.Serializable]
    public class SceneMapping
    {
        public string sceneNamePattern;  // 씬 이름 (Contains로 검색)
        public SceneType sceneType;
        
        public SceneMapping(string pattern, SceneType type)
        {
            sceneNamePattern = pattern;
            sceneType = type;
        }
    }
    
    // ========================================
    // UI 등록
    // ========================================
    
    [Header("UI Elements")]
    [Tooltip("관리할 모든 UI 리스트")]
    [SerializeField] private List<UIElement> uiElements = new List<UIElement>();
    
    // 빠른 접근을 위한 딕셔너리
    private Dictionary<string, UIElement> uiDictionary = new Dictionary<string, UIElement>();
    
    // 현재 씬 타입
    private SceneType currentSceneType = SceneType.None;
    
    // ========================================
    // 초기화
    // ========================================
    
    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[UIManager] DontDestroyOnLoad 적용");
        }
        else
        {
            Debug.Log("[UIManager] 중복 인스턴스 파괴");
            Destroy(gameObject);
            return;
        }
        
        // UI 딕셔너리 초기화
        InitializeUIDictionary();
        
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void Start()
    {
            foreach (var ui in uiElements)
    {
        if (ui.uiObject != null)
            ui.uiObject.SetActive(ui.startActive);
    }

        // 현재 씬에 맞게 UI 표시
        UpdateUIForCurrentScene();
    }
    
    /// <summary>
    /// UI 딕셔너리 초기화
    /// </summary>
    private void InitializeUIDictionary()
    {
        uiDictionary.Clear();
        
        foreach (var ui in uiElements)
        {
            if (ui.uiObject != null && !string.IsNullOrEmpty(ui.name))
            {
                uiDictionary[ui.name] = ui;
            }
        }
        
        Debug.Log($"[UIManager] {uiDictionary.Count}개 UI 등록 완료");
    }
    
    // ========================================
    // 씬 전환 시 UI 자동 업데이트
    // ========================================
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIManager] 씬 로드됨: {scene.name}");
        
        // 씬 타입 결정
        currentSceneType = GetSceneType(scene.name);
        Debug.Log($"[UIManager] 씬 타입: {currentSceneType}");
        
        // UI 표시 업데이트
        UpdateUIForCurrentScene();
    }
    
    /// <summary>
    /// 씬 이름으로 씬 타입 결정
    /// </summary>
    private SceneType GetSceneType(string sceneName)
    {
        foreach (var mapping in sceneMappings)
        {
            if (sceneName.Contains(mapping.sceneNamePattern))
            {
                return mapping.sceneType;
            }
        }
        
        Debug.LogWarning($"[UIManager] 씬 '{sceneName}'의 타입을 찾을 수 없습니다. None 반환");
        return SceneType.None;
    }
    
    /// <summary>
    /// 현재 씬에 맞게 UI 표시 업데이트
    /// </summary>
    private void UpdateUIForCurrentScene()
    {
        Debug.Log($"[UIManager] UI 업데이트 시작 - 현재 씬: {currentSceneType}");
        
        int showCount = 0;
        int hideCount = 0;
        
        foreach (var ui in uiElements)
        {
            if (ui.uiObject == null) continue;
            
            // 현재 씬에서 보여야 하는지 확인
            bool shouldShow = (ui.visibleInScenes & currentSceneType) != 0;
            
            if (shouldShow)
            {
                ui.uiObject.SetActive(ui.startActive);
                showCount++;
                Debug.Log($"  ✅ {ui.name}: 활성화 가능 (초기: {ui.startActive})");
            }
            else
            {
                ui.uiObject.SetActive(false);
                hideCount++;
                Debug.Log($"  ❌ {ui.name}: 비활성화");
            }
        }
        
        Debug.Log($"[UIManager] UI 업데이트 완료 - 활성: {showCount}, 비활성: {hideCount}");
    }
    
    // ========================================
    // UI 제어 메서드
    // ========================================
    
    /// <summary>
    /// UI 표시 (이름으로 접근)
    /// </summary>
    public void ShowUI(string uiName)
    {
        if (uiDictionary.TryGetValue(uiName, out UIElement ui))
        {
            // 현재 씬에서 표시 가능한지 확인
            if ((ui.visibleInScenes & currentSceneType) != 0)
            {
                ui.uiObject.SetActive(true);
                Debug.Log($"[UIManager] UI 표시: {uiName}");
            }
            else
            {
                Debug.LogWarning($"[UIManager] '{uiName}'은(는) 현재 씬({currentSceneType})에서 표시할 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError($"[UIManager] UI를 찾을 수 없습니다: {uiName}");
        }
    }
    
    /// <summary>
    /// UI 숨기기 (이름으로 접근)
    /// </summary>
    public void HideUI(string uiName)
    {
        if (uiDictionary.TryGetValue(uiName, out UIElement ui))
        {
            ui.uiObject.SetActive(false);
            Debug.Log($"[UIManager] UI 숨김: {uiName}");
        }
        else
        {
            Debug.LogError($"[UIManager] UI를 찾을 수 없습니다: {uiName}");
        }
    }
    
    /// <summary>
    /// UI 토글 (이름으로 접근)
    /// </summary>
    public void ToggleUI(string uiName)
    {
        if (uiDictionary.TryGetValue(uiName, out UIElement ui))
        {
            if (ui.uiObject.activeSelf)
            {
                HideUI(uiName);
            }
            else
            {
                ShowUI(uiName);
            }
        }
    }
    
    /// <summary>
    /// UI가 활성화되어 있는지 확인
    /// </summary>
    public bool IsUIActive(string uiName)
    {
        if (uiDictionary.TryGetValue(uiName, out UIElement ui))
        {
            return ui.uiObject.activeSelf;
        }
        return false;
    }
    
    /// <summary>
    /// 현재 씬에서 특정 UI가 표시 가능한지 확인
    /// </summary>
    public bool CanShowUI(string uiName)
    {
        if (uiDictionary.TryGetValue(uiName, out UIElement ui))
        {
            return (ui.visibleInScenes & currentSceneType) != 0;
        }
        return false;
    }
    
    // ========================================
    // 특정 UI 빠른 접근 (Optional)
    // ========================================
    
    public void ShowRewardUI() => ShowUI("RewardUI");
    public void HideRewardUI() => HideUI("RewardUI");
    
    public void ShowBossUpgradeUI() => ShowUI("BossUpgradeUI");
    public void HideBossUpgradeUI() => HideUI("BossUpgradeUI");
    
    public void ShowPauseUI() => ShowUI("PauseUI");
    public void HidePauseUI() => HideUI("PauseUI");
    
    public void ShowStageSelectUI() => ShowUI("StageSelectUI");
    public void HideStageSelectUI() => HideUI("StageSelectUI");
    
    public void ShowStatInvestUI() => ShowUI("StatInvestUI");
    public void HideStatInvestUI() => HideUI("StatInvestUI");
    
    public void ShowSocketManagerUI() => ShowUI("SocketManagerUI");
    public void HideSocketManagerUI() => HideUI("SocketManagerUI");
    
    public void ShowBossHealthBar() => ShowUI("BossHealthBar");
    public void HideBossHealthBar() => HideUI("BossHealthBar");
    
    // ========================================
    // 유틸리티
    // ========================================
    
    /// <summary>
    /// 현재 씬 타입 가져오기
    /// </summary>
    public SceneType GetCurrentSceneType()
    {
        return currentSceneType;
    }
    
    /// <summary>
    /// 모든 UI 숨기기
    /// </summary>
    public void HideAllUI()
    {
        foreach (var ui in uiElements)
        {
            if (ui.uiObject != null)
            {
                ui.uiObject.SetActive(false);
            }
        }
    }
}