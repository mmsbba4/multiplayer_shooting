using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool crouch;
	public bool fire;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	//public bool cursorLocked = true;
	public bool cursorInputForLook = true;
	private void Update()
	{
		if (cursorInputForLook /*&& cursorLocked*/)
		{
			float mouseX = Input.GetAxisRaw("Mouse X");
			float mouseY = Input.GetAxisRaw("Mouse Y");
			look = new Vector2(mouseX, -mouseY) * 100f;

		}
		else
		{
			look = Vector2.zero;
		}
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		move = new Vector2(horizontal, vertical);

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			crouch = true;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			crouch = false;
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			jump = true;
		}
		else
		{
			jump = false;
		}
		if (Input.GetButton("Fire1"))
        {
			fire = true;
        }
        else
        {
			fire = false;
        }
	}

	//private void Start()
	//{
	//	SetCursorState(cursorLocked);
	//}
	//private void OnApplicationFocus(bool hasFocus)
	//{
	//	SetCursorState(cursorLocked);
	//	Debug.Log("Application Focus " + hasFocus);
	//}
	//public void UpdateFocusStatus(bool value)
	//{
	//	cursorInputForLook = value;
	//	SetCursorState(cursorLocked);
	//}
	//public void SetCursorState(bool newState)
	//{
	//	print("update cursor lockstate");
	//	Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	//}
}
