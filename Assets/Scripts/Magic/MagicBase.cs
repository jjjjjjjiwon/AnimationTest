using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MagicBase : MonoBehaviour
{
    // --- [규칙 및 상태 변수] ---
    public Action OnLogic; // 라이브러리에서 주입한 실행 대본
    public Transform caster;
    public bool isLaunched = false;

    private Dictionary<string, float> _params = new Dictionary<string, float>();

    [Header("Stat")]
    public float moveSpeed = 0f;      // 이동 속도
    public float followDistance = 0f; // 플레이어와의 간격
    public float floatingHeight = 1.5f; // 💡 바닥에서 1.5m 띄우기


    [Header("Rules")]
    public int maxCommandLimit = 5;    // 최대 받을 수 있는 단어 명령 수
    private int _currentCommandCount = 0; // 현재 받은 명령 수


    // --- [핵심 기능: 규칙 검사 및 로직 주입] ---
    public void AddLogic(Action newLogic)
    {
        // 규칙 1: 이미 발사되었는가? (상태 판정)
        // 규칙 2: 명령 횟수를 초과했는가? (한도 판정)
        if (_currentCommandCount >= maxCommandLimit)
        {
            Debug.Log($"{gameObject.name}: 명령 한도 초과로 추가 명령을 거절합니다.");
            return;
        }

        // 규칙 통과 시 로직 추가
        OnLogic += newLogic;
        _currentCommandCount++;
        Debug.Log($"{gameObject.name}: 새로운 명령 수락! ({_currentCommandCount}/{maxCommandLimit})");
    }

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        // 등록된 대본 실행 (델리게이트 시스템)
        OnLogic?.Invoke();

        // 수명 관리
        HandleLifeTime();
    }

    // --- [생명 주기 관리] ---
    public void Init(Transform casterTransform)
    {
        caster = casterTransform;
        isLaunched = false;

        // 초기 상태: 플레이어 앞을 따라다니는 기본 대본 주입
        OnLogic = UpdateElementState;
    }

    public void Launch()
    {
        if (isLaunched) return; // 이미 발사되었다면 무시

        isLaunched = true;

        // 💡 OnLogic = null 대신, '추적(UpdateElementState)'만 델리게이트에서 뺍니다.
        // 만약 이미 명령이 쌓여있었다면 그 명령들은 유지됩니다.
        OnLogic -= UpdateElementState;
    }

    private void UpdateElementState()
    {
if (caster == null) return;

        // 💡 플레이어의 위치에 floatingHeight만큼 더해줍니다.
        Vector3 targetPos = caster.position + (caster.forward * followDistance) + (Vector3.up * floatingHeight);
        
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, caster.rotation, Time.deltaTime * 5f);
    }

    #region 데이터 관리 (Utility)
    public void SetParam(string key, float value) => _params[key] = value;
    public float GetParam(string key) => _params.ContainsKey(key) ? _params[key] : 0;

    private void HandleLifeTime()
    {
        float life = GetParam("Life");
        if (life > 0)
        {
            life -= Time.deltaTime;
            SetParam("Life", life);
            if (life <= 0) Destroy(gameObject);
        }
    }

    
    #endregion
}