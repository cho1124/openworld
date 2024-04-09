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

            // arrowRigidbody�� ȸ������ �߰� ȸ���� �����մϴ�.
            Quaternion desiredRotation = arrowRigidbody.transform.rotation * Quaternion.Euler(90f, -90f, 0f);

            // ����Ʈ�� ȸ������ �����մϴ�.
            transform.rotation = desiredRotation;
        }
        else
        {
            // ȭ���� ����� ��� ����Ʈ�� �����մϴ�.
            Destroy(gameObject);
        }

    }
}
