using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    private Rigidbody rb;

    void Start()
    {
        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // WSAD 키 입력을 감지하여 이동 방향을 설정
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 이동 방향에 속도를 곱하여 실제 이동 속도를 계산
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;

        // Rigidbody에 힘을 가해서 이동시킴
        rb.AddForce(movement, ForceMode.Force);

        // 좌우 회전 구현
        if (horizontal != 0f)
        {
            // horizontal 값에 따라서 좌우로 회전
            rb.AddTorque(Vector3.up * horizontal * rotationSpeed * Time.deltaTime);
        }
    }
    
}
