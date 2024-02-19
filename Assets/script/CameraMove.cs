using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target; // 캐릭터의 Transform
    public float rotateSpeed = 5f; // 회전 속도
    public float verticalLimit = 60f; // 수직 회전 제한 각도

    private float verticalRotation = 0f; // 수직 회전 각도 저장 변수

    void Update()
    {
        // 마우스 입력 감지
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 마우스 입력에 따라 오브젝트 회전
        RotateObject(mouseX, mouseY);

        // 캐릭터를 따라다니며 카메라 위치 및 회전 조정
        //FollowTarget();
    }

    void RotateObject(float mouseX, float mouseY)
    {
        // 수평 회전
        target.Rotate(Vector3.up * mouseX * rotateSpeed, Space.World);

        verticalRotation -= mouseY * rotateSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLimit, verticalLimit); // 수직 회전 각도 제한
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // 카메라 수직 회전 적용
    }

    void FollowTarget()
    {
        // 캐릭터를 따라다니며 카메라 위치 조정
        transform.position = new Vector3(target.position.x + 1f, 1.81f, target.position.z - 1f);

        // 캐릭터를 바라보도록 카메라 회전 조정
        transform.rotation = target.rotation;
    }
}
