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
        // 활 시위를 당기는 애니메이션 시작
        animator.SetBool("Draw", true);
        animator.SetBool("Release", false);
    }

    public void StartReleasing()
    {
        // 활 시위를 놓는 애니메이션 시작
        animator.SetBool("Release", true);
        animator.SetBool("Draw", false);
    }
}