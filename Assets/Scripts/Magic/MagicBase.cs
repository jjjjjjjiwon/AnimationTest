using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MagicBase : MonoBehaviour
{
    public Action OnLogic; 
    public Transform caster;
    public bool isLaunched = false;
    private Dictionary<string, float> _params = new Dictionary<string, float>();

    [Header("Stat")]     public float moveSpeed = 0f;
    public float rotationWeight = 0f; // 나선을 위한 계수

    [Header("spawn")]
    public float followDistance = 0f;   // 플레이어와의 간격
    public float floatingHeight = 1.5f; // 바닥에서 1.5m 


    [Header("Rules")]
    public int maxCommandLimit = 5;    // 최대 받을 수 있는 단어 명령 수
    private int _currentCommandCount = 0; // 현재 받은 명령 수


    public void AddLogic(Action newLogic)
    {
        if (_currentCommandCount >= maxCommandLimit) return;
        OnLogic += newLogic;
        _currentCommandCount++;
    }

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        // 델리게이트가 비어있지 않으면 실행 (원래 잘 되던 방식)
        OnLogic?.Invoke();

        if (isLaunched)
    {
        ApplyPhysicalMovement();
    }

        HandleLifeTime();
    }

    private void ApplyPhysicalMovement()
    {
        // 핵심 논리: 회전력은 전진 속도에 종속됨
        float finalRotation = moveSpeed * rotationWeight;

        // 고개 돌리기 (나선 성질)
        if (finalRotation != 0)
        {
            transform.Rotate(Vector3.up, finalRotation * Time.deltaTime);
        }

        // 전진하기 (현재 바라보는 방향 기준)
        if (moveSpeed > 0)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    // --- [생명 주기 관리] ---
public void Init(Transform casterTransform)
    {
        caster = casterTransform;
        isLaunched = false;
        OnLogic = UpdateElementState; // 초기 추적 로직 주입
    }

public void Launch()
{
    if (isLaunched) return;
    isLaunched = true;

    // 델리게이트에서 "추적(UpdateElementState)"만 정교하게 뺍니다. 
    // += 로 추가된 다른 로직(Gigantism 등)은 그대로 남습니다.
    OnLogic -= UpdateElementState; 
    
    Debug.Log("[Magic] 발사됨! 이동 및 특수 로직 시작");
}

    private void UpdateElementState()
    {
        if (caster == null) return;
        Vector3 targetPos = caster.position + (caster.forward * followDistance) + (Vector3.up * floatingHeight);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, caster.rotation, Time.deltaTime * 5f);
    }

    #region 데이터 관리
    public void SetParam(string key, float value) => _params[key] = value;
    public float GetParam(string key) => _params.ContainsKey(key) ? _params[key] : 0;
    private void HandleLifeTime() { /* 생략 */ }
    #endregion
}