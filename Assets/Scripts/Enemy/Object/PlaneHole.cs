using System.Collections;
using UnityEngine;

public class PlaneHole : BaseObject
{
	[SerializeField, Tooltip("Animation Time for Player Fall in PlaneHole")]
	private float fallDuration;

	protected override void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("PLAYER")) return;

		if (col.TryGetComponent<Player>(out var player))
		{
			if (player.IsDamageCoolDown || player.IsPlayerInvincible) return;

			GetComponent<Collider2D>().enabled = false;
			StartCoroutine(PlayerInHole(player));
		}
	}

	private IEnumerator PlayerInHole(Player player)
	{
		player.enabled = false;

		var startPos = player.transform.position;
		var capsule = GetComponent<CapsuleCollider2D>();
		var holeCenter = capsule.transform.TransformPoint(capsule.offset);
		var startScale = player.transform.localScale;

		float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
			float alpha = t / fallDuration;
			player.transform.position = Vector3.Lerp(startPos, holeCenter, alpha);
			player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, alpha);
			yield return null;
        }

        player.ImmediateDeath();
	}
}
