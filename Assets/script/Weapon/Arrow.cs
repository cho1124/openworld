using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;
    public GameObject effect;

    private void Start()
    {
        damage += GetComponentInParent<CharacterStat>().attackDamageSum; //생성될 때 화살 자체의 데미지에 활의 데미지를 추가하도록 하였음, 내 기본 캐릭터의 공격력은 0으로 설정하였음
    }


    void Update()
    {
        // 화살이 항상 속도 벡터의 방향으로 회전하도록 설정합니다.

        if(!GetComponent<Rigidbody>().isKinematic)
        {
            transform.up = GetComponent<Rigidbody>().velocity.normalized;
            
        }

        
        //단순히 화살의 데미지 뿐만 아니라 활의 데미지도 추가하기 위해 활의 자식 클래스로 화살 클래스를 만들 예정
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon") || other.CompareTag("Head"))
        {
            GameObject effectInstance = Instantiate(effect, transform.position, Quaternion.identity);

            // 일정 시간(예: 5초)이 지난 후에 effectInstance를 파괴합니다.
            Destroy(effectInstance, 1.0f);

            Destroy(gameObject);
            

            //Debug.Log("headShot"); 

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {

            Rigidbody arrowRigidbody = GetComponent<Rigidbody>();
            arrowRigidbody.isKinematic = true;

            GameObject effectInstance = Instantiate(effect, transform.position, Quaternion.identity);
            
            // 일정 시간(예: 5초)이 지난 후에 effectInstance를 파괴합니다.
            Destroy(effectInstance, 1.0f);
            
            //Destroy(gameObject, 1.0f);
        }

    }
}
