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
