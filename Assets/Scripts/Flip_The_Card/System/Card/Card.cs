using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 카드 오브젝트 (완성 버전)
/// 마우스 호버, 클릭, 뒤집기 애니메이션을 처리
/// </summary>
public class Card : MonoBehaviour
{
    [Header("Card Sides")]
    [SerializeField] private GameObject frontSide;  // 카드 앞면 (스테이지 정보 표시)
    [SerializeField] private GameObject backSide;   // 카드 뒷면 (초기 상태)

    [Header("Front Side UI")]
    [SerializeField] private MeshRenderer cardImageRenderer;  // 스테이지 이미지 표시용
    [SerializeField] private TextMeshPro nameText;            // 스테이지 이름
    [SerializeField] private TextMeshPro descText;            // 스테이지 설명

    [Header("Visual Effects")]
    [SerializeField] private Material normalMaterial;      // 기본 Material
    [SerializeField] private Material highlightMaterial;   // 호버 시 강조 Material

    [Header("Animation Settings")]
    [SerializeField] private float flipDuration = 0.5f;    // 뒤집기 애니메이션 시간

    private MeshRenderer cardRenderer;   // 카드 본체 Renderer (Material 변경용)
    private StageData stageData;         // 할당받은 스테이지 데이터
    private int cardIndex;               // 내가 몇 번째 카드인지 (GameData.selectedStages의 인덱스)
    private bool isSelected = false;     // 첫 번째 클릭 완료 여부
    private bool isFlipped = false;      // 카드가 뒤집혔는지 여부
    private bool canInteract = true;     // 상호작용 가능 여부 (다른 카드 선택 시 false)

    [SerializeField] private TextMeshPro forntName;

    void Awake()
    {
        // 컴포넌트 캐싱
        cardRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // 초기 상태: 뒷면만 보임
        ShowBack();
    }


    /// <summary>
    /// 카드 초기화
    /// CardManager가 생성 직후 호출
    /// </summary>
    /// <param name="data">할당할 스테이지 데이터</param>
    /// <param name="index">이 카드의 인덱스 (GameData.selectedStages의 몇 번째)</param>
    public void Initialize(StageData data, int index)
    {
        stageData = data;
        cardIndex = index;  // 인덱스 저장
        SetupFrontSide();

        Debug.Log($"[Card] 초기화: {data.stage_Name} (인덱스: {index})");
    }


    /// <summary>
    /// 상호작용 비활성화
    /// 다른 카드 선택 시 호출
    /// </summary>
    public void DisableInteraction()
    {
        canInteract = false;

        // 선택 안하면
        if (normalMaterial != null && cardRenderer != null)
        {
            cardRenderer.material = normalMaterial;
        }
    }


    /// <summary>
    /// StageData 정보를 앞면 UI에 세팅
    /// </summary>
void SetupFrontSide()
{
    Debug.Log("[Card] SetupFrontSide 진입");

    if (stageData == null) 
    {
        Debug.LogError("[Card] stageData가 Null입니다! 초기화 실패.");
        return;
    }

    // 1. 텍스트 설정
    if (nameText != null)
    {
        nameText.text = stageData.stage_Name;
        nameText.color = Color.white; // 검은색 방지
        Debug.Log($"[Card] 이름 설정 완료: {stageData.stage_Name}");
    }

    // 2. 이미지(Sprite) 설정
    if (cardImageRenderer != null && stageData.stage_Icon != null)
    {
        // Sprite의 원본 Texture2D 추출
        Texture2D texture = stageData.stage_Icon.texture;
        
        // Material의 인스턴스를 직접 참조하여 텍스처 변경
        // URP라면 "_BaseMap", 빌트인이라면 "_MainTex"를 주로 사용합니다.
        cardImageRenderer.material.mainTexture = texture;
        
        // 만약 안 나온다면 아래 주석을 해제해서 시도해보세요 (URP 대응)
        // cardImageRenderer.material.SetTexture("_BaseMap", texture);

        Debug.Log($"[Card] 이미지 설정 완료: {stageData.stage_Icon.name} (Size: {texture.width}x{texture.height})");
    }
    else
    {
        Debug.LogWarning($"[Card] 이미지 컴포넌트나 아이콘 데이터가 누락되었습니다. Renderer: {cardImageRenderer != null}, Icon: {stageData.stage_Icon != null}");
    }
}

