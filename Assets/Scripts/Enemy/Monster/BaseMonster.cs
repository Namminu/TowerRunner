using UnityEngine;

public abstract class BaseMonster : BaseEnemy
{
	[SerializeField]
	protected float _attackDamage;
	public float AttackDamage => _attackDamage;

	[SerializeField]
	protected AttackPattern attackPattern;

	protected override void Death()
	{
		base.Death();
		ItemData dropItem = ItemDatabase.GetRandomDrop();
		if (dropItem != null)
		{
			ItemPoolingManager.Instance.Spawn(dropItem,transform.position);
		}

	}
}