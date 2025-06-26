using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Health Potion")]
public class HealthPotion : ItemData
{
	[Tooltip("Percent of Player Heal Amount by Potion"), 
		Range(1f, 100f), SerializeField]
	private float healRatio = 25f;

	public override void Apply(Player player)
	{
		if (player == null) return;

		float healAmount = player.PlayerMaxHealth * (healRatio / 100);
		player.Heal(healAmount);

		Debug.Log("Potion Apply Heal to Player" + healAmount);
	}
}
