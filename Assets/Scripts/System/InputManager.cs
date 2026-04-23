using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    // --- [시스템 / UI 관련] ---
    // UI를 열고 닫는 키 (F)
    public static bool GetUIOpenInput() => Input.GetKeyDown(KeyCode.F);
    
    // 창을 닫거나 취소하는 키 (Escape)
    public static bool GetExitInput() => Input.GetKeyDown(KeyCode.Escape);

    // --- [플레이어 액션 관련] ---
    //  // PlayerController 공격 방식 전환
    public static bool GetModeToggleInput() => Input.GetKeyDown(KeyCode.Tab);

     // PlayerController 회피
    public static bool GetDodgeInput() => Input.GetKeyDown(KeyCode.Space);

    // 물리 공격 타입 판정 (Left/Right Click, Q, E, R)

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
}
