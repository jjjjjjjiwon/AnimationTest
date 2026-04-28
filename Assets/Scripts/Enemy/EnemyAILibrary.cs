using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAILibrary
{
#region 부품
    // 시야 체크 부품: 플레이어가 시야각 내에 있는지 확인
    public static bool IsPlayerInVision(Transform me, Transform player, float angle, float dist)
    {
        float d = Vector3.Distance(me.position, player.position);
        if (d > dist) return false;

        Vector3 dir = (player.position - me.position).normalized;
        return Vector3.Angle(me.forward, dir) < angle * 0.5f;
    }

    // 거리 체크 부품: 특정 범위 안에 있는지 확인
    public static bool IsInDistance(Transform me, Transform player, float targetDist)
    {
        return Vector3.Distance(me.position, player.position) <= targetDist;
    }

    public static bool DetectThreat(Transform me, float scanRadius)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Magic");

        // Physics.OverlapSphere는 지정된 레이어만 필터링해서 가져옵니다.
        Collider[] hitColliders = Physics.OverlapSphere(me.position, scanRadius, layerMask);

        return hitColliders.Length > 0;
    }
#endregion

    // [좀비형]: 그냥 플레이어 방향으로 뚜벅뚜벅
    public static Vector3 GetZombieMove(Transform me, Transform player)
    {
        return (player.position - me.position).normalized;
    }

    // [박쥐형]: 위아래로 흔들리면서 플레이어에게 접근
    public static Vector3 GetBatMove(Transform me, Transform player)
    {

        Vector3 dir = (player.position - me.position).normalized;
        dir.y = Mathf.Sin(Time.time * 5f) * 0.5f; // 위아래 흔들림 추가
        return dir;
    }

public static Vector3 GetAssassinMove(Transform me, Transform player, float viewAngle, float viewDist, out float speedMultiplier)
{
    speedMultiplier = 1.0f;
    

    bool canSee = IsPlayerInVision(me, player, viewAngle, viewDist);
    bool isThreatDetected = DetectThreat(me, 10f);

    Vector3 finalDir = (player.position - me.position).normalized;

if (canSee && isThreatDetected)
{
    Vector3 sideDir = Vector3.Cross(Vector3.up, finalDir).normalized;

    // [수정] 플레이어 방향(0.1)보다 옆 방향(0.9)의 비중을 압도적으로 높임
    // 이렇게 해야 전진을 멈추고 옆으로 "확" 꺾습니다.
    finalDir = (finalDir * 0.1f + sideDir * 0.9f).normalized; 
    
    // 속도 배율을 out으로 넘겨줍니다 (이게 핵심!)
    speedMultiplier = 10.0f; 
    
    //Debug.Log("<color=cyan>[AI]</color> 회피 가속 중!");
}

    return finalDir;
}

}
