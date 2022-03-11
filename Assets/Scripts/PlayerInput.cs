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
	public bool cursorInputForLook = true;
	private void Update()
	{
		if (ChatMessage.instance != null && ChatMessage.instance.isTyping)
        {
			move = Vector2.zero;
			look = Vector2.zero;
			return;
		}
		if (cursorInputForLook)
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
}
