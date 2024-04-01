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
            // ȭ���� ����� ��� ����Ʈ�� �����մϴ�.
            Destroy(gameObject);
        }
    }
}
