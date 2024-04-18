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
                    if(animator.GetCurrentAnimatorStateInfo(0).IsName("Get Hit"))
                    {
                        monsterData.currentAIState = MonsterData.MonsterAIState.Combat;
                    }
                    break;

                case MonsterData.MonsterAIState.Patrol:


                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Get Hit"))
                    {
                        monsterData.currentAIState = MonsterData.MonsterAIState.Combat;
                    }

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
                    //Debug.Log("Combat");
                    
                    TrackPlayer(delay);
                    
                    if (monsterData.currentHealth == 40)
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
        monsterData.Timer += delay;
        
        if (monsterData.Timer >= 5f) // ���÷� 10�ʰ� ������ Patrol ���·� ��ȯ�ϴ� ������ ����մϴ�.
        {
            animator.SetBool("Patrol", true);
            animator.SetFloat("Speed", combatSpeed);
            monsterData.currentAIState = MonsterData.MonsterAIState.Patrol;
            monsterData.Timer = 0f;
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
        //�� ������ ��츦 ó��, fov�� ���� �÷��̾ �����ϰų�, �������� �Ծ ������ �������·� �����ϰų�
        if(fieldOfView.playerTransform != null)
        {
            playerTransform = fieldOfView.playerTransform;
        }
        else
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            //��ó�� Player�±׸� ���� ������Ʈ�� Ž���Ͽ� �� ������Ʈ�� �����ϵ��� ��, chaseDisntance�ۿ� ������ ������ �����ϰ� �ٽ� �⺻���·� ���ư�, chasedistance�ȿ� ������ �ٷ� ������ ������
        }

        
        if (playerTransform == null)
        {
            Debug.Log("Returned");
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"))
        {
            monsterData.Timer = 0f;
            nmAgent.speed = 0f;
        }
        else
        {
            nmAgent.speed = 2.5f;
        }

            
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        

        if (distanceToPlayer <= chaseDistance)
        {
            // �÷��̾�� �ٰ����� ����
            if (distanceToPlayer > monsterData.meleeAttackRange)
            {
                monsterData.Timer += delay;
                
                Debug.Log(monsterData.Timer);
                // �÷��̾� ������ �̵�
                FlameAttack();
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
                monsterData.Timer = 0f;
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

    void FlameAttack()
    {
        if (monsterData.Timer >= 10f)
        {
            Debug.Log("flame");
            monsterData.Timer = 0f;
            animator.SetTrigger("Breathe");
            nmAgent.speed = 0f;
        }
        else
        {
            nmAgent.speed = 2.5f;
        }
    }

    void AttackPlayerIfInRange(Vector3 playerPosition)
    {
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Attack");
        Debug.Log("Attack");
    }



    //�ִϸ��̼� �̺�Ʈ
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

    

    



    //�� �κк��� �Ʒ� �κ� ���� AI �Ŵ��� ��ũ��Ʈ�� �ű� ����

}
