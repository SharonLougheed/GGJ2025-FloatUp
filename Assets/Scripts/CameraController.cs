using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Transform playerTransform;
	[SerializeField] Transform cameraTransform;

	[Header("Camera Settings")]
	[SerializeField] float minDistance = 0f; // 0 distance is fine, you can see through bubbles
	[SerializeField] float rotationSpeed = 15f;
	[SerializeField] LayerMask collisionMask;
	[SerializeField] float timeToZoomIfBlocked = 5f;
	[SerializeField] bool squishCamera = false;

	private float currentXRotation = 0f;
	private float currentYRotation = 0f;
	private Vector2 inputRotation;
	private bool hasRotateInput;
	private InputAction lookAction;
	private float minCameraZ;
	private float maxCameraZ;
	private float maxDistance;
	private float targetCameraZ;
	private float cameraX, cameraY;
	private bool isCameraBlocked;
	private float currTimeToZoom = 0f;

	private void Awake()
	{
		minCameraZ = -minDistance;
		maxCameraZ = cameraTransform.position.z;
		maxDistance = Math.Abs(maxCameraZ);
		targetCameraZ = minCameraZ;
		cameraX = cameraTransform.position.x;
		cameraY = cameraTransform.position.y;

		if (playerTransform.localPosition != Vector3.zero)
		{
			Debug.LogWarning("Player is not at the local position (0, 0, 0). CameraController expects the playerTransform to be centered.");
		}
	}

	private void Start()
	{
		lookAction = InputSystem.actions.FindAction("Look");

		if (lookAction == null)
		{
			Debug.LogError("Look action not found in the Input System!");
			return;
		}

		lookAction.started += OnRotate;
		lookAction.performed += OnRotate;
		lookAction.canceled += OnStopRotate;
	}

	private void OnRotate(InputAction.CallbackContext context)
	{
		if(Cursor.visible)
		{
			OnStopRotate(context);
		}
		else
		{
			inputRotation = context.ReadValue<Vector2>();
			hasRotateInput = true;
		}
	}
	private void OnStopRotate(InputAction.CallbackContext context)
	{
		inputRotation = Vector2.zero;
		hasRotateInput = false;
	}

	private void Update()
	{
		if (hasRotateInput && !isCameraBlocked)
		{
			HandleRotation();
		}
		if (squishCamera)
			HandleCollision();
	}

	private void HandleCollision()
	{
		// Ignore any X or Y offsets
		Vector3 cameraPosOnlyZ = new Vector3(0, 0, cameraTransform.localPosition.z);

		// SphereCast casts a bubble from the player bubble back to camera
		isCameraBlocked = Physics.SphereCast(playerTransform.position, playerTransform.localScale.x/2, Vector3.back, out RaycastHit hit, maxDistance, collisionMask);
		float nextCameraZ = targetCameraZ;

		if (isCameraBlocked)
		{
			targetCameraZ = -hit.distance;
		}
		else
		{
			targetCameraZ = maxCameraZ;
		}

		bool hasMoreToMove = Math.Abs(targetCameraZ - cameraTransform.localPosition.z) > 0.0001f;
		if (hasMoreToMove)
		{
			currTimeToZoom += Time.deltaTime;
			nextCameraZ = Mathf.Lerp(cameraTransform.localPosition.z, targetCameraZ, currTimeToZoom/timeToZoomIfBlocked);
			cameraTransform.localPosition = new Vector3(cameraX, cameraY, nextCameraZ);
		}
		else
		{
			currTimeToZoom = 0f;
		}
	}

	private void HandleRotation()
	{
		currentYRotation += inputRotation.x * rotationSpeed * Time.deltaTime;
		currentXRotation -= inputRotation.y * rotationSpeed * Time.deltaTime;

		// This actually rotates the parent of the camera, because the player should be center,
		// and I don't feel like using lookAt
		transform.localRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
	}
}
