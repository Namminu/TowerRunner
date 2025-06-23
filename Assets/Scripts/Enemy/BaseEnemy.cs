using UnityEngine;

/// <summary>
/// �������� ���� �� �ִ� ��ü �Ӽ�
/// </summary>
public interface IDamageable
{
	void TakeDamage(int amount);
}

/// <summary>
/// �ǰ� �������� �� �� �ִ� ��ü �Ӽ�
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

	public BaseEnemy Prefab { get; protected set; }

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
		GetComponent<Collider2D>.enable = true;
		GetComponent<Animator>?.Play("Idle");
	}
	public virtual void OnDespawn()
	{
		enabled = false;
		StopAllCoroutines();
		GetComponent<Collider2D>.enable = false;
	}
}
