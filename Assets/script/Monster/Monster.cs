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

    private float attackInterval = 2f; // ���� ����

    Rigidbody rb;
    BoxCollider coll;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
    }

    void Start()
    {
        // attackInterval���� Attack() �Լ��� ȣ��
        InvokeRepeating("Attack", 0f, attackInterval);
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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        currentHealth = 0;
    }

}
