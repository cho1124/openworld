using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None = 0,
    Sword = 1,
    Bow = 2,
    // 추가 무기 종류는 여기에 나열
}


public class WeaponEquip : MonoBehaviour
{
    public Transform backPosition;
    public Transform leftHandPosition;
    public Transform rightHandPosition;
    public GameObject bow;
    public GameObject sword;
    private WeaponType currentWeapon = WeaponType.None; // 현재 장착된 무기


    private bool equipBow = false;
    private bool equipSword = true;
    private Animator animator; // 애니메이터 컴포넌트에 대한 참조

    void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        
        animator.SetBool("UseSword", true);
        currentWeapon = WeaponType.Sword;
        animator.SetInteger("WeaponType", (int)currentWeapon);
        MoveBowToBack();
        MoveSwordToRightHand();
        


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !equipBow)
        {
            //ReleaseSword();
            EquipWeapon(WeaponType.Bow);
            equipBow = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !equipSword)
        {

            //ReleaseBow();
            EquipWeapon(WeaponType.Sword);
            equipSword = true;

        }
    }


    private void EquipWeapon(WeaponType newWeaponType)
    {
        switch(newWeaponType)
        {
            case WeaponType.None:
                break;
            case WeaponType.Bow:
                equipSword = false;
                animator.SetTrigger("ReleaseSword");
                animator.SetBool("UseSword", false);
                //animator.SetInteger("test", 1);
                WeaponType weapon = WeaponType.Bow;
                animator.SetInteger("WeaponType", (int)weapon);
                
                break;
            case WeaponType.Sword:
                equipBow = false;
                animator.SetTrigger("ReleaseBow");
                animator.SetBool("UseSword", true);
                weapon = WeaponType.Sword;
                animator.SetInteger("WeaponType", (int)weapon);

                break;


        }
    }

    private void ReleaseBow()
    {
        equipBow = false;
        animator.SetTrigger("ReleaseBow");
        animator.SetBool("UseSword", true);
        WeaponType weapon = WeaponType.Sword;
        animator.SetInteger("WeaponType", (int)weapon);
    }



    private void ReleaseSword()
    {
        equipSword = false;
        animator.SetTrigger("ReleaseSword");
        animator.SetBool("UseSword", false);
        //animator.SetInteger("test", 1);
        WeaponType weapon = WeaponType.Bow;
        animator.SetInteger("WeaponType", (int)weapon);
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

    void MoveSwordToRightHand()
    {
        sword.transform.SetParent(rightHandPosition);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
    }

    void MoveSwordToBack()
    {
        sword.transform.SetParent(backPosition);
        sword.transform.localPosition = new Vector3(-0.1402862f, 0.145049f, 0.1862096f); // 이 값을 (-0.2, 0, -0.15)
        sword.transform.localRotation = Quaternion.Euler(-3.94f, -178.37f, -67.721f);
    }


}