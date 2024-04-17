using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttack : MonoBehaviour
{
    private Monster monster;
    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponentInParent<Monster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStat characterStat = other.GetComponent<CharacterStat>();
            if (characterStat != null)
            {
                characterStat.TakeDamage(monster.monsterData.damage);
            }
        }
    }
}
