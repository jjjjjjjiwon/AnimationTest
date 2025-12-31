using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboSystem
{
    private List<ComboData> allCombos;
    
    // 현재 가능한 콤보 후보들!
    private List<ComboData> possibleCombos;
    
    private ComboData currentCombo;  // 확정된 콤보 (나중에)
    private int currentStep = -1;
    
    private int perfectCount = 0;
    private bool isPerfectWindow = false;
    
    public ComboSystem(List<ComboData> combos)
    {
        allCombos = combos;
        possibleCombos = new List<ComboData>();
    }
    
    // ========== 콤보 시작 ==========
    
    public bool StartCombo(InputType firstInput)
    {
        // 첫 입력으로 시작 가능한 콤보들 찾기
        possibleCombos.Clear();
        
        foreach (var combo in allCombos)
        {
            if (combo.steps.Length > 0 && combo.steps[0].inputType == firstInput)
            {
                possibleCombos.Add(combo);  // 후보 추가!
            }
        }
        
        if (possibleCombos.Count == 0)
        {
            Debug.LogWarning($"입력 {firstInput}로 시작하는 콤보 없음!");
            return false;
        }
        
        // 아직 콤보 확정 안 함!
        currentCombo = null;
        currentStep = 0;
        perfectCount = 0;
        isPerfectWindow = false;
        
        Debug.Log($"가능한 콤보 {possibleCombos.Count}개 발견!");
        return true;
    }
    
    // ========== 입력 처리 ==========
    
    public bool ProcessInput(InputType input)
    {
        int nextStep = currentStep + 1;
        
        // 다음 단계 범위 체크
        if (nextStep >= 5)  // 5타 넘으면
        {
            Debug.LogWarning("이미 콤보 마지막 타!");
            return false;
        }
        
        // 후보들 중 입력과 맞는 콤보 필터링
        var matchingCombos = possibleCombos.Where(combo =>
            combo.steps.Length > nextStep &&
            combo.steps[nextStep].inputType == input
        ).ToList();
        
        if (matchingCombos.Count == 0)
        {
            // 맞는 콤보 없음! 실패!
            Debug.Log($"틀린 입력! 단계 {nextStep + 1}에서 {input} 불가능");
            return false;
        }
        
        // 성공! 후보 업데이트
        possibleCombos = matchingCombos;
        
        // Perfect 체크
        if (isPerfectWindow)
        {
            perfectCount++;
            Debug.Log($"Perfect! (총 {perfectCount}개)");
        }
        
        // 다음 단계로
        currentStep = nextStep;
        isPerfectWindow = false;
        
        Debug.Log($"콤보 진행: {currentStep + 1}타 (후보 {possibleCombos.Count}개)");
        
        // 후보가 1개면 확정!
        if (possibleCombos.Count == 1)
        {
            currentCombo = possibleCombos[0];
            Debug.Log($"콤보 확정: {currentCombo.comboName}");
        }
        
        return true;
    }
    
    // ========== Perfect 타이밍 ==========
    
    public void OnPerfectWindowStart()
    {
        isPerfectWindow = true;
    }
    
    public void OnPerfectWindowEnd()
    {
        isPerfectWindow = false;
    }
    
    public bool IsPerfectWindow()
    {
        return isPerfectWindow;
    }
    
    // ========== 정보 반환 ==========
    
    public string GetCurrentAnimation()
    {
        if (currentStep < 0)
            return "";
        
        // 확정된 콤보 있으면 그거 사용
        if (currentCombo != null)
        {
            return currentCombo.steps[currentStep].animationName;
        }
        
        // 아직 확정 안 됐으면 첫 번째 후보 사용
        if (possibleCombos.Count > 0)
        {
            return possibleCombos[0].steps[currentStep].animationName;
        }
        
        return "";
    }
    
    public float GetCurrentDamage()
    {
        if (currentStep < 0)
            return 0f;
        
        ComboData combo = currentCombo ?? possibleCombos[0];
        float baseDamage = combo.steps[currentStep].damage;
        
        return baseDamage;
    }
    
    public float GetCurrentStunDuration()
    {
        if (currentStep < 0)
            return 0f;
        
        ComboData combo = currentCombo ?? possibleCombos[0];
        float baseStun = combo.steps[currentStep].stunDuration;
        
        return baseStun;
    }
    
    public bool IsComboComplete()
    {
        return currentStep >= 4;  // 5타 완료 (0~4)
    }
    
    public int GetCurrentStep()
    {
        return currentStep;
    }
    
    public int GetPerfectCount()
    {
        return perfectCount;
    }
    
    public string GetFinisherAnimation()
    {
        // 확정된 콤보의 피니셔
        ComboData combo = currentCombo ?? possibleCombos[0];
        return combo.finisher.animationName;
    }
    
    public float GetFinisherDamage()
    {
        ComboData combo = currentCombo ?? possibleCombos[0];
        
        float baseDamage = combo.finisher.baseDamage;
        float bonusDamage = combo.finisher.damagePerPerfect * perfectCount;
        
        return baseDamage + bonusDamage;
    }
    
    public void ResetCombo()
    {
        currentCombo = null;
        possibleCombos.Clear();
        currentStep = -1;
        perfectCount = 0;
        isPerfectWindow = false;
    }
}