using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float minZoomDistance = 2.5f;
    public float maxZoomDistance = 5f;

    void Update()
    {
        // ���콺 �� �Է��� �����Ͽ� ī�޶��� �Ÿ��� ����
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        float zoomDelta = scrollAmount * zoomSpeed * Time.deltaTime;

        // ī�޶��� �Ÿ��� ����������, �ּ� �� �ִ� �� �Ÿ� ���� ���� �־�� ��
        float newDistance = Mathf.Clamp(transform.localPosition.z + zoomDelta, minZoomDistance, maxZoomDistance);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newDistance);
    }
}
