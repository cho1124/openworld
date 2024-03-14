using System.Collections;
using UnityEngine;

public class DragonHead : MonoBehaviour
{
    private Monster monster; // Monster ��ũ��Ʈ Ÿ������ ����� ����
    

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
            int damage = other.GetComponent<Sword>().damage; // ������ ������ ���

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // ������ �Ӹ��� �������� �����ϴ� �Լ� ȣ��
        }
    }
}
