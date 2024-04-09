using UnityEngine;

public class ArrowEffectFollow : MonoBehaviour
{
    private Rigidbody arrowRigidbody;

    public void SetArrow(Rigidbody arrow)
    {
        arrowRigidbody = arrow;
    }

    private void Update()
    {
        if (arrowRigidbody != null && !arrowRigidbody.isKinematic)
        {
            transform.position = arrowRigidbody.transform.position;

            // arrowRigidbody의 회전값에 추가 회전을 적용합니다.
            Quaternion desiredRotation = arrowRigidbody.transform.rotation * Quaternion.Euler(90f, -90f, 0f);

            // 이펙트의 회전값을 설정합니다.
            transform.rotation = desiredRotation;
        }
        else
        {
            // 화살이 사라진 경우 이펙트도 제거합니다.
            Destroy(gameObject);
        }

    }
}
