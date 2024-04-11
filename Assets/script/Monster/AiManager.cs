using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{
    public MonsterData monsterData;
    // Start is called before the first frame update


    void Start()
    {
        // 0.2�� �������� �ڷ�ƾ ȣ��
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            switch (monsterData.currentAIState)
            {
                case MonsterData.MonsterAIState.Idle: 
                    break;
                case MonsterData.MonsterAIState.Patrol:
                    break;
                case MonsterData.MonsterAIState.Chase:
                    break;
                case MonsterData.MonsterAIState.Attack:
                    break;
                case MonsterData.MonsterAIState.Die:
                    break;

            }
            
        }
    }
}
