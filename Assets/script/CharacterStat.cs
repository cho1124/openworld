using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    private Animator animator;
    public bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Method to apply damage to the character
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Check if the character is dead
        if (currentHealth <= 0)
        {
            Die();
        }
        animator.SetTrigger("Damaged");
    }

    // Method to handle character death
    void Die()
    {
        // Handle character death here
        Debug.Log("Character died!");
        animator.SetTrigger("Die");
        isDead = true;
        // For example, you could deactivate the GameObject or play death animation.
    }

    
}
