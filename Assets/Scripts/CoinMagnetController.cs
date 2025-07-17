using System.Runtime.CompilerServices;
using UnityEngine;

public class CoinMagnetController : MonoBehaviour
{
	private CircleCollider2D circle;
	private float initialRadius;

	[SerializeField, Tooltip("Coin Adsorption Speed")]
	private float pullSpeed;

	private void Awake()
	{
		circle = GetComponent<CircleCollider2D>();
		if (circle == null)
			Debug.LogError("CircleCollider2D In CoinMangetCtrl Error");
		initialRadius = circle.radius;
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (!col.CompareTag("COIN")) return;

		Transform coinTf = col.transform;
		Vector2 targetPos = transform.parent.position;
		coinTf.position = Vector2.MoveTowards(
			coinTf.position, targetPos, pullSpeed * Time.deltaTime);
	}

	public void ActivateMagnet(float range)
	{
		circle.radius *= range;
		enabled = true;
	}

	public void DeactiveMagnet()
	{
		circle.radius = initialRadius;
		enabled = false;
	}
}
