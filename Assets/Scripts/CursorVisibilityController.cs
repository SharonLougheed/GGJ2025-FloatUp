using UnityEngine;
using UnityEngine.InputSystem;

public class CursorVisibilityController : MonoBehaviour
{
	void Start()
	{
		InputSystem.actions.FindAction("Escape").performed += ReleaseCursor;
		InputSystem.actions.FindAction("ClickInGame").performed += LockAndHideCursor;
	}

	private void ReleaseCursor(InputAction.CallbackContext context)
	{
		// Releases the cursor
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;		
	}

	private void LockAndHideCursor(InputAction.CallbackContext context)
	{
		// Hides and locks the cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}