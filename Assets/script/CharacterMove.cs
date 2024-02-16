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

        if (currentState == CharacterState.Attacking && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            StartCoroutine(ActivateAttackObjectCoroutine());
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

        if(horizontal != 0f)
        {
            animator.SetBool("Move", true);
        }


        if (Input.GetMouseButtonDown(0) && currentState != CharacterState.Attacking)
        {
            ChangeState(CharacterState.Attacking);
            Attack();
        }

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && currentState != CharacterState.Jumping)
        {
            ChangeState(CharacterState.Jumping);
            Jump();
        }
    }

    void UpdateAnimation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        animator.SetFloat("Speed", horizontal);
        animator.SetFloat("Speed", vertical);
        animator.SetBool("Run", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("Block", Input.GetMouseButton(1));
    }

    bool IsGrounded()
    {
        // 점프할 때 땅에 닿아있는지 여부를 판단할 코드 작성
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
        // 현재 상태가 공격 상태이고 공격 오브젝트가 존재하는지 확인
        if (currentState == CharacterState.Attacking && childAttack != null)
        {
            // 공격 상태일 때는 공격 오브젝트를 활성화
            childAttack.SetActive(true);
            // 코루틴 시작
        }
    }

    IEnumerator ActivateAttackObjectCoroutine()
    {
        // 0.5초 대기
        yield return new WaitForSeconds(1.0f);
        // 대기 후에 공격 오브젝트 비활성화
        childAttack.SetActive(false);
        ChangeState(CharacterState.Idle);
    }
}
