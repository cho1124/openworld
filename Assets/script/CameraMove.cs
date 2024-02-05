using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rotateSpeed = 5f; // ȸ�� �ӵ�

    void Update()
    {
        // ���콺 �Է� ����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���콺 �Է¿� ���� ������Ʈ ȸ��
        RotateObject(mouseX, mouseY);
    }

    void RotateObject(float mouseX, float mouseY)
    {
        // ���� ȸ��
        transform.Rotate(Vector3.up * mouseX * rotateSpeed, Space.World);

        // ���� ȸ�� (���Ϸ� ������ �������� �ּ� ó��)
        // transform.Rotate(Vector3.left * mouseY * rotateSpeed, Space.Self);
    }
}
