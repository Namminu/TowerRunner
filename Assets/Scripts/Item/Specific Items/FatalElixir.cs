using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Fatal Elixir")]
public class FatalElixir : ItemData
{
	[SerializeField, Tooltip("Player Fatal Hit Chance Weighting"), Range(10, 30)]
	private int maxRange;

	[SerializeField, Range(10f, 30f)]
	private float duration = 10f;

	public override void Apply(Player player)
	{
		player.ItemChecker.ApplyFatalElixir(maxRange);

		TimerUIManager.Instance.StartTimer(itemIcon, duration, () =>
		{
			player.ItemChecker.RemoveFatalElixir(maxRange);
		});
	}
}
