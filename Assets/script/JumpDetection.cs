using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    public bool isGrounded; // 바닥에 닿아 있는지 여부를 저장하는 변수

    // Collider가 다른 Collider에 닿았을 때 호출되는 메서드
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿아 있는지 확인
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Collider가 다른 Collider에 떼어졌을 때 호출되는 메서드
    private void OnCollisionExit(Collision collision)
    {
        // 바닥에서 떼어졌는지 확인
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
