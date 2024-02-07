using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float jumpForce = 10f; // ���� �� ������ ���� ����

    private Rigidbody rb;
    private bool isJumped = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2.0f : speed;

        // WSAD Ű �Է��� �����Ͽ� �̵� ������ ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �̵� ���⿡ �ӵ��� ���Ͽ� ���� �̵� �ӵ��� ���
        

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * currentSpeed * Time.deltaTime;

        // ���� ��ġ�� �̵����� ���� ĳ���͸� �̵���Ŵ
        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.Space)) // �����̽��ٸ� ������
        {
            Jump(); // ���� �Լ� ȣ��
        }


    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,jumpForce, rb.velocity.z); // ���� ���� �����Ͽ� Rigidbody�� y�� �ӵ��� ����
    }

}
