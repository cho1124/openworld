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
        if (arrowRigidbody != null)
        {
            transform.position = arrowRigidbody.transform.position;
            transform.rotation = arrowRigidbody.transform.rotation;
        }
        else
        {
            // 화살이 사라진 경우 이펙트도 제거합니다.
            Destroy(gameObject);
        }
    }
}
