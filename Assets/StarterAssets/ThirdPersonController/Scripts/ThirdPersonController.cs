﻿using System.Collections;
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
        public bool isClimbing = false;
        private bool holdSuccessed = false;
        public float _attackCooldown = 1.0f; // 콤보를 유지할 시간
        public int maxComboCount = 3; // 최대 콤보 횟수

        private int _comboCount = 0; // 콤보 카운터
        

        private bool _canMove = true; // 공격중이거나, 방어중일 때 움직임 값을 받지 않도록 함
        
        private bool arrowSpawned = false;
        
        private Rigidbody arrowRigidbody;

        public bool ikActive = false;
        public Transform rightHandObj = null;
        public Transform lookObj = null;
        public GameObject ArrowObj = null;
        private GameObject arrow = null;
        public GameObject arrowEffect = null;


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

             

                if (_canMove)
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
                armTransform.localRotation = Quaternion.Euler(armTransform.localEulerAngles.x, armTransform.localEulerAngles.y, _cinemachineTargetPitch);

                
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

                    // 이동 방향 벡터
                    

                    // 이동 방향 벡터의 크기를 가져옵니다
                    float moveMagnitude = _input.move.magnitude;

                    // 이동 방향 벡터가 없는 경우를 고려하여 분모를 0으로 만들지 않습니다.
                    float speedX = (moveMagnitude != 0) ? (_animationBlend * 0.5f * _input.move.x ) : _animationBlend * 0.5f;
                    float speedY = (moveMagnitude != 0) ? (_animationBlendLR * 0.5f * _input.move.y ) : _animationBlendLR * 0.5f;

                    // 애니메이터에 속도값을 설정합니다.
                    _animator.SetFloat(_animIDSpeed, speedX);
                    _animator.SetFloat("SpeedLR", speedY);




                }
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void WallMove()
        {
            // 이동 속도 설정
            float targetSpeed = MoveSpeed * 0.5f;

            // 만약 입력값이 없다면 목표 속도를 0으로 설정하여 정지 상태로 만듭니다.
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            // 현재 수평 속도를 계산합니다.
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            // 이동 속도를 조절하기 위한 오프셋 값 설정
            float speedOffset = 0.1f;

            // 현재 속도가 목표 속도와 일정 범위 이내에 있는지 확인하여 조절합니다.
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // 현재 속도를 목표 속도로 서서히 변화시킵니다.
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                // 현재 속도가 목표 속도와 일치하면 목표 속도를 유지합니다.
                _speed = targetSpeed;
            }

            // 캐릭터를 이동시킵니다.
            Vector3 moveDirection = transform.forward;
            _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
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
            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(5);
            
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
                    
                    if (!_animator.GetBool("UseSword"))
                    {
                        //flag 함수
                        isAiming = true;
                        //childAnimator.SetBool("Draw", true);

                        if(!arrowSpawned)
                        {
                            arrow = Instantiate(ArrowObj, rightHandObj.position, rightHandObj.rotation);
                            
                            arrow.transform.SetParent(rightHandObj.transform);
                            arrowSpawned = true;
                            
                            //arrowTrailEffect.Stop();
                            arrowRigidbody = arrow.GetComponent<Rigidbody>();
                            
                            if (arrowRigidbody == null)
                            {
                                arrowRigidbody = arrow.AddComponent<Rigidbody>();
                            }
                            arrowRigidbody.isKinematic = true;

                        }

                    }
                    else
                    {
                        _canMove = false;
                    }

                    _animator.SetBool(_animIDHold, true);
                    _animator.SetBool("AimHold", true);


                }
            }

            if (!_input.hold && _animator.GetBool(_animIDHold))
            {
                if (!_animator.GetBool("UseSword"))
                {
                    //isAiming = false;

                    if(!currentState.IsName("AimOverdraw"))
                    {
                        isAiming = false;
                        Destroy(arrow);
                    }

                    childAnimator.SetBool("Draw", false);
                    arrowSpawned = false;


                }


                
                _animator.SetBool(_animIDHold, false);

                
                _animator.SetBool("AimHold", false);
                if (currentState.IsName("AimOverdraw") && arrow != null)
                {
                    arrow.transform.SetParent(null); // 홀드를 놓을 때 화살의 부모를 해제합니다.
                    if (arrowRigidbody != null)
                    {
                        Debug.Log("Sasdsazx");
                        arrowRigidbody.isKinematic = false;
                    }
                    ShootArrow();
                }



                _canMove = true;
                

                
            }
        }

        private void ShootArrow()
        {
            if (arrowRigidbody != null)
            {
                // 애니메이션의 진행률을 얻어옵니다.
                float animationProgress = _animator.GetCurrentAnimatorStateInfo(5).normalizedTime;

                // 화살에 가해질 초기 힘의 크기를 설정합니다.
                float initialForce = 10f;

                // 애니메이션의 시간이 지날수록 힘의 크기를 증가시킵니다.
                float finalForce = initialForce + (animationProgress * 10f); // 애니메이션의 진행률에 따라 힘의 증가율을 조절합니다.

                // 화살을 발사합니다.
                arrowRigidbody.AddForce(CinemachineCameraTarget.transform.forward * finalForce, ForceMode.Impulse);

                // 화살의 위치를 기준으로 이펙트를 생성합니다.
                GameObject effect = Instantiate(arrowEffect, transform.position, transform.rotation);

                // 이펙트를 활성화시킵니다.
                effect.SetActive(true);

                // 화살과 이펙트를 연결합니다.
                ArrowEffectFollow arrowEffectFollow = effect.GetComponent<ArrowEffectFollow>();
                if (arrowEffectFollow != null)
                {
                    arrowEffectFollow.SetArrow(arrowRigidbody);
                }
            }
        }




        void AimingRelease()
        {
            isAiming = false;
        }

        

        private void OnAnimatorIK()
        {
            if (_animator)
            {
                if (!_animator.GetBool("UseSword") && _input.hold)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                if (!_animator.GetBool(_animIDHold))
                {
                    if (!_animator.GetBool("UseSword"))
                    {
                        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    }
                }
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
            childAnimator.SetBool("Draw", true);
        }

        private void bowOverDraw()
        {
            childAnimator.SetBool("Draw", true);
        }

        private void bowRelease()
        {
            childAnimator.SetBool("Draw", false);
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