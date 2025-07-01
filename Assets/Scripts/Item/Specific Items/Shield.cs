using UnityEngine;

public class Shield : ItemData
{
	public override void Apply(Player player)
	{
		player.GetShield();
	}
}
