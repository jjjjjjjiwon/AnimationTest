using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class InputManager
{

#region System

    // 포탈 사용
    public static bool UsePortal() => Input.GetKeyDown(KeyCode.F);

    // 카메라 열기/막기
    public static bool CameraController() => Input.GetKeyDown(KeyCode.Escape);

    // PlayerController 공격 방식 전환
    public static bool GetModeToggleInput() => Input.GetKeyDown(KeyCode.Tab);


#endregion

#region UI

    // 소켓 열기/닫기 (F)
    public static bool GetUIOpenInput() => Input.GetKeyDown(KeyCode.F);
    
    // 창을 닫거나 취소하는 키 (Escape)
    public static bool GetExitInput() => Input.GetKeyDown(KeyCode.Escape);

    // 플레이어 정보
    public static bool PlayerInfoUI() => Input.GetKeyDown(KeyCode.I);

    // 콤보 미리보기
    public static bool ComboPreview() => Input.GetKeyDown(KeyCode.Tab);

#endregion

#region PlayerController

    // PlayerController 회피
    public static bool GetDodgeInput() => Input.GetKeyDown(KeyCode.Space);

    // PlayerController 물리 공격 키
    public static InputTypes GetMeleeInputType()
    {
        if (Input.GetMouseButtonDown(0)) return InputTypes.LeftClick;
        if (Input.GetMouseButtonDown(1)) return InputTypes.RightClick;
        if (Input.GetKeyDown(KeyCode.Q)) return InputTypes.QKey;
        if (Input.GetKeyDown(KeyCode.E)) return InputTypes.EKey;
        if (Input.GetKeyDown(KeyCode.R)) return InputTypes.RKey;
        return InputTypes.None;
    }

    // PlayerController 마법 공격 키
    public static int GetMagicSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) return 0;
        if (Input.GetKeyDown(KeyCode.E)) return 1;
        if (Input.GetKeyDown(KeyCode.R)) return 2;
        if (Input.GetKeyDown(KeyCode.T)) return 3;
        if (Input.GetKeyDown(KeyCode.LeftShift)) return 4;
        if (Input.GetKeyDown(KeyCode.LeftControl)) return 5;
        return -1; // 아무것도 안 눌림
    } 
#endregion

#region 테스트용 키

    // G 키: 골드 추가 테스트
    public static bool GetDebugAddGold() => Input.GetKeyDown(KeyCode.G);

    // C 키: 스탯 출력 테스트
    public static bool StatOutput() => Input.GetKeyDown(KeyCode.C);

    // L 키: 적 스턴
    public static bool EnemyStun() => Input.GetKeyDown(KeyCode.L);

    // Y키: 플레이어 데미지
    public static bool PlayerDamage() => Input.GetKeyDown(KeyCode.Y);

    // Z키: 플레이어 추가 스탯
    public static bool PlayerAddStat() => Input.GetKeyDown(KeyCode.Z);



#endregion
}
