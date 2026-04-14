using System;
using UnityEngine;
using System.Collections.Generic;


public class PlayerSkillLoader : MonoBehaviour
{
    // 적 시스템과 동일하게 static으로 선언하여 어디서든 도감에 접근 가능하게 합니다.
    public static Dictionary<string, PlayerSkillData> SkillDict = new Dictionary<string, PlayerSkillData>();

    private void Awake()
    {
        LoadSkillData();
    }

    private void LoadSkillData()
    {
        // 1. JSON 파일 로드 
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Player/PlayerSkill");

        if (jsonFile == null)
        {
            Debug.LogError("PlayerSkillLibrary JSON 파일을 찾을 수 없습니다! 경로를 확인하세요.");
            return;
        }

        // 2. Wrapper 클래스로 역직렬화
        PlayerSkillDataListWrapper wrapper = JsonUtility.FromJson<PlayerSkillDataListWrapper>(jsonFile.text);

        if (wrapper != null && wrapper.PlayerSkills != null)
        {
            SkillDict.Clear(); // 중복 로드 방지

            // 3. 리스트 데이터를 Dictionary에 담아 ID로 즉시 검색 가능하게 함
            foreach (var skill in wrapper.PlayerSkills)
            {
                if (!SkillDict.ContainsKey(skill.player_Skill_ID))
                {
                    SkillDict.Add(skill.player_Skill_ID, skill);
                }
            }
            Debug.Log($"[PlayerSkillLoader] {SkillDict.Count}개의 플레이어 스킬 데이터를 로드했습니다.");
        }
    }

    // 특정 스킬 데이터를 가져오는 편의 기능
    public static PlayerSkillData GetSkill(string id)
    {
        if (SkillDict.TryGetValue(id, out PlayerSkillData data))
        {
            return data;
        }
        return null;
    }

// ... 기존 코드들 ...

    public static List<PlayerSkillData> GetAllSkills()
    {
        // ⭐ skillDatabase가 아니라 유저님이 정의하신 SkillDict를 사용해야 합니다.
        if (SkillDict == null || SkillDict.Count == 0)
        {
            return new List<PlayerSkillData>();
        }
        
        return new List<PlayerSkillData>(SkillDict.Values);
    }

}