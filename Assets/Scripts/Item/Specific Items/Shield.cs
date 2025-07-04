using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Shield")]
public class Shield : ItemData
{
	public override void Apply(Player player)
	{
		player.ItemChecker.GetShield();
	}
}
