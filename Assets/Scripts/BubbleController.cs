using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BubbleController : MonoBehaviour
{
	public float forwardStrength = 100.0f;
	public float ouchStrength = 200.0f;
	public float strafeStrength = 50.0f;
	public float timeToBrake;

	public float internalDensity = 0.1f; 
	public float externalDensity = 1.0f; // Water?
	public bool doFloat = true;
	public GameObject bubbleObject;
	public float bubbleVolumeScale = 0.05f;

	private float bubbleVolume; // Calculated dynamically
	private Rigidbody rb;
	private bool isBraking = false;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		UpdateBubbleVolume();
	}

	void UpdateBubbleVolume()
	{
		// Assuming the bubble is spherical: V = (4/3) * π * r³
		float radius = bubbleObject.transform.localScale.x / 2; // Adjust for scaling
		bubbleVolume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
		bubbleVolume = bubbleVolumeScale * bubbleVolume;
	}

	void FixedUpdate()
	{
		if(doFloat)
		{
			UpdateBubbleVolume();

			// Buoyant Force: (waterDensity - gasDensity) * volume
			float buoyancyForce = (externalDensity - internalDensity) * bubbleVolume * Physics.gravity.magnitude;
			rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
		}
	}
	public void PushForward()
	{
		Push(Vector3.forward, forwardStrength);
	}

	public void PushBack()
	{
		Push(Vector3.back, forwardStrength);
	}

	public void PushOuch()
	{
		Push(Vector3.back, ouchStrength);
	}

	public void PushLeft()
	{
		Push(Vector3.left, strafeStrength);
	}

	public void PushRight()
	{
		Push(Vector3.right, strafeStrength);
	}

	public void Push(Vector3 direction, float strength)
	{
		rb.AddRelativeForce(direction * strength, ForceMode.Force);
	}

	public void Brake()
	{
		StartCoroutine(ApplyBrakingForce());
	}

	private IEnumerator ApplyBrakingForce()
	{
		if (!isBraking)
		{
			isBraking = true;

			float brakingDuration = timeToBrake / 1000f;
			float startTime = Time.time;

			Vector3 initialVelocity = rb.linearVelocity;

			while (Time.time < startTime + brakingDuration)
			{
				float elapsed = Time.time - startTime;
				float t = Mathf.Clamp01(elapsed / brakingDuration);
				rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);

				yield return null;
			}

			rb.linearVelocity = Vector3.zero;

			isBraking = false;
		}
	}

	internal void Freeze()
	{
		doFloat = false;
		rb.linearVelocity = Vector3.zero;
	}
}
