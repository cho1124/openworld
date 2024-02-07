using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    public bool isGrounded; // �ٴڿ� ��� �ִ��� ���θ� �����ϴ� ����

    // Collider�� �ٸ� Collider�� ����� �� ȣ��Ǵ� �޼���
    //private void OnCollisionEnter(Collision collision)
    //{
    //    // �ٴڿ� ��� �ִ��� Ȯ��
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = true;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Collider�� �ٸ� Collider�� �������� �� ȣ��Ǵ� �޼���
    //private void OnCollisionExit(Collision collision)
    //{
    //    // �ٴڿ��� ���������� Ȯ��
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = false;
    //    }
    //}
}
