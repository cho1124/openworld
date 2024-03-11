using UnityEngine;

public class Bow : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartDrawing()
    {
        // Ȱ ������ ���� �ִϸ��̼� ����
        animator.SetBool("Draw", true);
        animator.SetBool("Release", false);
    }

    public void StartReleasing()
    {
        // Ȱ ������ ���� �ִϸ��̼� ����
        animator.SetBool("Release", true);
        animator.SetBool("Draw", false);
    }
}