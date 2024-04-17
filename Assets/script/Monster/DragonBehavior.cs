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
            int damage = other.GetComponentInParent<CharacterStat>().attackDamageSum; // 무기의 데미지 계산

            //Debug.Log("headShot");
            monster.Damaged(damage); // 몬스터의 머리에 데미지를 적용하는 함수 호출
        }
        if(other.CompareTag("Arrow"))
        {
            int damage = other.GetComponent<Arrow>().damage; // 무기의 데미지 계산

            //Debug.Log("headShot");
            monster.Damaged(damage); // 몬스터의 머리에 데미지를 적용하는 함수 호출
        }

        
    }
}
