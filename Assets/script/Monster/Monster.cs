using UnityEngine;

public class Monster : MonoBehaviour
{
    
    public MonsterData monsterData;
    public Animator animator;   

    protected virtual void Start()
    {
        monsterData.currentHealth = monsterData.maxHealth;
        animator = GetComponent<Animator>();
        
    }

    public virtual void Damaged(int damageAmount)
    {
        monsterData.currentHealth -= damageAmount;
        animator.SetTrigger("Damaged");
        Debug.Log(monsterData.currentHealth);
        if (monsterData.currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void DamagedOnHead(int damageAmount)
    {

        monsterData.currentHealth -= damageAmount * 2;
        animator.SetTrigger("Damaged");
        if (monsterData.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        monsterData.currentHealth = 0;
    }
}
