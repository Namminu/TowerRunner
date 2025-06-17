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

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamageDealer
{



	public virtual void TakeDamage(int amount)
	{

	}
	public void DealDamage(IDamageable target)
	{
	
	}
}
