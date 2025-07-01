using UnityEngine;

public class CoinMagnet : ItemData
{
	[SerializeField, Range(10f, 30f)]
	private float duration = 10f;

	[SerializeField, Tooltip("Magnet Range Increase Rate"), Range(10f, 30f)]
	private float range = 10f;

	public override void Apply(Player player)
	{

	}
}
