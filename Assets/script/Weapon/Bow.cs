using StarterAssets;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Bow : MonoBehaviour
{
    private Animator animator;
    public Animator characterAnimator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the Bow object.");
        }

        // ĳ������ Hold ���� ���� �̺�Ʈ ����
        
    }

    // Hold ���°� ����� �� ȣ��Ǵ� �޼���
    private void OnHoldStateChanged(bool hold)
    {
        // Ȱ �ִϸ������� Draw ���� ����
        animator.SetBool("Draw", hold);
    }
}
