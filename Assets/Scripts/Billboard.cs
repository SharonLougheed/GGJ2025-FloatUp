using UnityEngine;

[ExecuteAlways] // Ensures this script runs in the editor as well as in play mode
public class Billboard : MonoBehaviour
{
	Camera mainCamera;
	void Start()
	{
		mainCamera = Camera.main;
	}

	void LateUpdate()
	{
		transform.LookAt(mainCamera.transform);
		transform.Rotate(0, 90, 90);
	}
}
