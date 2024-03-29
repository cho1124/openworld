using UnityEngine;
using System.Collections;
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

        private bool holding = false;
        private float holdTimeThreshold = 1.5f; // Ȧ��� ������ �ּ� �ð� (���÷� 0.5��)

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput(context.ReadValue<Vector2>());
        }


        public void OnLook(InputAction.CallbackContext context)
        {

            if (cursorInputForLook)
            {
                LookInput(context.ReadValue<Vector2>());
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpInput(context.performed);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintInput(context.performed);
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

                if (context.interaction is TapInteraction && !context.canceled)
                {
                    AttackInput(true);
                }
                else if (context.interaction is HoldInteraction)
                {
                    //Debug.Log("Hold");
                    HoldInput(true); // Hold ����
                    holdout = false;
                    holding = true;
                    StartCoroutine(HoldCoroutine());
                }
                else if (context.interaction is PressInteraction && !context.canceled)
                {
                    Debug.Log("Pressed");
                    AttackInput(true); // Pressed ����
                                       //���⼭ ���� �߻�, context
                }



            }
            else if (context.canceled)
            {
                if (context.interaction is HoldInteraction)
                {
                    //Debug.Log("Hold ended");
                    HoldInput(false);
                    holdout = true;
                    holding = false;
                }
                else if (context.interaction is PressInteraction)
                {
                    Debug.Log("Pressed ended");
                    // Pressed�� �������� ��Ÿ���� �α� ��� �� �߰� ó��

                }
            }
        }

        private IEnumerator HoldCoroutine()
        {
            yield return new WaitForSeconds(holdTimeThreshold);

            if (holding)
            {
                Debug.Log("Shoot");
                // Ȧ�尡 ���� �ð� �̻� ���ӵǾ��� �� �߰� �۾� ����
            }
        }


        public void OnDefence(InputAction.CallbackContext context)
        {
            DefenceInput(context.performed);
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