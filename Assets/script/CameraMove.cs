using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target; // ĳ������ Transform
    public float rotateSpeed = 5f; // ȸ�� �ӵ�
    public float verticalLimit = 60f; // ���� ȸ�� ���� ����

    private float verticalRotation = 0f; // ���� ȸ�� ���� ���� ����

    void Update()
    {
        // ���콺 �Է� ����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���콺 �Է¿� ���� ������Ʈ ȸ��
        RotateObject(mouseX, mouseY);

        // ĳ���͸� ����ٴϸ� ī�޶� ��ġ �� ȸ�� ����
        //FollowTarget();
    }

    void RotateObject(float mouseX, float mouseY)
    {
        // ���� ȸ��
        target.Rotate(Vector3.up * mouseX * rotateSpeed, Space.World);

        verticalRotation -= mouseY * rotateSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLimit, verticalLimit); // ���� ȸ�� ���� ����
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // ī�޶� ���� ȸ�� ����
    }

    void FollowTarget()
    {
        // ĳ���͸� ����ٴϸ� ī�޶� ��ġ ����
        transform.position = new Vector3(target.position.x + 1f, 1.81f, target.position.z - 1f);

        // ĳ���͸� �ٶ󺸵��� ī�޶� ȸ�� ����
        transform.rotation = target.rotation;
    }
}
