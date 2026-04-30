using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummyController : MonoBehaviour
{
    private PlayerController owner;
    private Animator anim;
    private MagicBase mb;
    // 분신 전용 설정

    public void Setup(PlayerController owner)
    {
        this.owner = owner;
        anim = GetComponent<Animator>();
        mb = GetComponent<MagicBase>();

        // 분신은 리지드바디의 물리 시뮬레이션을 직접 받지 않도록 설정 (본체 조작 복제 집중)
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        
        // 분신은 일정 시간 뒤 자동 소멸
        // float lifeTime = mb.magicLifeTime > 0 ? mb.magicLifeTime : 10f;
        Destroy(gameObject, mb.magicLifeTime);
    }

    void Update()
    {
        if (owner == null) return;

        // 1. 조작 복제: 본체의 MoveInput을 사용하여 이동
        Vector3 input = owner.GetLastMoveInput();
        if (input.magnitude > 0.1f)
        {
            // PlayerData에 정의된 playerMoveSpeed 사용
            transform.Translate(input * owner.Data.playerMoveSpeed * Time.deltaTime, Space.World);
            
            // 회전 동기화 (본체의 회전값을 그대로 추종하거나 델타값 적용;)
            transform.rotation = owner.GetRotationDelta();
        }

        // 2. 애니메이션 및 마법 동기화
        // 본체의 애니메이터 파라미터를 복사하면 애니메이션 이벤트에 걸린 마법 발사도 자동 실행됩니다.
        SyncAnimator(owner.Animator, anim);
        
    }

private void SyncAnimator(Animator source, Animator target)
    {
        if (source == null || target == null) return;

        // 본체의 모든 파라미터 상태를 복사
        foreach (var param in source.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Float)
                target.SetFloat(param.name, source.GetFloat(param.name));
            else if (param.type == AnimatorControllerParameterType.Bool)
                target.SetBool(param.name, source.GetBool(param.name));
            else if (param.type == AnimatorControllerParameterType.Int)
                target.SetInteger(param.name, source.GetInteger(param.name));
            // Trigger는 로직 흐름에 따라 추가 처리가 필요할 수 있음
        }
    }

    // 분신이 피격당했을 때 호출될 함수 (MagicObject 등에서 호출)
public void TakeDamage(float damage)
{
    if (owner != null)
    {
        // PlayerData의 stats(PlayerStats 클래스)에 접근하여 데미지 처리
        owner.Data.stats.TakeDamage(damage); 
        
        // currentHp로 변수명 수정
        Debug.Log($"분신 피격! 본체 체력 남음: {owner.Data.stats.currentHp}");
    }
}
}
