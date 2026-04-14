using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    // 1. 타입을 PlayerSkillData로 변경
    private PlayerSkillData skillData;
    private SocketManagerUI manager;

    private MagicData magicData; // 마법 데이터 저장을 위한 변수 추가
    private bool isMagic = false; // 현재 슬롯이 마법용인지 구분

    // 기존 무기 스킬용
    public void Initialize(PlayerSkillData skill, SocketManagerUI managerUI)
    {
        skillData = skill;
        manager = managerUI;
        isMagic = false;

        SetupButton();
        UpdateUI();
    }

    // ⭐ 새롭게 추가: 마법 주문용 오버로딩
    public void Initialize(MagicData magic, SocketManagerUI managerUI)
    {
        manager = managerUI;
        magicData = magic; // 데이터를 저장해야 OnClick에서 쓸 수 있음
        isMagic = true;    // 마법 모드임을 표시

        // 1. 버튼 세팅 (클릭 이벤트 연결)
        SetupButton();

        // 2. 아이콘 로드 및 표시
        if (!string.IsNullOrEmpty(magic.Icon_Path))
        {
            Sprite icon = Resources.Load<Sprite>(magic.Icon_Path);
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.color = Color.white;
                iconImage.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[마법] 아이콘 로드 실패: {magic.Icon_Path}");
                iconImage.enabled = false;
            }
        }
    }

    public void UpdateUI()
    {
        if (skillData != null && !string.IsNullOrEmpty(skillData.skill_Icon_Path))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(skillData.skill_Icon_Path);

            if (loadedSprite != null)
            {
                iconImage.sprite = loadedSprite;
                Debug.Log($"[성공] {skillData.skill_Name} 아이콘 로드 완료! ========================================================");
            }
            else
            {
                // 이 로그가 콘솔에 찍힌다면 경로가 틀린 것입니다.
                Debug.LogError($"[실패] 경로를 찾을 수 없음: Resources/{skillData.skill_Icon_Path}       +++===================================");
            }
        }
    }

    private void SetupButton()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (manager == null) return;

        if (isMagic) // 마법 모드일 때
        {
            if (magicData != null)
                manager.EquipSkillToSelectedSocket(magicData);
        }
        else // 무기 스킬 모드일 때
        {
            if (skillData != null)
                manager.EquipSkillToSelectedSocket(skillData);
        }
    }
}