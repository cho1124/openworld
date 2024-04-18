using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiManager : MonoBehaviour
{

    private Animator animator;
    public MonsterData monsterData;
    NavMeshAgent nmAgent;
    public float chaseDistance = 10f; // 추적할 거리
    private Transform playerTransform; // 플레이어 위치 저장 변수
    public float combatSpeed = 1.0f; // 전투 속도
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
                        isPatrolDestinationSet = true; // 목적지가 설정되었음을 표시
                    }
                    
                    if (!nmAgent.pathPending && nmAgent.remainingDistance < 5.5f)
                    {
                        animator.SetBool("Patrol", false);
                        animator.SetFloat("Speed", 0f);
                        monsterData.currentAIState = MonsterData.MonsterAIState.Idle;
                        isPatrolDestinationSet = false; // Patrol이 끝났으므로 다음 패트롤 목적지를 설정할 수 있도록 허용
                    }
                    break;

                case MonsterData.MonsterAIState.Fly:

                    break;
                case MonsterData.MonsterAIState.Combat:
                    animator.SetBool("Patrol", false);
                    animator.SetBool("InCombat", true); // 전투 상태 트리거 설정
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
        
        if (monsterData.Timer >= 5f) // 예시로 10초가 지나면 Patrol 상태로 전환하는 조건을 사용합니다.
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
        //두 가지의 경우를 처리, fov를 통해 플레이어를 감지하거나, 데미지를 입어서 강제로 전투상태로 돌입하거나
        if(fieldOfView.playerTransform != null)
        {
            playerTransform = fieldOfView.playerTransform;
        }
        else
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            //근처에 Player태그를 가진 오브젝트를 탐색하여 그 오브젝트를 추적하도록 함, chaseDisntance밖에 있으면 추적을 종료하고 다시 기본상태로 돌아감, chasedistance안에 있으면 바로 추적을 시작함
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
            // 플레이어에게 다가가고 공격
            if (distanceToPlayer > monsterData.meleeAttackRange)
            {
                monsterData.Timer += delay;
                
                Debug.Log(monsterData.Timer);
                // 플레이어 쪽으로 이동
                FlameAttack();
                nmAgent.SetDestination(playerTransform.position);
                
                // 추적하는 영역 제한
                NavMeshHit hit;
                if (NavMesh.SamplePosition(playerTransform.position, out hit, chaseDistance, NavMesh.AllAreas))
                {
                    nmAgent.SetDestination(hit.position);
                }

                animator.SetFloat("Speed", combatSpeed);

                
            }
            else
            {
                // 플레이어 공격
                monsterData.Timer = 0f;
                AttackPlayerIfInRange(playerTransform.position);
            }
        }
        else
        {
            // 추적 범위를 벗어났으므로 추적을 멈춤
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



    //애니메이션 이벤트
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

    

    



    //이 부분부터 아래 부분 전부 AI 매니저 스크립트로 옮길 예정

}
