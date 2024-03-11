using UnityEngine;

public class DragonBehavior : MonoBehaviour
{
    private Monster monster;
    

    private void Start()
    {
        monster = GetComponentInParent<Monster>();
        if (monster != null)
        {
            Debug.Log("good?2");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            int damage = other.GetComponent<Sword>().damage; // ������ ������ ���

            //Debug.Log("headShot");
            monster.Damaged(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }

        if (other.CompareTag("Player"))
        {
            CharacterStat characterStat = other.GetComponent<CharacterStat>();
            if (characterStat != null)
            {
                characterStat.TakeDamage(monster.damage);
            }
        }
    }
}
