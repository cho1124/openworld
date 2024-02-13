using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpForce = 10f; // ���� �� ������ ���� ����

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
        
        



        // WSAD Ű �Է��� �����Ͽ� �̵� ������ ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        if (vertical != 0)
        {
            m_Animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)));
            //�ִϸ��̼� ���
        }

        // �̵� ���⿡ �ӵ��� ���Ͽ� ���� �̵� �ӵ��� ���
        

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * currentSpeed * Time.deltaTime;

        // ���� ��ġ�� �̵����� ���� ĳ���͸� �̵���Ŵ
        transform.Translate(movement);

        if (a.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Jump");
            Jump(); // ���� �Լ� ȣ��
        }

        



    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,jumpForce, rb.velocity.z); // ���� ���� �����Ͽ� Rigidbody�� y�� �ӵ��� ����
    }

}
