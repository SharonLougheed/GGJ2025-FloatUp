using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BubbleController))]
public class FriendlyBubbleController : MonoBehaviour
{
    public int rangeOfSight;
	public bool followingPlayer = false;
	public float delayBetweenPushesSeconds = 1f;
	public float strengthToMove = 1f;
	public GameObject star;
	private Vector3 playerPos;
	private BubbleController bubbleController;
	private Coroutine currentCoroutine = null;
	private GameObject playerObject;
	private BubblePlayer player;

	private void Start()
	{
		playerObject = GameObject.FindGameObjectWithTag("Player");
		player.GetComponentInParent<BubblePlayer>();
		playerPos = playerObject.transform.position;
		bubbleController = GetComponent<BubbleController>();
	}

	private void Update()
	{
        //if ((true))
        //{
            
        //}
        //Vector3 direction = Vector3.Angle(transform.position, playerPos);
		//bubbleController.Push(direction, strengthToMove);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == playerObject)
		{
			// TODO: Separate bubble and camera, cuz I can't put "Player" on the parent of the bubble, that's weird
			
			followingPlayer = true;
			//currentCoroutine = StartCoroutine(FollowPlayer());
			//player.AddBabble(this);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// TODO: Separate bubble and camera, cuz I can't put "Player" on the parent of the bubble, that's weird
			BubblePlayer player = collision.gameObject.GetComponentInParent<BubblePlayer>();
			if (player != null)
			{
				player.RemoveBabble(this);
				followingPlayer = false;
				currentCoroutine = null;
			}
		}
	}



	//private IEnumerator FollowPlayer()
	//{
	//	MoveTowards
	//	//player.
	//	//yield return new WaitForSeconds(delayBetweenPushesSeconds);
	//}

	public void AddStar()
	{

	}
}
