using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour
{
    public Transform backPosition;
    public Transform leftHandPosition;
    public GameObject bow;

    private bool equipBow = false;
    private Animator animator; // �ִϸ����� ������Ʈ�� ���� ����

    void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            equipBow = true; // Ȱ ������ ��û

            if(bow != null)
            {
                
                animator.SetTrigger("EquipBow");

                Invoke("MoveBowToLeftHand", 0.5f); // 1�� �Ŀ� MoveBowToLeftHand �޼��� ȣ��
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (bow != null)
            {
                animator.SetTrigger("ReleaseBow");

                Invoke("MoveBowToBack", 0.5f); // 1�� �Ŀ� MoveBowToLeftHand �޼��� ȣ��



                equipBow = false; // Ȱ ����
            }
        }

        
    }

    void MoveBowToLeftHand()
    {
        bow.transform.SetParent(leftHandPosition);
        bow.transform.localPosition = new Vector3(0.174f, 0f, 0f); // �� ���� (0.174, 0, 0)
        bow.transform.localRotation = Quaternion.identity;
    }

    void MoveBowToBack()
    {
        bow.transform.SetParent(backPosition);
        bow.transform.localPosition = new Vector3(-0.112f, 0.307f, -0.132f); // �� ���� (-0.2, 0, -0.15)
        bow.transform.localRotation = Quaternion.Euler(-62.128f, 64.645f, -40.307f);
    }

}