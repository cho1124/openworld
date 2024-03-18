using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.Rendering.DebugUI;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool attack;
		public bool hold;
		public bool holdout;
		public bool defense;


		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        //public void OnAttack(InputValue value)
        //{
        //
        //	AttackInput(value.isPressed);
        //	
        //
        //}

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    Debug.Log("Hold");
                    HoldInput(true); // Hold 시작
					holdout = false;
					
                }
                else if (context.interaction is PressInteraction)
                {
                    Debug.Log("Pressed");
                    AttackInput(true); // Pressed 상태
                }
            }
            else if (context.canceled)
            {
                if (context.interaction is HoldInteraction)
                {
                    Debug.Log("Hold ended");
					holdout = true;
                }
            }
        }


        public void OnDefence(InputValue value)
		{
			DefenceInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;

		}

		public void HoldInput(bool newHoldState)
		{
			hold = newHoldState;
		}

		public void HoldOut(bool newOutState)
		{
			holdout = newOutState;
		}

		public void DefenceInput(bool newDefenceState)
		{
			defense = newDefenceState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}