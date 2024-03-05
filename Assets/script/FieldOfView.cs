using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Animator animator;

    // �þ� ������ �������� �þ� ����
    public float viewRadiusHorizontal;
    [Range(0, 360)]
    public float viewAngleHorizontal;

    public float viewRadiusVertical;
    [Range(0, 360)]
    public float viewAngleVertical;

    public float chaseDistance = 50f; // ������ �Ÿ�

    private Transform playerTransform;



    // ����ũ 2��
    public LayerMask targetMask, obstacleMask;

    // Target mask�� ray hit�� transform�� �����ϴ� ����Ʈ
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        animator = GetComponent<Animator>();

        // 0.2�� �������� �ڷ�ƾ ȣ��
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
            float angleHorizontal = Vector3.Angle(transform.forward, new Vector3(dirToTarget.x, 0, dirToTarget.z)); // ���� �þ� ���� ���
            float angleVertical = Vector3.Angle(transform.forward, dirToTarget); // ���� �þ� ���� ���

            if (angleHorizontal < viewAngleHorizontal / 2 && angleVertical < viewAngleVertical / 2) // ���� �� ���� �þ� ���� ���� �ִ��� Ȯ��
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
            playerTransform = visibleTargets[0]; // ���� ����� �÷��̾� ���� (���� ���� ù ��° ��ҷ� �����Ǿ� ����)

            // ���� ���·� ��ȯ
            animator.SetBool("InCombat", true); // ���� ���� Ʈ���� ����

            // �÷��̾� ����
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            // ���� ���� ���� ���� �̾ �����Ͻʽÿ�.
        }
        else if (animator.GetBool("InCombat")) // ���� ���¿����� �˻�
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= chaseDistance)
            {
                MoveTowardsPlayer(playerTransform.position);
                AttackPlayerIfInRange(playerTransform.position);
                return;
            }
            // �÷��̾ ���� ������ ����� ���� ���� ����
            animator.SetBool("InCombat", false);
        }
    }

    void MoveTowardsPlayer(Vector3 playerPosition)
    {
        // �÷��̾� ������ �̵��ϴ� �ڵ带 ���⿡ �߰�
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

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngleHorizontal / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleTargets�� Add
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
            float angleVertical = Vector3.Angle(transform.forward, dirToTarget); // ���Ϳ� Ÿ�� ������ ���� ���� ���

            if (angleVertical < viewAngleVertical / 2) // ���� �þ� ���� ���� �ִ��� Ȯ��
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    // y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ�Ѵ�.
    // ������ ������ ��¦ �ٸ��� ����. ����� ����.
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