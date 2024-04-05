using StarterAssets;
using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	public LayerMask pushLayers;
	public LayerMask climbLayers;
	public bool canPush;
	public bool canClimb;
	
	[Range(0.5f, 5f)] public float strength = 1.1f;


    private ThirdPersonController thirdPersonController; // ThirdPersonController 컴포넌트에 대한 참조 변수

    private void Start()
    {
        // Start 메서드에서 ThirdPersonController 컴포넌트에 대한 참조를 가져옵니다.
        thirdPersonController = GetComponent<ThirdPersonController>();

        // 만약 ThirdPersonController 컴포넌트가 없다면 에러를 출력합니다.
        if (thirdPersonController == null)
        {
            Debug.LogError("ThirdPersonController 컴포넌트를 찾을 수 없습니다!");
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (canPush) PushRigidBodies(hit);

		if (canClimb) ClimbCheck();


	}

	private void PushRigidBodies(ControllerColliderHit hit)
	{
		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

		// make sure we hit a non kinematic rigidbody
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;

		// make sure we only push desired layer(s)
		var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0) return;

		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3f) return;

		// Calculate push direction from move direction, horizontal motion only
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

		// Apply the push and take strength into account
		body.AddForce(pushDir * strength, ForceMode.Impulse);
	}

    private void ClimbCheck()
    {
        RaycastHit hit;

        // 캐릭터의 위치와 방향으로 레이를 쏩니다.
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            // 레이가 충돌한 객체의 레이어를 가져옵니다.
            int objectLayer = hit.collider.gameObject.layer;

            // 충돌한 객체의 레이어가 climbLayer에 속해 있는지 확인합니다.
            if (climbLayers == (climbLayers | (1 << objectLayer)))
            {
                Debug.Log("Climbing");

                // isClimb 플래그를 true로 설정합니다.
                thirdPersonController.canClimb = true;
            }
        }
        else
        {
            // isClimb 플래그를 false로 설정합니다.
            thirdPersonController.canClimb = false;
        }
    }



}