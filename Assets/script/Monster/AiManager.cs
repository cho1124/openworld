using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{

    private Animator animator;
    public MonsterData monsterData;
    NavMeshAgent nmAgent;
    public float chaseDistance = 10f; // ������ �Ÿ�
    private Transform playerTransform; // �÷��̾� ��ġ ���� ����
    public float combatSpeed = 1.0f; // ���� �ӵ�
    // Start is called before the first frame update
    private FieldOfView fieldOfView;
    private float Timer = 0f;
    private bool isPatrolDestinationSet = false;
    public Transform[] points;
    private int destPoint = 0;
    public GameObject Attackpoint1;
    void Start()
    {
        animator = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
        monsterData.currentAIState = MonsterData.MonsterAIState.Idle;
        


        StartCoroutine(Test(0.2f));


    }
    IEnumerator Test(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            switch (monsterData.currentAIState)
            {
                case MonsterData.MonsterAIState.Idle:
                    //Debug.Log("Idle");
                    IdleToPatrol(delay);
                    
                    break;

                case MonsterData.MonsterAIState.Patrol:

                    
                    if (!isPatrolDestinationSet)
                    {
                        GotoNextPoint();
                        isPatrolDestinationSet = true; // �������� �����Ǿ����� ǥ��
                    }
                    
                    if (!nmAgent.pathPending && nmAgent.remainingDistance < 5.5f)
                    {
                        animator.SetBool("Patrol", false);
                        animator.SetFloat("Speed", 0f);
                        monsterData.currentAIState = MonsterData.MonsterAIState.Idle;
                        isPatrolDestinationSet = false; // Patrol�� �������Ƿ� ���� ��Ʈ�� �������� ������ �� �ֵ��� ���
                    }
                    break;

                case MonsterData.MonsterAIState.Fly:

                    break;
                case MonsterData.MonsterAIState.Combat:
                    animator.SetBool("Patrol", false);
                    animator.SetBool("InCombat", true); // ���� ���� Ʈ���� ����
                    Debug.Log("Combat");

                    TrackPlayer(delay);

                    if(monsterData.currentHealth == 40)
                    {
                        animator.SetBool("Fly", true);
                    }
                    //FlameAttack(delay);

                    break;

                case MonsterData.MonsterAIState.Die:

                    break;


            }
        }
    }

    
    void IdleToPatrol(float delay)
    {
        Timer += delay;
        
        if (Timer >= 5f) // ���÷� 10�ʰ� ������ Patrol ���·� ��ȯ�ϴ� ������ ����մϴ�.
        {
            animator.SetBool("Patrol", true);
            animator.SetFloat("Speed", combatSpeed);
            monsterData.currentAIState = MonsterData.MonsterAIState.Patrol;
            Timer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        nmAgent.destination = points[destPoint].position;
        
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    void TrackPlayer(float delay)
    {
        playerTransform = fieldOfView.playerTransform;
        if (playerTransform == null)
        {
            Debug.Log("Returned");
            return;
        }
            
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= chaseDistance)
        {
            //Debug.Log(distanceToPlayer +", " + chaseDistance);



            // �÷��̾�� �ٰ����� ����
            if (distanceToPlayer > monsterData.meleeAttackRange)
            {
                Timer += delay;
                Debug.Log(Timer);
                // �÷��̾� ������ �̵�
                nmAgent.SetDestination(playerTransform.position);
                
                // �����ϴ� ���� ����
                NavMeshHit hit;
                if (NavMesh.SamplePosition(playerTransform.position, out hit, chaseDistance, NavMesh.AllAreas))
                {
                    nmAgent.SetDestination(hit.position);
                }

                animator.SetFloat("Speed", combatSpeed);
            }
            else
            {
                // �÷��̾� ����
                AttackPlayerIfInRange(playerTransform.position);
            }
        }
        else
        {
            // ���� ������ ������Ƿ� ������ ����
            nmAgent.SetDestination(transform.position);
            monsterData.currentAIState = MonsterData.MonsterAIState.Idle;
            animator.SetFloat("Speed", 0f);
            animator.SetBool("InCombat", false);
        }
    }

    void AttackPlayerIfInRange(Vector3 playerPosition)
    {
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Attack");
        Debug.Log("Attack");
    }

    void BasicAttackStart()
    {
        Attackpoint1.SetActive(true);
    }

    void BasicAttackEnd()
    {
        Attackpoint1.SetActive(false);
    }

    void WingAttackStart()
    {
        //Attackpoint2.SetActive(true);
    }

    void WingAttackEnd()
    {
        //Attackpoint2.SetActive(false);
    }

    void FlameAttack(float delay)
    {
        Timer += delay;

        if(Timer > 5.0f)
        {
            //
        }

    }



    //�� �κк��� �Ʒ� �κ� ���� AI �Ŵ��� ��ũ��Ʈ�� �ű� ����

}
