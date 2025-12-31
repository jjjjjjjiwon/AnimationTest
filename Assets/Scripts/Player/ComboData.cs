using UnityEngine;

[CreateAssetMenu(fileName = "ComboData", menuName = "Player/Combo Data")]
public class ComboData : ScriptableObject
{
    [Header("Combo Info")]
    [Tooltip("콤보 이름")]
    public string comboName = "Combo A";
    
    [Tooltip("콤보 단계들 (5타)")]
    public ComboStep[] steps;
    
    [Header("Finisher")]
    [Tooltip("피니셔 데이터")]
    public FinisherData finisher;
}

[System.Serializable]
public class ComboStep
{
    [Header("Input")]
    [Tooltip("요구되는 입력")]
    public InputType inputType;
    
    [Header("Animation")]
    [Tooltip("애니메이션 State 이름")]
    public string animationName;
    
    [Header("Damage")]
    [Tooltip("기본 데미지")]
    public float damage = 10f;
    
    [Tooltip("적 스턴 시간 (초)")]
    public float stunDuration = 1f;
}

[System.Serializable]
public class FinisherData
{
    [Header("Animation")]
    [Tooltip("피니셔 애니메이션 이름")]
    public string animationName = "Finisher";
    
    [Header("Damage")]
    [Tooltip("기본 피니셔 데미지")]
    public float baseDamage = 50f;
    
    [Tooltip("Perfect 1개당 추가 데미지")]
    public float damagePerPerfect = 10f;
}

public enum InputType
{
    LeftClick,
    RightClick,
}