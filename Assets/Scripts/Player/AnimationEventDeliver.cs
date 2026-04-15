using UnityEngine;

public class AnimationEventDeliver : MonoBehaviour
{
	private Player player;

	private void Awake()
	{
		player = GetComponentInParent<Player>();
	}

	public void EndAttack()
	{
		if(player)
		{
			player.EndAttack();
		}
	}
}
