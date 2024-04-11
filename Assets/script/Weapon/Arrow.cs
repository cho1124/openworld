using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;
    public GameObject effect;

    private void Start()
    {
        damage += GetComponentInParent<CharacterStat>().attackDamageSum; //������ �� ȭ�� ��ü�� �������� Ȱ�� �������� �߰��ϵ��� �Ͽ���, �� �⺻ ĳ������ ���ݷ��� 0���� �����Ͽ���
    }


    void Update()
    {
        // ȭ���� �׻� �ӵ� ������ �������� ȸ���ϵ��� �����մϴ�.

        if(!GetComponent<Rigidbody>().isKinematic)
        {
            transform.up = GetComponent<Rigidbody>().velocity.normalized;
            
        }

        
        //�ܼ��� ȭ���� ������ �Ӹ� �ƴ϶� Ȱ�� �������� �߰��ϱ� ���� Ȱ�� �ڽ� Ŭ������ ȭ�� Ŭ������ ���� ����
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon") || other.CompareTag("Head"))
        {
            GameObject effectInstance = Instantiate(effect, transform.position, Quaternion.identity);

            // ���� �ð�(��: 5��)�� ���� �Ŀ� effectInstance�� �ı��մϴ�.
            Destroy(effectInstance, 1.0f);

            Destroy(gameObject);
            

            //Debug.Log("headShot"); 

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {

            Rigidbody arrowRigidbody = GetComponent<Rigidbody>();
            arrowRigidbody.isKinematic = true;

            GameObject effectInstance = Instantiate(effect, transform.position, Quaternion.identity);
            
            // ���� �ð�(��: 5��)�� ���� �Ŀ� effectInstance�� �ı��մϴ�.
            Destroy(effectInstance, 1.0f);
            
            //Destroy(gameObject, 1.0f);
        }

    }
}
