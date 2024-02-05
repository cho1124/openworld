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
        // Rigidbody ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // WSAD Ű �Է��� �����Ͽ� �̵� ������ ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �̵� ���⿡ �ӵ��� ���Ͽ� ���� �̵� �ӵ��� ���
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;

        // Rigidbody�� ���� ���ؼ� �̵���Ŵ
        rb.AddForce(movement, ForceMode.Force);

        // �¿� ȸ�� ����
        if (horizontal != 0f)
        {
            // horizontal ���� ���� �¿�� ȸ��
            rb.AddTorque(Vector3.up * horizontal * rotationSpeed * Time.deltaTime);
        }
    }
    
}
