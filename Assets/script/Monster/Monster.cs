using UnityEngine;

public class Monster : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int damage;
    public Animator animator;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public virtual void Damaged(int damageAmount)
    {
        currentHealth -= damageAmount;
        animator.SetTrigger("Damaged");
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void DamagedOnHead(int damageAmount)
    {

        currentHealth -= damageAmount * 2;
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
