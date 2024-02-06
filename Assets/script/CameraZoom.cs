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
        // 마우스 휠 입력을 감지하여 카메라의 거리를 조절
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        float zoomDelta = scrollAmount * zoomSpeed * Time.deltaTime;

        // 카메라의 거리를 조절하지만, 최소 및 최대 줌 거리 범위 내에 있어야 함
        float newDistance = Mathf.Clamp(transform.localPosition.z + zoomDelta, minZoomDistance, maxZoomDistance);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newDistance);
    }
}
