using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		mInputListen = GetComponent<PlayerInput>();
		mCharacterController = GetComponent<CharacterController>();
		mAnimator = GetComponentInChildren<Animator>();
    }
	public PlayerInput mInputListen;
    // Update is called once per frame
    void LateUpdate()
    {
		GroundedCheck();
		CameraRotation();
		JumpAndGravity();
		CrouchUpdate();
		if (Crouch) return;
		Move();
	}
	bool Crouch = false;
	void CrouchUpdate()
    {
		Crouch = mInputListen.crouch;
		if (Crouch)
        {
			mAnimator.SetBool("is_crouch", true);
			mCharacterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			mCharacterController.center = new Vector3( 0,-0.625f,0);
			mCharacterController.height = 0.75f;
		}
        else
        {
			mAnimator.SetBool("is_crouch", false);
			mCharacterController.center = new Vector3(0, -0.25f, 0);
			mCharacterController.height = 1.5f;
		}
    }
	public float _threshold = 0.1f;
	public float _cinemachineTargetYaw = 0;
	public float _cinemachineTargetPitch = 0;


	float BottomClamp = -30;
	float TopClamp = 70;
	public GameObject CinemachineCameraTarget;
	private void CameraRotation()
	{
		// if there is an input and camera position is not fixed
		if (mInputListen.look.sqrMagnitude >= _threshold)
		{
			_cinemachineTargetYaw += mInputListen.look.x * Time.deltaTime;
			_cinemachineTargetPitch += mInputListen.look.y * Time.deltaTime;
		}

		// clamp our rotations so our values are limited 360 degrees
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);



		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
		transform.localRotation = Quaternion.Euler(0.0f , _cinemachineTargetYaw, 0.0f);
	}
	float _rotationVelocity;

	public float moveSpeed;
	public CharacterController mCharacterController;
	public float _verticalVelocity = -9.8f;
	public Animator mAnimator;
	private void Move()
	{

		Vector3 inputDirection = new Vector3(mInputListen.move.x, 0, mInputListen.move.y);

		Vector3 targetDirection = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f) * inputDirection;

		mCharacterController.Move(targetDirection.normalized * (moveSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


		mAnimator.SetFloat("horizontal", inputDirection.normalized.x);
		mAnimator.SetFloat("vertical", inputDirection.normalized.z);
		mAnimator.SetFloat("moving_speed", targetDirection.magnitude);
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
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		// update animator if using character
	}
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;
	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;


	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.5f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.1f;
	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;
	private float _terminalVelocity = 53.0f;
	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;
			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (mInputListen.jump && _jumpTimeoutDelta <= 0.0f)
			{
				mAnimator.SetBool("is_jump", true);
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

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
				mAnimator.SetBool("is_jump", false);
			}

			// if we are not grounded, do not jump
			mInputListen.jump = false;
			
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}
}
