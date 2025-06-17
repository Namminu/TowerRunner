using UnityEngine;

public interface IAttackPattern
{
	void Attack();
}

public abstract class BaseMonster : BaseEnemy
{
	[SerializeField]
	protected IAttackPattern attackPattern;
	[SerializeField]
	protected float damage;

	public float Damage => damage;
}
