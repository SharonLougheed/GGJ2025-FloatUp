using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent (typeof(Light))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways] // Ensures this script runs in the editor as well as in play mode
public class StarFlicker : MonoBehaviour
{
	public float flickerSpeed = 5f;
	public float minIntensity = 0.2f;
	public float maxIntensity = 1.0f;
	public float minScale = 0.5f;
	public float maxScale = 1f;
	public float initScale = 1f;
	public bool doScaleInit = true;
	public bool testInEditMode = true;

	private Light starLight;
	private Color originalLightColor;
	private Material starMaterial;
	private Color originalEmissiveMaterialColor;
	private float randomOffset;
	private float prevInitScale;
	private Vector3 prevTransformScale;
	private bool doTwinkle = true;

	void Start()
	{
		initScale = transform.localScale.x;
		prevInitScale = initScale;
		prevTransformScale = transform.localScale;

		// Randomize flicker pattern
		randomOffset = UnityEngine.Random.Range(0f, 100f);
		MeshRenderer starRenderer = gameObject.GetComponent<MeshRenderer>();
		starMaterial = new Material(starRenderer.sharedMaterial);
		starRenderer.material = starMaterial;

		if (starMaterial != null && starMaterial.HasProperty("_EmissionColor"))
		{
			originalEmissiveMaterialColor = starMaterial.GetColor("_EmissionColor");
		}
		starLight = gameObject.GetComponent<Light>();
		originalLightColor = starLight.color;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			if (prevInitScale != initScale)
			{
				prevTransformScale = transform.localScale;
				transform.localScale = new Vector3(initScale, initScale, initScale);
				prevInitScale = initScale;
			}
			else if (prevTransformScale != transform.localScale)
			{
				float changedScale = prevTransformScale.x;
				if (prevTransformScale.x != transform.localScale.x)
				{
					changedScale = transform.localScale.x;
				}
				else if (prevTransformScale.y != transform.localScale.y)
				{
					changedScale = transform.localScale.y;
				}
				else if (prevTransformScale.z != transform.localScale.z)
				{
					changedScale = transform.localScale.z;
				}
				prevTransformScale = transform.localScale;
				transform.localScale = new Vector3(changedScale, changedScale, changedScale);
				prevInitScale = changedScale;
			}
		}
	}

#endif

	void Update()
	{
#if UNITY_EDITOR // Don't want to do this check in a real build, ik ik, useless overoptimization
		if (doTwinkle && (testInEditMode || Application.isPlaying))
		{
#endif
			// Flicker the light intensity
			float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
			float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
			if (starLight != null)
				starLight.intensity = intensity;

			// Flicker the size of the star (scale)
			float scale = Mathf.Lerp(minScale* initScale, maxScale* initScale, noise);
			transform.localScale = new Vector3(scale, scale, scale);

#if UNITY_EDITOR
		}
#endif
	}


	private IEnumerator FlashCoroutine(Color flashColor, float flashDuration, Action onStart = null,  Action onComplete = null)
	{
		starMaterial.SetColor("_EmissionColor", flashColor);
		starLight.color = flashColor;
		yield return new WaitForSeconds(flashDuration);
		starMaterial.SetColor("_EmissionColor", originalEmissiveMaterialColor);
		starLight.color = originalLightColor;
		onComplete?.Invoke();
	}

	private IEnumerator BlinkCoroutine(Color flashColor, float flashDuration, float delayBetweenFlashes, int numOfFlashes, Action onStart = null, Action onComplete = null)
	{
		onStart?.Invoke();
		doTwinkle = false;
		for (int i = 0; i < numOfFlashes; i++)
		{
			starMaterial.SetColor("_EmissionColor", flashColor);
			starMaterial.DisableKeyword("_EMISSION");
			starLight.color = flashColor;
			yield return new WaitForSeconds(flashDuration);
			starMaterial.SetColor("_EmissionColor", originalEmissiveMaterialColor);
			starMaterial.EnableKeyword("_EMISSION");
			starLight.color = originalLightColor;
			yield return new WaitForSeconds(delayBetweenFlashes);
		}
		doTwinkle = true;
		onComplete?.Invoke();
	}

	public void Flash(Color color, float duration, Action onStart = null, Action onComplete = null)
	{
		if (starMaterial != null && starMaterial.HasProperty("_EmissionColor"))
		{
			StartCoroutine(FlashCoroutine(color, duration, onStart, onComplete));
		}
	}

	public void Blink(Color color, float flashDuration, float delayBetweenFlashes, int numOfFlashes, Action onStart = null, Action onComplete = null)
	{
		if (starMaterial != null && starMaterial.HasProperty("_EmissionColor"))
		{
			StartCoroutine(BlinkCoroutine(color, flashDuration, delayBetweenFlashes, numOfFlashes, onStart, onComplete));
		}
	}
}
