using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EnemyStateLoader : MonoBehaviour
{
    // Factory가 가져갈 실제 데이터 리스트
    private List<EnemyIdleStateJsonData> idleStateList = new List<EnemyIdleStateJsonData>();                // idle
    private List<EnemyChaseStateJsonData> chaseStateList = new List<EnemyChaseStateJsonData>();             // chase
    private List<EnemyDashStateJsonData> dashStateList = new List<EnemyDashStateJsonData>();                // dash
    private List<EnemyTeleportStateJsonData> teleportStateList = new List<EnemyTeleportStateJsonData>();    // teleport
    private List<EnemyAttackMotionJsonData> motionList = new List<EnemyAttackMotionJsonData>();             // attack
    private List<EnemyComboJsonData> comboList = new List<EnemyComboJsonData>();                            // attack
    private List<EnemyStunStateJsonData> stunStateList = new List<EnemyStunStateJsonData>();                // stun
    private List<EnemyDeathStateJsonData> deathStateList = new List<EnemyDeathStateJsonData>();             // death

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
        // 수정한 부분: "EnemyAttackState" -> "EnemyChaseState"
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyChaseState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Chase 파일을 찾지 못했습니다!");
            return;
        }

        EnemyChaseStateListWrapper wrapper = JsonUtility.FromJson<EnemyChaseStateListWrapper>(targetFile.text);
        if (wrapper != null)
        {
            chaseStateList = wrapper.EnemyChaseState;
            Debug.Log($"<color=blue>[Chase]</color> {chaseStateList.Count}개 로드 완료.");
        }
    }

    public void LoadDashState()
    {
        // 수정한 부분: "EnemyAttackState" -> "EnemyChaseState"
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyDashState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Dash 파일을 찾지 못했습니다!");
            return;
        }

        EnemyDashStateListWrapper wrapper = JsonUtility.FromJson<EnemyDashStateListWrapper>(targetFile.text);
        if (wrapper != null)
        {
            dashStateList = wrapper.EnemyDashState;
            Debug.Log($"<color=blue>[Dash]</color> {dashStateList.Count}개 로드 완료.");
        }
    }

    public void LoadTeleportState()
    {
        // 수정한 부분: "EnemyAttackState" -> "EnemyChaseState"
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyTeleportState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Teleport 파일을 찾지 못했습니다!");
            return;
        }

        EnemyTeleportStateListWrapper wrapper = JsonUtility.FromJson<EnemyTeleportStateListWrapper>(targetFile.text);
        if (wrapper != null)
        {
            teleportStateList = wrapper.EnemyTeleportState;
            Debug.Log($"<color=blue>[Dash]</color> {teleportStateList.Count}개 로드 완료.");
        }
    }


    // attack Motion
    public void LoadAttackMotions()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyAttackMotion");
        if (targetFile != null)
        {
            EnemyAttackMotionListWrapper wrapper = JsonUtility.FromJson<EnemyAttackMotionListWrapper>(targetFile.text);
            motionList = wrapper.enemyAttackMotion;
            Debug.Log($"<color=orange>[Loader]</color> {motionList.Count}개의 공격 모션 로드 완료.");
        }
    }

    // attack Combos
    public void LoadCombos()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyCombo");
        if (targetFile != null)
        {
            EnemyComboListWrapper wrapper = JsonUtility.FromJson<EnemyComboListWrapper>(targetFile.text);
            comboList = wrapper.enemyCombo;
            Debug.Log($"<color=cyan>[Loader]</color> {comboList.Count}개의 콤보 데이터 로드 완료.");
        }
    }

    // stun
    public void LoadStunState()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyStunState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Stun 파일을 찾지 못했습니다!");
            return;
        }

        EnemyStunStateListWrapper wrapper = JsonUtility.FromJson<EnemyStunStateListWrapper>(targetFile.text);
        if (wrapper != null)
        {
            stunStateList = wrapper.EnemyStunState;
            Debug.Log($"<color=purple>[Chase]</color> {stunStateList.Count}개 로드 완료.");
        }
    }

    // death
    public void LoadDeathState()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Json/EnemyState/EnemyDeathState");

        if (targetFile == null)
        {
            Debug.LogError("[StateLoader] Death 파일을 찾지 못했습니다!");
            return;
        }

        EnemyDeathStateListWrapper wrapper = JsonUtility.FromJson<EnemyDeathStateListWrapper>(targetFile.text);
        if (wrapper != null)
        {
            deathStateList = wrapper.EnemyDeathState;
            Debug.Log($"<color=lime>[Death]</color> {deathStateList.Count}개 로드 완료.");
        }
    }


    // Factory가 데이터를 가져갈 수 있게 제공
    public List<EnemyIdleStateJsonData> GetIdleList() => idleStateList;
    public List<EnemyChaseStateJsonData> GetChaseList() => chaseStateList;
    public List<EnemyDashStateJsonData> GetDashList() => dashStateList;
    public List<EnemyTeleportStateJsonData> GetTeleportList() => teleportStateList;
    public List<EnemyAttackMotionJsonData> GetMotionList() => motionList;
    public List<EnemyComboJsonData> GetComboList() => comboList;
    public List<EnemyStunStateJsonData> GetStunList() => stunStateList;
    public List<EnemyDeathStateJsonData> GetDeathList() => deathStateList;
}