using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BubbleController))]
public class BubblePlayer : MonoBehaviour
{
    [SerializeField] int soloHealth = 1;

	public UnityEngine.Color flashColor = UnityEngine.Color.red;
	public float flashDurationSeconds = 0.25f;
	public float delayBetweenFlashesSeconds = 0.1f;
	public int numOfFlashes = 6;
	public float minScale = 0.5f;
	public Action onPop = null;
	public Animator animator;
	public int maxBabbles = 10;

	private int health;
	private bool isDead = false;
	private bool didWin = false;
	private StarFlicker star;
	private BubbleController bubbleController;
	private GameObject bubbleObject;
	private bool isInvulnerable = false;
	private float maxScale;
	private int babbleCount = 0;

	void Start()
    {
		health = soloHealth;
        star = GetComponentInChildren<StarFlicker>();
		bubbleController = GetComponent<BubbleController>();
		bubbleObject = bubbleController.bubbleObject;
		maxScale = transform.localScale.x;
		if(minScale > maxScale)
		{
			Debug.LogError("Player bubble minScale > maxScale");
		}

		InputAction moveAction = InputSystem.actions.FindAction("Move");
		moveAction.started += OnMove;
		moveAction.performed += OnMove;
		animator.enabled = false;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		if (isDead || didWin) return;
		Vector2 moveInput = context.ReadValue<Vector2>();
		// W: Move Forward
		if (moveInput.y > 0)
		{
			bubbleController.PushForward();
		}
		// S: Move Backward
		else if (moveInput.y < 0)
		{
			bubbleController.PushBack();
		}

		// A/D: Strafe (Move sideways)
		if (moveInput.x < 0)
		{
			bubbleController.PushLeft();
		}
		else if (moveInput.x > 0)
		{
			bubbleController.PushRight();
		}
	}

	// Update is called once per frame
	void Update()
    {
    }

	internal void TakeDamage(int damageAmount)
	{
		if(!isInvulnerable)
		{
			SetInvulnerable(true);
			health--;
			if (health < soloHealth)
			{
				if(health <= 0)
				{
					Pop();
				}
				else
				{
					bubbleController.PushOuch();
					float healthRatio = (float)health / (float)soloHealth;
					float newScale = Mathf.Lerp(minScale, maxScale, healthRatio);
					bubbleObject.transform.localScale = new Vector3(newScale, newScale, newScale);
					star.Blink(flashColor, flashDurationSeconds, delayBetweenFlashesSeconds, numOfFlashes, null, () => SetInvulnerable(false));
				}
			}
			else
			{
				star.Blink(flashColor, flashDurationSeconds, delayBetweenFlashesSeconds, numOfFlashes, null, () => SetInvulnerable(false));
				// Pop friends here :(
			}
		}
	}

	private void Pop()
	{
		isDead = true;
		animator.enabled = true;
		animator.SetBool("Dead", true);
		bubbleController.Freeze();
		StartCoroutine(GoToMenu());
	}

	public void Win()
	{
		didWin = true;
		animator.enabled = true;
		bubbleController.enabled = false;
		animator.SetBool("Win", true);
		StartCoroutine(GoToMenu());
	}


	private IEnumerator GoToMenu()
	{
		yield return new WaitForSeconds(5f);
		SceneManager.LoadSceneAsync("SCR_Start");
	}

	internal void SetInvulnerable(bool makeInvulnerable)
	{
		isInvulnerable = makeInvulnerable;
	}

	//internal bool AddBabble(FriendlyBubbleController babble)
	//{
	//	babbleCount++;
	//	health++;
	//	if (babbleCount == 0)
	//	{
	//		babbleBubble = babble;
	//	}
	//	else
	//	{
	//		babbleBubble.AddStar(babble);
	//	}
	//}

	internal void RemoveBabble(FriendlyBubbleController babble)
	{
		babbleCount--;
	}
}
