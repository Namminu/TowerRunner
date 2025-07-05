using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 객체 속성
/// </summary>
public interface IDamageable
{
	void TakeDamage(float amount);
}

/// <summary>
/// 피격 데미지를 줄 수 있는 객체 속성
/// </summary>
public interface IDamageDealer
{
	void DealDamage(IDamageable target);
}

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamageDealer, IPoolable
{
	[SerializeField]
	private float _hitDamage;
	public float HitDamage => _hitDamage;

	[SerializeField]
	protected float enemyMaxHealth;
	protected float enemyCurHealth;

	public BaseEnemy Prefab { get; internal set; }

	protected virtual void Awake()
	{
		enemyCurHealth = enemyMaxHealth;
	}

	protected virtual void Death()
	{
		EnemyPoolingManager.Instance.Despawn(this);
	}

	public virtual void TakeDamage(float amount)
	{
		enemyCurHealth -= amount;
		if (enemyCurHealth <= 0f)
			Death();
	}

	public void DealDamage(IDamageable target)
	{
		target.TakeDamage(_hitDamage);
	}

	public virtual void OnSpawn()
	{
		enemyCurHealth = enemyMaxHealth;
		enabled = true;
		if (GetComponent<Collider2D>() is Collider2D col)
			col.enabled = true;
		if (GetComponent<Animator>() is Animator ani)
			ani.Play("Idle");
	}

	public virtual void OnDespawn()
	{
		enabled = false;
		StopAllCoroutines();
		if (GetComponent<Collider2D>() is Collider2D col)
			col.enabled = false;
	}

	protected void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("PLAYER")) return;
	
		if(col.TryGetComponent<Player>(out var player))
			DealDamage(player);
	}
}
