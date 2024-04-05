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
        if (other.CompareTag("Dragon"))
        {
            Instantiate(effect, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
            

            //Debug.Log("headShot");

        }
    }
}
