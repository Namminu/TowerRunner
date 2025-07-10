using UnityEngine;
using UnityEngine.AddressableAssets;

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

public abstract class BaseEnemy : MonoBehaviour, IDamageDealer, IPoolable
{
	public AssetReferenceGameObject PrefabRef { get; internal set; }

	[SerializeField]
	private float _hitDamage;
	public float HitDamage => _hitDamage;

	//public BaseEnemy Prefab { get; internal set; }

	public void DealDamage(IDamageable target)
	{
		target.TakeDamage(_hitDamage);
	}

	public virtual void OnSpawn()
	{
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

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("PLAYER")) return;
	
		if(col.TryGetComponent<Player>(out var player))
			DealDamage(player);
	}
}
