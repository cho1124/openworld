using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpForce = 10f; // 점프 힘 조절을 위한 변수

    private Rigidbody rb;
    private bool isJumped = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2.0f : speed;

        // WSAD 키 입력을 감지하여 이동 방향을 설정
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 이동 방향에 속도를 곱하여 실제 이동 속도를 계산
        

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * currentSpeed * Time.deltaTime;

        // 현재 위치에 이동량을 더해 캐릭터를 이동시킴
        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바를 누르면
        {
            Jump(); // 점프 함수 호출
        }


    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,jumpForce, rb.velocity.z); // 점프 힘을 적용하여 Rigidbody의 y축 속도를 변경
    }

}
