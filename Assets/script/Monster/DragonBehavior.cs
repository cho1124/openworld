using UnityEngine;

public class DragonBehavior : MonoBehaviour
{
    private Monster monster;
    

    private void Start()
    {
        monster = GetComponent<Monster>();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            Sword sword = other.GetComponent<Sword>();
            if (sword != null)
            {
                
                monster.Damaged(sword.damage);
                
            }
        }

        if (other.CompareTag("Player"))
        {
            CharacterStat characterStat = other.GetComponent<CharacterStat>();
            if (characterStat != null)
            {
                characterStat.TakeDamage(monster.damage);
            }
        }
    }
}
