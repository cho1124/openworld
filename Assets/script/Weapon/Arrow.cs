using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;
    public GameObject effect;

    void Update()
    {
        // 화살이 항상 속도 벡터의 방향으로 회전하도록 설정합니다.

        if(!GetComponent<Rigidbody>().isKinematic)
        {
            transform.up = GetComponent<Rigidbody>().velocity.normalized;
            
        }

        

        
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
