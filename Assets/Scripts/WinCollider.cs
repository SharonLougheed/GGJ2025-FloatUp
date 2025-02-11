using UnityEngine;

public class WinCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // TODO: Separate bubble and camera, cuz I can't put "Player" on the parent of the bubble, that's weird
            BubblePlayer player = collision.gameObject.GetComponentInParent<BubblePlayer>();
            if (player != null)
            {
                player.Win();
            }
        }
    }
}
