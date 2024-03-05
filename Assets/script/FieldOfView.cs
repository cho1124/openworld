using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Animator animator;

    // 시야 영역의 반지름과 시야 각도
    public float viewRadiusHorizontal;
    [Range(0, 360)]
    public float viewAngleHorizontal;

    public float viewRadiusVertical;
    [Range(0, 360)]
    public float viewAngleVertical;

    public float chaseDistance = 50f; // 추적할 거리

    private Transform playerTransform;



    // 마스크 2종
    public LayerMask targetMask, obstacleMask;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        animator = GetComponent<Animator>();

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
            animator.SetBool("InCombat", true); // 전투 상태 트리거 설정

            // 플레이어 추적
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            // 추적 방향 설정 등을 이어서 구현하십시오.
        }
        else if (animator.GetBool("InCombat")) // 전투 상태에서만 검사
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= chaseDistance)
            {
                MoveTowardsPlayer(playerTransform.position);
                AttackPlayerIfInRange(playerTransform.position);
                return;
            }
            // 플레이어가 추적 범위를 벗어나면 전투 상태 종료
            animator.SetBool("InCombat", false);
        }
    }

    void MoveTowardsPlayer(Vector3 playerPosition)
    {
        // 플레이어 쪽으로 이동하는 코드를 여기에 추가
    }

    void AttackPlayerIfInRange(Vector3 playerPosition)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        
    }


    void FindVisibleTargetsHorizontal()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadiusHorizontal, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngleHorizontal / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void FindVisibleTargetsVertical()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadiusVertical, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angleVertical = Vector3.Angle(transform.forward, dirToTarget); // 몬스터와 타겟 사이의 수직 각도 계산

            if (angleVertical < viewAngleVertical / 2) // 수직 시야 각도 내에 있는지 확인
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
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