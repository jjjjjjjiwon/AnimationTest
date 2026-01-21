using UnityEngine;

[System.Serializable]
public class BossStats
{
    public float maxHp = 5000f;
    public float damage = 35f;
    public float moveSpeed = 3.5f;

    // Loader가 armor도 넣으려 하니까 필드 추가
    public float armor = 0f;
}