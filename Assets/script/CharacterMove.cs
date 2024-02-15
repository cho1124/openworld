using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterState
{
    Idle,
    Walking,
    Attacking,
    Jumping,
    Blocking
}

public class CharacterMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpForce = 10f;

    private Rigidbody rb;
    private Animator animator;
    private CharacterState currentState = CharacterState.Idle;
    
    public GameObject childAttack;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        

    }

    void Update()
    {
        
        HandleInput();
        UpdateAnimation();

        UpdateAttackObject();

        if (currentState == CharacterState.Attacking && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            ChangeState(CharacterState.Idle);
        }

        if (currentState == CharacterState.Jumping && !animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            ChangeState(CharacterState.Idle);
        }
    }
   

    void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetMouseButtonDown(0) && currentState != CharacterState.Attacking)
        {
            
            ChangeState(CharacterState.Attacking);
            Attack();
        }

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)  && currentState != CharacterState.Jumping)
        {
            ChangeState(CharacterState.Jumping);
            Jump();
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical"))));
        animator.SetBool("Run", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("Block", Input.GetMouseButton(1));
    }

    bool IsGrounded()
    {
        // ������ �� ���� ����ִ��� ���θ� �Ǵ��� �ڵ� �ۼ�
        return true;
    }

    void Attack()
    {
        
        animator.SetTrigger("Attack");
        
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        
    }

    void ChangeState(CharacterState newState)
    {
        currentState = newState;
    }

    void UpdateAttackObject()
    {
        // ���� ���°� ���� �����̰� ���� ������Ʈ�� �����ϴ��� Ȯ��
        if (currentState == CharacterState.Attacking && childAttack != null)
        {
            // ���� ������ ���� ���� ������Ʈ�� Ȱ��ȭ
            childAttack.SetActive(true);
        }
        else
        {
            // �� ���� ������ ���� ���� ������Ʈ�� ��Ȱ��ȭ
            if (childAttack != null)
            {
                childAttack.SetActive(false);
            }
        }
    }
    

}
