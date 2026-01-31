using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EnemyStateLoader : MonoBehaviour
{
    // Factory가 가져갈 실제 데이터 리스트
    private List<EnemyIdleStateJsonData> idleStateList = new List<EnemyIdleStateJsonData>();
    private List<EnemyChaseStateJsonData> chaseStateList = new List<EnemyChaseStateJsonData>();
    
    // idle
    public void LoadIdleState()
    {
        // Resources.Load는 확장자를 쓰지 않고, Resources 폴더 이후의 경로만 씁니다.
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyIdleState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Resources에서 파일을 찾지 못했습니다!");
            return;
        }

        string jsonText = targetFile.text;

        // Wrapper를 사용하여 파싱
        EnemyIdleStateListWrapper wrapper = JsonUtility.FromJson<EnemyIdleStateListWrapper>(jsonText);

        if (wrapper != null && wrapper.EnemyIdleState != null)
        {
            // 파싱된 데이터를 멤버 변수에 할당 (이걸 안 하면 GetIdleList가 빈 값을 줌)
            idleStateList = wrapper.EnemyIdleState;
            Debug.Log($"<color=green>[StateLoader]</color> {idleStateList.Count}개의 Idle 데이터 로드 완료.");
        }
        else
        {
            Debug.LogError("[StateLoader] JSON 파싱 실패! 키 이름이나 형식을 확인하세요.");
        }
    }

    // chase
    public void LoadChaseState()
    {
        // 경로만 Chase용으로 변경
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyChaseState");

        if (targetFile == null) {
            Debug.LogError("[StateLoader] Chase 파일을 찾지 못했습니다!");
            return;
        }

        // Wrapper도 Chase용으로 사용
        EnemyChaseStateListWrapper wrapper = JsonUtility.FromJson<EnemyChaseStateListWrapper>(targetFile.text);
        if (wrapper != null) {
            chaseStateList = wrapper.EnemyChaseState;
            Debug.Log($"<color=blue>[Chase]</color> {chaseStateList.Count}개 로드 완료.");
        }
    }

    // Factory가 데이터를 가져갈 수 있게 제공
    public List<EnemyIdleStateJsonData> GetIdleList() => idleStateList;
    public List<EnemyChaseStateJsonData> GetChaseList() => chaseStateList;
}