using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Monster
{
    public float breathCooldown = 5f;
    public float meleeAttackRange = 3f;
    public float flyingHealthThreshold = 50f;

    protected float currentBreathCooldown; // 브레스 발사 쿨다운

    protected override void Start()
    {
        base.Start();
        currentBreathCooldown = breathCooldown;
    }


    public void Fly()
    {
        // 하늘을 나는 동작을 여기에 추가
        animator.SetTrigger("Fly");
    }

    public void FireBreath()
    {
        // 브레스 발사 동작을 여기에 추가
        animator.SetTrigger("FireBreath");
    }

    public void RandomCharge()
    {
        // 랜덤한 위치로 돌진하는 동작을 여기에 추가
        animator.SetTrigger("RandomCharge");
    }

    void Update()
    {
        

        // 드래곤의 추가 행동 패턴
        if (currentHealth <= flyingHealthThreshold)
        {
            Fly();
        }

        // 특정 조건을 만족할 때 브레스 발사 패턴
        if (currentBreathCooldown <= 0f)
        {
            FireBreath();
            currentBreathCooldown = breathCooldown;
        }

        // 랜덤한 타이밍으로 돌진하는 패턴
        if (Random.value < 0.1f)
        {
            RandomCharge();
        }
    }
}
