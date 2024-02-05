using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rotateSpeed = 5f; // 회전 속도

    void Update()
    {
        // 마우스 입력 감지
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 마우스 입력에 따라 오브젝트 회전
        RotateObject(mouseX, mouseY);
    }

    void RotateObject(float mouseX, float mouseY)
    {
        // 수평 회전
        transform.Rotate(Vector3.up * mouseX * rotateSpeed, Space.World);

        // 수직 회전 (상하로 돌리지 않으려면 주석 처리)
        // transform.Rotate(Vector3.left * mouseY * rotateSpeed, Space.Self);
    }
}
