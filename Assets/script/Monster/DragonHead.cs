using System.Collections;
using UnityEngine;

public class DragonHead : MonoBehaviour
{
    private Monster monster; // Monster ��ũ��Ʈ Ÿ������ ����� ����
    
    public MonsterData monsterData;

    private void Start()
    {
        monster = GetComponentInParent<Monster>();
        if(monster != null )
        {
            Debug.Log("good?");
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            int damage = other.GetComponentInParent<CharacterStat>().attackDamageSum; // ������ ������ ���

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }
        if (other.CompareTag("Arrow"))
        {
            int damage = other.GetComponent<Arrow>().damage; // ������ ������ ���
            

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }
    }
}
