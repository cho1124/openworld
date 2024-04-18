using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    private Animator animator;
    public MonsterData monsterData;
    private Dragon dragon;

    // 시야 영역의 반지름과 시야 각도
    public float viewRadiusHorizontal;
    [Range(0, 360)]
    public float viewAngleHorizontal;

    public float viewRadiusVertical;
    [Range(0, 360)]
    public float viewAngleVertical;

    [Range(0, 100)]
    public float rotationSpeed;
    

    public float chaseDistance = 50f; // 추적할 거리

    public float combatSpeed = 1.0f; // 전투 속도
    public float idleSpeed = 1.0f;   // 대기 속도

    public Transform playerTransform;

    NavMeshAgent nmAgent;

    



    // 마스크 2종
    public LayerMask targetMask, obstacleMask;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    public List<Transform> visibleTargets = new List<Transform>();

    private void Awake()
    {
        dragon = GetComponent<Dragon>();
        if (dragon == null)
        {
            Debug.LogError("Dragon component not found on the GameObject!");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        nmAgent = GetComponent<NavMeshAgent>();

        // 0.2초 간격으로 코루틴 호출
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            TrackingTarget();
        }
    }

    //void FindVisibleTargets()
    //{
    //    visibleTargets.Clear();
    //
    //    FindVisibleTargetsHorizontal();
    //    FindVisibleTargetsVertical();
    //}

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, Mathf.Max(viewRadiusHorizontal, viewRadiusVertical), targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angleHorizontal = Vector3.Angle(transform.forward, new Vector3(dirToTarget.x, 0, dirToTarget.z)); // 수평 시야 각도 계산
            float angleVertical = Vector3.Angle(transform.forward, dirToTarget); // 수직 시야 각도 계산

            if (angleHorizontal < viewAngleHorizontal / 2 && angleVertical < viewAngleVertical / 2) // 수평 및 수직 시야 각도 내에 있는지 확인
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }


    void TrackingTarget()
    {
        if (visibleTargets.Count > 0)
        {
            playerTransform = visibleTargets[0]; // 가장 가까운 플레이어 추적 (현재 가장 첫 번째 요소로 설정되어 있음)

            // 전투 상태로 전환
           // animator.SetBool("InCombat", true); // 전투 상태 트리거 설정
            monsterData.currentAIState = MonsterData.MonsterAIState.Combat; //몬스터의 상태를 변경시키도록 하고 위의 애니메이터를 다루는 부분은 매니저 스크립트로 바꿀 예정
            

            
        }
    }

    void TrackPlayer()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= chaseDistance)
        {
            // 플레이어에게 다가가고 공격
            if (distanceToPlayer > dragon.meleeAttackRange)
            {
                // 플레이어 쪽으로 이동
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
                AttackPlayerIfInRange(playerTransform.position);
            }
        }
        else
        {
            // 추적 범위를 벗어났으므로 추적을 멈춤
            nmAgent.SetDestination(transform.position);
            animator.SetFloat("Speed", 0f);
            animator.SetBool("InCombat", false);
        }
    }//이 부분부터 아래 부분 전부 AI 매니저 스크립트로 옮길 예정

    void AttackPlayerIfInRange(Vector3 playerPosition)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        


    }

    void MoveTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        // 드래곤이 플레이어를 향하도록 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // 이동 방향으로 이동하기 위해 플레이어를 향해 이동합니다.
        transform.Translate(directionToPlayer * Time.deltaTime * combatSpeed);
        

        // 부드러운 회전을 위해 드래곤의 현재 회전값을 targetRotation으로 점진적으로 회전합니다.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        
        // 플레이어 쪽으로 이동하는 코드를 여기에 추가
    }

    

    // y축 오일러 각을 3차원 방향 벡터로 변환한다.
    // 원본과 구현이 살짝 다름에 주의. 결과는 같다.
    public Vector3 DirFromHorizontalAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    public Vector3 DirFromVerticalAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(0,  Mathf.Sin(angleDegrees * Mathf.Deg2Rad), Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

}