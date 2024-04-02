using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;

    void Start()
    {
        // 화살이 처음 생성될 때 머리가 화살의 방향으로 향하도록 설정합니다.
        //transform.up = GetComponent<Rigidbody>().velocity.normalized;
    }

    void Update()
    {
        // 화살이 항상 속도 벡터의 방향으로 회전하도록 설정합니다.

        if(!GetComponent<Rigidbody>().isKinematic)
        {
            transform.up = GetComponent<Rigidbody>().velocity.normalized;
        }

        
    }
}
