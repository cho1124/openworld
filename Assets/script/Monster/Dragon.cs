using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Monster
{
    public float breathCooldown = 5f;
    public float meleeAttackRange = 3f;
    public float flyingHealthThreshold = 50f;

    protected float currentBreathCooldown; // �극�� �߻� ��ٿ�

    protected override void Start()
    {
        base.Start();
        currentBreathCooldown = breathCooldown;
    }


    public void Fly()
    {
        // �ϴ��� ���� ������ ���⿡ �߰�
        animator.SetTrigger("Fly");
    }

    public void FireBreath()
    {
        // �극�� �߻� ������ ���⿡ �߰�
        animator.SetTrigger("FireBreath");
    }

    public void RandomCharge()
    {
        // ������ ��ġ�� �����ϴ� ������ ���⿡ �߰�
        animator.SetTrigger("RandomCharge");
    }

    void Update()
    {
        

        // �巡���� �߰� �ൿ ����
        if (currentHealth <= flyingHealthThreshold)
        {
            Fly();
        }

        // Ư�� ������ ������ �� �극�� �߻� ����
        if (currentBreathCooldown <= 0f)
        {
            FireBreath();
            currentBreathCooldown = breathCooldown;
        }

        // ������ Ÿ�̹����� �����ϴ� ����
        if (Random.value < 0.1f)
        {
            RandomCharge();
        }
    }
}
