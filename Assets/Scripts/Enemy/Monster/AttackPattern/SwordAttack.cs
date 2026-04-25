using System.Collections;
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
		contactFilter.SetLayerMask(LayerMask.GetMask("PlayerBody"));
		contactFilter.useLayerMask = true;
		contactFilter.useTriggers = true;
	}

	protected override IEnumerator AttackRoutine(float dmg, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		BoxCollider2D collider = attackRange as BoxCollider2D;
		Vector2 worldPos = (Vector2)attackRange.transform.position +
			(Vector2)(attackRange.transform.rotation * collider.offset);

		int hitCount = Physics2D.OverlapBox(
			worldPos, collider.size, attackRange.transform.eulerAngles.z,
			contactFilter, results);

		for (int i = 0; i < hitCount; i++)
		{
			var col = results[i];
			Player player = col.GetComponentInParent<Player>();
			if(player)
			{
				player.TakeDamage(dmg);
			}
		}
	}
}
