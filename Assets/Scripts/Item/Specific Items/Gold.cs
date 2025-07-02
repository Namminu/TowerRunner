using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Gold")]
public class Gold : ItemData
{
	[SerializeField, Tooltip("Max Range to Gold Amount"), Range(100, 1000)]
	private int maxRange;

	public override void Apply(Player player)
	{
		player.ItemChecker.GetCoin(GetRandomCoin());
	}

	private int GetRandomCoin()
	{
		return Random.Range(0, maxRange);
	}
}
