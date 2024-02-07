using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    public bool isGrounded; // �ٴڿ� ��� �ִ��� ���θ� �����ϴ� ����

    // Collider�� �ٸ� Collider�� ����� �� ȣ��Ǵ� �޼���
    private void OnCollisionEnter(Collision collision)
    {
        // �ٴڿ� ��� �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Collider�� �ٸ� Collider�� �������� �� ȣ��Ǵ� �޼���
    private void OnCollisionExit(Collision collision)
    {
        // �ٴڿ��� ���������� Ȯ��
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
