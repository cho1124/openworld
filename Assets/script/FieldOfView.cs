using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Animator animator;
    private Dragon dragon;

    // �þ� ������ �������� �þ� ����
    public float viewRadiusHorizontal;
    [Range(0, 360)]
    public float viewAngleHorizontal;

    public float viewRadiusVertical;
    [Range(0, 360)]
    public float viewAngleVertical;

    [Range(0, 100)]
    public float rotationSpeed;
    

    public float chaseDistance = 50f; // ������ �Ÿ�

    public float combatSpeed = 1.0f; // ���� �ӵ�
    public float idleSpeed = 1.0f;   // ��� �ӵ�

    private Transform playerTransform;

    


    // ����ũ 2��
    public LayerMask targetMask, obstacleMask;

    // Target mask�� ray hit�� transform�� �����ϴ� ����Ʈ
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


        if (animator.GetBool("InCombat")) // ���� ���¿����� �˻�
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            //Debug.Log(distanceToPlayer);
            if (distanceToPlayer <= chaseDistance)
            {
                
                TrackPlayer(distanceToPlayer);
                return;
            }
            // �÷��̾ ���� ������ ����� ���� ���� ����
            transform.position = new Vector3(0, 0, -20);
            transform.transform.rotation = Quaternion.identity;
            animator.SetFloat("Speed", 0);
            animator.SetBool("InCombat", false);
        }
    }


    void TrackPlayer(float distance)
    {
        // �÷��̾���� �Ÿ� ���
        

        // �÷��̾�� �ٰ����� ����
        if (distance > dragon.meleeAttackRange)
        {
            // �÷��̾� ������ �̵�
            MoveTowardsPlayer(playerTransform.position);

        }
        else
        {
            // �÷��̾� ����
            AttackPlayerIfInRange(playerTransform.position);
        }
    }

    void AttackPlayerIfInRange(Vector3 playerPosition)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        Debug.Log(distanceToPlayer);


    }

    void MoveTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        // �巡���� �÷��̾ ���ϵ��� ȸ���� ���
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // �̵� �������� �̵��ϱ� ���� �÷��̾ ���� �̵��մϴ�.
        transform.Translate(directionToPlayer * Time.deltaTime * combatSpeed);
        

        // �ε巯�� ȸ���� ���� �巡���� ���� ȸ������ targetRotation���� ���������� ȸ���մϴ�.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        animator.SetFloat("Speed", combatSpeed);
        // �÷��̾� ������ �̵��ϴ� �ڵ带 ���⿡ �߰�
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