    /// <summary>
    /// 앞면 표시
    /// </summary>
    void ShowFront()
    {
        if (frontSide != null) frontSide.SetActive(true);
        if (backSide != null) backSide.SetActive(false);
    }

    /// <summary>
    /// 뒷면 표시
    /// </summary>
    void ShowBack()
    {
        if (frontSide != null) frontSide.SetActive(false);
        if (backSide != null) backSide.SetActive(true);
    }

    /// <summary>
    /// 첫 번째 클릭 처리
    /// </summary>
    void FirstClick()
    {
        isSelected = true;

        // 뒤집기
        if (!isFlipped)
        {
            StartCoroutine(FlipCard());
        }

        // CardManager에 알림
        CardManager cardManager = FindObjectOfType<CardManager>();
        if (cardManager != null)
        {
            cardManager.OnCardSelected(this);
        }

        Debug.Log($"[Card] 선택됨: {stageData.stage_Name}");
        transform.localScale = new Vector3(2f, 2f, 2f);
    }

    /// <summary>
    /// 두 번째 클릭 처리
    /// 스테이지 시작 및 묘지로 이동
    /// </summary>
    void SecondClick()
    {
        Debug.Log($"[Card] 스테이지 시작: {stageData.stage_Name}");

        // 1) 묘지 이동 + 다음 층 (GameData null 방지)
        if (GameData.Instance != null)
        {
            GameData.Instance.MoveToGraveyard(cardIndex);
            GameData.Instance.NextFloor();
        }
        else
        {
            Debug.LogWarning("[Card] GameData.Instance가 null입니다. (묘지/층 진행 스킵)");
        }

        // 2) StageManager로 스테이지 시작 (FindObjectOfType 대신 Instance 사용 권장)
        if (StageManager.Instance != null)
        {
            StageManager.Instance.StartStage(stageData); // ✅ LoadStage가 아니라 StartStage
        }
        else
        {
            Debug.LogError("[Card] StageManager.Instance를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 마우스 호버 시작
    /// </summary>
    void OnMouseEnter()
    {
        if (!canInteract) return;

        if (highlightMaterial != null && cardRenderer != null)
        {
            cardRenderer.material = highlightMaterial;
        }
    }

    /// <summary>
    /// 마우스 호버 종료
    /// </summary>
    void OnMouseExit()
    {
        if (!canInteract) return;

        if (!isSelected && normalMaterial != null && cardRenderer != null)
        {
            cardRenderer.material = normalMaterial;
        }
    }


    /// <summary>
    /// 마우스 클릭 시
    /// Unity의 OnMouseDown 이벤트
    /// </summary>
    void OnMouseDown()
    {
        // 🔥 [추가] 마우스가 UI(소켓, 버튼 등) 위에 있다면 카드 클릭 로직을 실행하지 않음
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 상호작용 불가능하거나 데이터가 없으면 무시
        if (!canInteract || stageData == null) return;

        if (!isSelected)
        {
            // 첫 번째 클릭: 카드 선택
            FirstClick();
        }
        else
        {
            // 두 번째 클릭: 스테이지 시작
            SecondClick();
        }
    }


    /// <summary>
    /// 카드 뒤집기 애니메이션
    /// Y축 기준 180도 회전
    /// </summary>
    IEnumerator FlipCard()
    {
        float elapsed = 0f;  // 경과 시간
        Quaternion startRotation = transform.rotation;  // 시작 회전
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 180, 0);  // 목표 회전 (180도)

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;  // 시간 누적

            // 정규화된 시간 (0~1 비율)
            // 프레임마다 deltaTime이 다르므로 비율로 변환
            float t = elapsed / flipDuration;

            // 회전 보간 (Lerp: 선형 보간)
            // 시작 회전에서 끝 회전까지 부드럽게 변환
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            // 애니메이션 중간 지점(50%)에서 앞/뒷면 전환
            // 이 시점에 전환해야 자연스러움
            if (t >= 0.5f && !isFlipped)
            {
                ShowFront();
                isFlipped = true;
            }

            yield return null;  // 다음 프레임까지 대기
        }

        // 최종 회전 확정 (정확한 180도)
        transform.rotation = endRotation;
    }

}