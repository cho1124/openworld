using System.Collections;
using UnityEngine;

public class DragonHead : MonoBehaviour
{
    private Monster monster; // Monster 스크립트 타입으로 선언된 변수
    
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
            int damage = other.GetComponentInParent<CharacterStat>().attackDamageSum; // 무기의 데미지 계산

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // 몬스터의 머리에 데미지를 적용하는 함수 호출
        }
        if (other.CompareTag("Arrow"))
        {
            int damage = other.GetComponent<Arrow>().damage; // 무기의 데미지 계산
            

            //Debug.Log("headShot");
            monster.DamagedOnHead(damage); // 몬스터의 머리에 데미지를 적용하는 함수 호출
        }
    }
}
