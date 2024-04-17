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
            int damage = other.GetComponentInParent<CharacterStat>().attackDamageSum; // ������ ������ ���

            //Debug.Log("headShot");
            monster.Damaged(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }
        if(other.CompareTag("Arrow"))
        {
            int damage = other.GetComponent<Arrow>().damage; // ������ ������ ���

            //Debug.Log("headShot");
            monster.Damaged(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }

        
    }
}
