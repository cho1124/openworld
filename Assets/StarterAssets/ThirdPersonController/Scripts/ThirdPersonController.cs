using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.Windows;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _animationBlendLR;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        public Transform armTransform;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _attackTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAttack;
        private int _animIDHold;
        
        
        private int _animIDDefence;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        public Animator childAnimator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        public GameObject _attackObject;
        private Collider attackCollider;
        private CharacterStat characterStat;
        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool _isAttacking;
        private bool isAiming = false;
        public float _attackCooldown = 1.0f; // 콤보를 유지할 시간
        public int maxComboCount = 3; // 최대 콤보 횟수

        private int _comboCount = 0; // 콤보 카운터
        

        private bool _canMove = true; // 공격중이거나, 방어중일 때 움직임 값을 받지 않도록 함
        private bool holdTimerStarted = false;
        private float holdTimer = 0f;
        private float holdTimeThreshold = 1.0f; // 홀드가 활성화되는 시간(예: 1초)

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            characterStat = GetComponent<CharacterStat>();
            if (_attackObject != null)
            {
                attackCollider = _attackObject.GetComponent<Collider>();
            }

#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _attackTimeoutDelta = JumpTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            if (!characterStat.isDead)
            {
                // 캐릭터의 입력 처리
                // 예를 들어 이동, 점프, 공격 등의 입력을 처리합니다.
                GroundedCheck();
                
                JumpAndGravity();
                Attack();
                Block();
                ChargeAttack();
                if(_canMove)
                {
                    Move();
                }

            }
        }

        private void LateUpdate()
        {
            CameraRotation();

            
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDHold = Animator.StringToHash("Hold");
            _animIDDefence = Animator.StringToHash("Defence");
            
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // 입력이 있고 카메라 위치가 고정되지 않았을 때
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // 마우스 입력에 Time.deltaTime을 곱하지 않습니다.
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // 마우스 입력 값에 deltaTimeMultiplier를 곱합니다.
                float mouseX = _input.look.x * deltaTimeMultiplier;
                float mouseY = _input.look.y * deltaTimeMultiplier;

                // Yaw와 Pitch 값을 업데이트합니다.
                _cinemachineTargetYaw += mouseX;
                _cinemachineTargetPitch += mouseY;
            }

            

            // 에임 상태일 때는 카메라의 회전을 자유롭게 설정합니다.
            if (isAiming)
            {
                // 카메라의 회전만 변경하는 것이 아니라 캐릭터의 회전도 함께 변경합니다.
                Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
                CinemachineCameraTarget.transform.rotation = targetRotation;
                transform.rotation = Quaternion.Euler(0f, _cinemachineTargetYaw, 0f); // Yaw값만 사용하여 캐릭터를 회전합니다.
                armTransform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, armTransform.localEulerAngles.y, armTransform.localEulerAngles.z);


            }

            // 회전 값을 360도 범위로 제한합니다.
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            

            // Cinemachine이 이 타겟을 따라가도록 회전 값을 설정합니다.
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);

        }


        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            _animationBlendLR = Mathf.Lerp(_animationBlendLR, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlendLR < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (!isAiming && _input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            }

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);
            

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            Vector3 moveDirection = Vector3.zero;
            if (!isAiming && _input.move != Vector2.zero)
            {
                moveDirection = targetDirection;
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else if (isAiming && _input.move != Vector2.zero)
            {
                moveDirection = _input.move.x * transform.right + _input.move.y * transform.forward;
            }

            _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                if (!isAiming)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                }
                else
                {
                    

                    _input.sprint = false;

                    // 조준 상태에서 키 입력에 따라 애니메이션 속도 설정
                    if (_input.move.x > 0)
                    {
                        _animator.SetFloat(_animIDSpeed, _animationBlend * 0.5f);

                    }
                    else if (_input.move.x < 0)
                    {
                        _animator.SetFloat(_animIDSpeed, -_animationBlend * 0.5f);
                    }
                    else
                    {
                        if(_input.move.y == 0)
                        {
                            _animator.SetFloat(_animIDSpeed, _animationBlend);
                        }

                        
                    }
                    

                    if (_input.move.y > 0)
                    {
                        _animator.SetFloat("SpeedLR", _animationBlendLR * 0.5f);
                    }
                    else if (_input.move.y < 0)
                    {
                        _animator.SetFloat("SpeedLR", -_animationBlendLR * 0.5f);
                    }
                    else
                    {
                        if (_input.move.x == 0)
                        {
                            _animator.SetFloat("SpeedLR", _animationBlend);
                        }
                    }
                   
                }
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }



        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    Debug.Log("jumped");
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void Attack()
        {
            if (!Grounded)
            {
                _input.attack = false;
                _attackTimeoutDelta = JumpTimeout;
                
            }

            if (_input.attack && _canMove)
            {
                _input.attack = false;
                Debug.Log("Attacked");
                _isAttacking = true;
                _canMove = false;
                

                if (_animator)
                {
                    
                    _animator.SetTrigger(_animIDAttack);
                    StartCoroutine(ActivateAttackObjectCoroutine());
                }
            }
            


        }

        private void ChargeAttack()
        {
            if (!Grounded)
            {
                _input.hold = false;
            }

            if (_input.hold && _canMove)
            {
                _isAttacking = true;
                //_canMove = false;
                

                if (_animator)
                {

                    if(!_animator.GetBool("UseSword"))
                    {
                        //flag 함수
                        isAiming = true;
                        childAnimator.SetBool("Draw", true);

                    }
                    else
                    {
                        _canMove = false;
                    }

                    _animator.SetBool(_animIDHold, true);
                    
                }
            }

            if (_input.holdout && _animator.GetBool(_animIDHold))
            {
                if(!_animator.GetBool("UseSword"))
                {
                    isAiming = false;
                    childAnimator.SetBool("Draw", false);
                }


                _animator.SetBool(_animIDHold, false);
                
                
                _canMove = true;
            }
        }



        private void Block()
        {

            // _input.defense 값이 false일 때 애니메이션을 중지합니다.
            if (!_input.defense)
                
            {
                if (_animator)
                {
                    _animator.SetBool(_animIDDefence, false);
                }
            }
            // _input.defense 값이 true일 때 애니메이션을 시작합니다.
            else
            {
                if (_animator)
                {
                    _animator.SetBool(_animIDDefence, true);
                }
            }


        }


        //애니메이션에 사용되는 함수들
        private void enableAttackCol()
        {
            attackCollider.enabled = true;
        }

        private void disableAttackCol()
        {
            attackCollider.enabled = false;
        }

        private void shootStart()
        {
            _animator.SetBool("Shoot", true);
        }

        private void shootEnd()
        {
            _animator.SetBool("Shoot", false);
        }

        private void bowPull()
        {
            childAnimator.SetTrigger("Draw");
        }

        private void bowOverDraw()
        {

        }

        private void bowRelease()
        {
            childAnimator.SetTrigger("Release");
        }



        IEnumerator ActivateAttackObjectCoroutine()
        {
            

            // 애니메이션이 끝날 때까지 대기
            yield return new WaitForSeconds(0.5f);

            
            

            // 공격 상태 초기화

            
                _isAttacking = false;
                isAiming = false;
            

            
            _canMove = true;
            
            //_animator.SetBool(_animIDAttack, false);
            //_animator.SetInteger(_animIDAttack, 0);
            //_input.attack = false;

        }

    }
}