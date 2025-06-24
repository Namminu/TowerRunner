using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 객체 속성
/// </summary>
public interface IDamageable
{
	void TakeDamage(int amount);
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
	protected int enemyMaxHealth;
	protected int enemyCurHealth;

	public BaseEnemy Prefab { get; internal set; }

	protected virtual void Awake()
	{
		enemyCurHealth = enemyMaxHealth;
	}
	public virtual void TakeDamage(int amount)
	{

	}
	public void DealDamage(IDamageable target)
	{
	
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
}
