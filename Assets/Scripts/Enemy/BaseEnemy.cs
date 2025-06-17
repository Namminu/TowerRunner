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

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamageDealer
{



	public virtual void TakeDamage(int amount)
	{

	}
	public void DealDamage(IDamageable target)
	{
	
	}
}
