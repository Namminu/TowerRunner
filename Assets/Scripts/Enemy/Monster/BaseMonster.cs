using UnityEngine;

public abstract class BaseMonster : BaseEnemy
{
	[SerializeField]
	protected AttackPattern attackPattern;
	[SerializeField]
	protected float damage;

	public float Damage => damage;
}
