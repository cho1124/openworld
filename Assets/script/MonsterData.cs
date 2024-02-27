using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monsters/Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int maxHealth = 100;
    public int damage = 10;
    public float moveSpeed = 5f;
    // �߰����� ���� �Ӽ��� ���⿡ �߰��� �� �ֽ��ϴ�.
}
