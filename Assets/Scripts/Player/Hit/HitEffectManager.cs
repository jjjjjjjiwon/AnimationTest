using UnityEngine;
using System.Collections;

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance;

    void Awake() => Instance = this;

    // 1. 역경직 (Hit Stop)
    public void HitStop(float duration)
    {
        if (duration <= 0) return;
        StartCoroutine(DoHitStop(duration));
    }

    private IEnumerator DoHitStop(float duration)
    {
        Time.timeScale = 0.05f; // 아주 느리게 멈춤
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f; // 원래대로
    }

    // 2. 카메라 흔들림 (간이 버전)
    public void CameraShake(float intensity, float duration)
    {
        // 메인 카메라에 붙은 스크립트가 있다면 호출, 없으면 간단히 구현 가능
        Debug.Log($"[Shake] 강도: {intensity}");
    }
}