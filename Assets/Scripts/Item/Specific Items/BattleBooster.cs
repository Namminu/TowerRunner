using UnityEngine;

public class BattleBooster : ItemData
{
	[SerializeField, Tooltip("Player Invincible Time"), Range(10f, 30f)]
	private float duration = 10f;

	public override void Apply(Player player)
	{
		player.ApplyBattleBooster(duration);

		TimerUIManager.Instance.StartTimer(itemIcon, duration, () =>
		{
			player.RemoveBattleBooster();
		});
	}
}
