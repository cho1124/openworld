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

        // 캐릭터의 Hold 상태 변경 이벤트 구독
        
    }

    // Hold 상태가 변경될 때 호출되는 메서드
    private void OnHoldStateChanged(bool hold)
    {
        // 활 애니메이터의 Draw 상태 변경
        animator.SetBool("Draw", hold);
    }
}
