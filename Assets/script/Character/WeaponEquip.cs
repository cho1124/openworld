using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour
{
    public Transform backPosition;
    public Transform leftHandPosition;
    public GameObject bow;

    private bool equipBow = false;
    private Animator animator; // 애니메이터 컴포넌트에 대한 참조

    void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            equipBow = true; // 활 장착을 요청

            if(bow != null)
            {
                
                animator.SetTrigger("EquipBow");

                Invoke("MoveBowToLeftHand", 0.5f); // 1초 후에 MoveBowToLeftHand 메서드 호출
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (bow != null)
            {
                animator.SetTrigger("ReleaseBow");

                Invoke("MoveBowToBack", 0.5f); // 1초 후에 MoveBowToLeftHand 메서드 호출



                equipBow = false; // 활 해제
            }
        }

        
    }

    void MoveBowToLeftHand()
    {
        bow.transform.SetParent(leftHandPosition);
        bow.transform.localPosition = new Vector3(0.174f, 0f, 0f); // 이 값을 (0.174, 0, 0)
        bow.transform.localRotation = Quaternion.identity;
    }

    void MoveBowToBack()
    {
        bow.transform.SetParent(backPosition);
        bow.transform.localPosition = new Vector3(-0.112f, 0.307f, -0.132f); // 이 값을 (-0.2, 0, -0.15)
        bow.transform.localRotation = Quaternion.Euler(-62.128f, 64.645f, -40.307f);
    }

}