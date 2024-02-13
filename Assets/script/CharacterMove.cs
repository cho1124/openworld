using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpForce = 10f; // 점프 힘 조절을 위한 변수

    private Rigidbody rb;
    private bool isJumped = false;
    private JumpDetection a;

    Animator m_Animator;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        a = GetComponentInChildren<JumpDetection>();

        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2.0f : speed;

        
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            
            m_Animator.SetBool("Run", true);
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_Animator.SetBool("Run", false);
        }
        
        



        // WSAD 키 입력을 감지하여 이동 방향을 설정
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        if (vertical != 0)
        {
            m_Animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)));
            //애니메이션 재생
        }

        // 이동 방향에 속도를 곱하여 실제 이동 속도를 계산
        

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * currentSpeed * Time.deltaTime;

        // 현재 위치에 이동량을 더해 캐릭터를 이동시킴
        transform.Translate(movement);

        if (a.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Jump");
            Jump(); // 점프 함수 호출
        }

        



    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,jumpForce, rb.velocity.z); // 점프 힘을 적용하여 Rigidbody의 y축 속도를 변경
    }

}
