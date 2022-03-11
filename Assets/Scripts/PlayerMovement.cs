using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.FinalIK;
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
	public Cinemachine.CinemachineVirtualCamera vCamera;
	public AudioClip mfoot1, mfoot2, mjump, mland;
	AudioSource mSource;
    void Start()
    {
		mInputListen = GetComponent<PlayerInput>();
		mCharacterController = GetComponent<CharacterController>();
		mAnimator = GetComponentInChildren<Animator>();
		lastCapCenter = mCharacterController.center;
		lastCapHeight = mCharacterController.height;
		mSource = GetComponent<AudioSource>();
		if (photonView.IsMine)
        {
			if (FindObjectOfType<Cinemachine.CinemachineVirtualCamera>() != null)
			{
				FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = cameraAnchor;
				FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().LookAt = cameraAnchor;

			}
			
		}
        else
        {
				Destroy(GetComponentInChildren<PlayerLookTarget>());
        }
	}

	public void SendMessageFrom(string content)
    {
		photonView.RPC("SendMessageTo", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, content);
    }
	[PunRPC]
	void SendMessageTo(string sender, string value)
    {
		ChatMessage.instance.SendMessage(sender, value);
    }
	public Transform cameraAnchor, aimTarget;
	public PlayerInput mInputListen;
    // Update is called once per frame
    void Update()
    {
		if (!photonView.IsMine) return;
		GroundedCheck();
		CameraRotation();
		JumpAndGravity();
		CrouchUpdate();
		if (Crouch) return;
		Move();
	}
	bool Crouch = false;
	public Vector3 lastCapCenter;
	public float lastCapHeight;
	void CrouchUpdate()
    {
		Crouch = mInputListen.crouch;
		if (Crouch)
        {
			mAnimator.SetBool("is_crouch", true);
			mCharacterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			mCharacterController.center = new Vector3( 0, lastCapCenter.y - Mathf.Abs(lastCapCenter.y/2f), 0);
			mCharacterController.height = lastCapHeight / 2f;
		}
        else
        {
			mAnimator.SetBool("is_crouch", false);
			mCharacterController.center = lastCapCenter;
			mCharacterController.height = lastCapHeight;
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
	float deltaMoveStep = 0;
	bool mstep = false;
	private void Move()
	{


		Vector3 inputDirection = new Vector3(mInputListen.move.x, 0, mInputListen.move.y);

		Vector3 targetDirection = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f) * inputDirection;

		mCharacterController.Move(targetDirection.normalized * (moveSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


		mAnimator.SetFloat("horizontal", inputDirection.normalized.x);
		mAnimator.SetFloat("vertical", inputDirection.normalized.z);
		mAnimator.SetFloat("moving_speed", targetDirection.magnitude);
		deltaMoveStep += Time.deltaTime;
		if (Grounded && deltaMoveStep > 0.25f && targetDirection.magnitude > 0.25f)
		{
			mstep = !mstep;
			if (mstep)
            {
				mSource.PlayOneShot(mfoot1);
			}
            else
            {
				mSource.PlayOneShot(mfoot2);
			}
			deltaMoveStep = 0;
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
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}
	bool lastGrounded = false;
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		if (Grounded && lastGrounded != Grounded)
        {
			mSource.PlayOneShot(mland);
		}
		lastGrounded = Grounded;
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
				mSource.PlayOneShot(mjump);
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

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
			stream.SendNext(aimTarget.position);
		}
        else
        {
			aimTarget.position = (Vector3)stream.ReceiveNext();
        }
    }
}
