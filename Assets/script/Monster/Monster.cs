using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int defend;
    public float speed;
    public Animator animator;

    private float attackInterval = 2f; // 공격 간격

    Rigidbody rb;
    BoxCollider coll;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
    }

    protected virtual void Start()
    {
        // attackInterval마다 Attack() 함수를 호출

        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        //InvokeRepeating("Attack", 0f, attackInterval);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Sword")
        {
            Sword sword = other.GetComponent<Sword>();
            if (sword != null)
            {
                Damaged(sword.damage);
                Debug.Log(currentHealth);
            }
        }

        if (other.CompareTag("Player"))
        {
            CharacterStat characterStat = other.GetComponent<CharacterStat>();
            if (characterStat != null)
            {
                
                characterStat.TakeDamage(damage);
                Debug.Log(characterStat.currentHealth);
            }
        }

    }


    public void Attack()
    {
        Debug.Log("Monster attacked!");
    }

    public void Sleep()
    {

    }

    public void Damaged(int damageAmount)
    {
        currentHealth -= damageAmount;
        animator.SetTrigger("Damaged");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        currentHealth = 0;
    }

}
