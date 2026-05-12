using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MagicBase : MonoBehaviour
{
    public Action OnLogic;
    public Transform caster;
    private Dictionary<string, float> _params = new Dictionary<string, float>();

    public bool isLaunched = false;
    public bool isTargetingPlayer = false;  // 마법의 대상이 true 플레이어, false 원소인지

    private Vector3 _launchDirection; // 발사 시점의 방향을 기억할 변수
    public Vector3 InitialFireDirection { get; private set; } // 초기 발사 방향 저장
    // 이동에 사용할 고정된 화살표 (내부 관리용)
    private Vector3 _moveDirection;

    [Header("Stat")]
    public float magicDamage = 10f;
    public float moveSpeed = 0f;
    public float rotationWeight = 0f;       // 나선을 위한 계수
    public float magicLifeTime = 10f;       // 마법 유지 시간


    [Header("spawn")]
    public float followDistance = 0f;       // 플레이어와의 간격
    public float floatingHeight = 1.5f;     // 바닥에서 1.5m 


    [Header("Rules")]
    public int maxCommandLimit = 5;         // 최대 받을 수 있는 단어 명령 수
    private int _currentCommandCount = 0;   // 현재 받은 명령 수


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
        // 1. 회전 처리 (rotationWeight가 있을 때만 고개를 돌림)
        float finalRotation = moveSpeed * rotationWeight;
        if (finalRotation != 0)
        {
            transform.Rotate(Vector3.up, finalRotation * Time.deltaTime);
        }

        // 2. 이동 처리 (고쳐야 할 부분!)
        if (moveSpeed > 0)
        {
            if (isTargetingPlayer && caster != null)
            {
                // 1. 플레이어 대상 마법: 실시간으로 플레이어의 '코(forward)' 방향으로 전진
                // 조작에 따라 궤적이 휘어야 하므로 실시간 forward를 씁니다.
                transform.position += caster.forward * moveSpeed * Time.deltaTime;
            }
            else
            {
                // 2. 원소 마법: 발사 시점에 저장한 '고정 화살표' 방향으로만 전진
                // 모델이 아무리 돌아가도 절대 휘지 않습니다.
                transform.position += _moveDirection * moveSpeed * Time.deltaTime;
            }
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

        // 1. 발사 시점의 방향을 '박제'합니다.
        _moveDirection = transform.forward;
        InitialFireDirection = transform.forward;

        // 2. 추적 로직 중단
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

    public void ResetToNormalState()
    {
        // 1. 마법 상태 초기화
        isLaunched = false;
        isTargetingPlayer = false; // 타겟 해제
        OnLogic = null;           // 수행하던 마법 대본 삭제

        // 2. 물리 수치 초기화
        moveSpeed = 0f;
        rotationWeight = 0f;
        transform.localScale = Vector3.one; // 거대화 해제 (필요시)

        // 3. 조작권 복구 (PlayerController가 있다면)
        if (TryGetComponent(out PlayerController pc))
        {
            pc.enabled = true;
        }

        Debug.Log("플레이어 마법 상태가 종료되었습니다.");
    }



    #region 데이터 관리
    public void SetParam(string key, float value) => _params[key] = value;
    public float GetParam(string key) => _params.ContainsKey(key) ? _params[key] : 0;

    private void HandleLifeTime()
    {
        // 발사된 상태에서만 시간을 깎음
        if (isLaunched)
        {
            magicLifeTime -= Time.deltaTime;

            if (magicLifeTime <= 0)
            {
                StopMagic(); // 시간이 다 되면 마법 중단
            }
        }
    }

    public void StopMagic()
    {
        // 1. 모든 추가 로직(MoveForward, Spiral 등) 삭제
        OnLogic = null;

        // 2. 물리 수치 초기화
        moveSpeed = 0f;
        rotationWeight = 0f;
        isLaunched = false;

        // 3. 만약 플레이어라면 초기 상태(추적 등)로 돌리거나 종료 처리
        if (isTargetingPlayer)
        {
            // 플레이어 본체는 파괴하지 않고 상태만 리셋
            Debug.Log("플레이어 대상 마법 종료");
        }
        else
        {
            // 원소 마법은 오브젝트 파괴
            Destroy(gameObject);
        }
    }
    #endregion
}