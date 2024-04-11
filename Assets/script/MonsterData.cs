using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monsters/Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int maxHealth = 100;
    public int damage = 10;
    public int currentHealth = 10;
    public float moveSpeed = 5f;
    // �߰����� ���� �Ӽ��� ���⿡ �߰��� �� �ֽ��ϴ�.

    public enum MonsterAIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Die
    }
    public MonsterAIState currentAIState = MonsterAIState.Idle;
    //public MonsterAIState initialAIState = MonsterAIState.Idle;
    public float chaseRange = 10f;
    public float attackRange = 2f;

}
