using System.Collections;
using UnityEngine;

public class DragonHead : MonoBehaviour
{
    private Monster monster; // Monster 스크립트 타입으로 선언된 변수
    

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
        if (other.CompareTag("Sword")) // 만약 충돌한 객체가 Sword 태그를 가지고 충돌 처리가 이루어지지 않은 경우
        {
            int damage = other.GetComponent<Sword>().damage; // 무기의 데미지 계산

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // 몬스터의 머리에 데미지를 적용하는 함수 호출
        }
    }
}
