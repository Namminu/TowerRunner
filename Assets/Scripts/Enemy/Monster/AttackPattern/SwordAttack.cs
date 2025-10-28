using TMPro.EditorUtilities;
using UnityEngine;

public class SwordAttack : AttackPattern
{
	[SerializeField]
	protected Collider2D attackRange;

	private ContactFilter2D contactFilter;
	private Collider2D[] results = new Collider2D[8];

	private void Awake()
	{
		contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask(LayerMask.GetMask("Player"));
		contactFilter.useTriggers = true;
	}

	public override void ExecuteAttack(float dmg)
	{
		int hitCount = attackRange.Overlap(contactFilter, results);
		for(int i = 0; i< hitCount; i++)
		{
			var col = results[i];
			if(col.TryGetComponent<Player>(out var player))
			{
				player.TakeDamage(dmg);
			}
		}
	}
}
