using UnityEngine;

public abstract class BaseMonster : BaseEnemy, IDamageable
{
	[SerializeField]
	protected float _attackDamage;
	public float AttackDamage => _attackDamage;

	[SerializeField]
	protected AttackPattern attackPattern;

	[SerializeField]
	protected float enemyMaxHealth;
	protected float enemyCurHealth;


	protected virtual void Awake()
	{
		enemyCurHealth = enemyMaxHealth;
	}

	protected void Death()
	{
		ItemData dropItem = ItemDatabase.GetRandomDrop();
		if (dropItem != null)
		{
			ItemPoolingManager.Instance.Spawn(dropItem,transform.position);
		}
		EnemyPoolingManager.Instance.Despawn(this);
	}

	public virtual void TakeDamage(float amount)
	{
		enemyCurHealth -= amount;
		if (enemyCurHealth <= 0f)
			Death();
	}
}