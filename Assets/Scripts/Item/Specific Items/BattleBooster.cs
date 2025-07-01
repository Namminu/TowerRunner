using UnityEngine;

public class BattleBooster : ItemData
{
	[SerializeField, Range(10f, 30f)]
	private float duration = 10f;

	public override void Apply(Player player)
	{
		player.ApplyBattleBooster();

		TimerUIManager.Instance.StartTimer(itemIcon, duration, () =>
		{
			player.RemoveBattleBooster();
		});
	}
}
