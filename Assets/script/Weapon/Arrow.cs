using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;

    void Start()
    {
        // ȭ���� ó�� ������ �� �Ӹ��� ȭ���� �������� ���ϵ��� �����մϴ�.
        //transform.up = GetComponent<Rigidbody>().velocity.normalized;
    }

    void Update()
    {
        // ȭ���� �׻� �ӵ� ������ �������� ȸ���ϵ��� �����մϴ�.

        if(!GetComponent<Rigidbody>().isKinematic)
        {
            transform.up = GetComponent<Rigidbody>().velocity.normalized;
        }

        
    }
}
