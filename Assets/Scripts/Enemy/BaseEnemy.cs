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

/// <summary>
/// 몬스터가 생성하는 발사체 속성
/// </summary>
public interface IProjectile
{
	void InitializeProjectile(Transform tr, float damage);
}

public abstract class BaseEnemy : MonoBehaviour, IDamageDealer, IPoolable
{
	public AssetReferenceGameObject PrefabRef { get; internal set; }

	[SerializeField]
	private float _hitDamage;
	public float HitDamage => _hitDamage;

	private ObjectMover _mover;

	protected Collider2D _collider;

	protected virtual void Awake()
	{
		_mover = GetComponentInChildren<ObjectMover>();
		if (_mover == null)
			_mover = gameObject.AddComponent<ObjectMover>();

		_collider = GetComponentInChildren<Collider2D>();
	}

	public void DealDamage(IDamageable target)
	{
		target.TakeDamage(_hitDamage);
	}

	public virtual void OnSpawn()
	{
		enabled = true;
		if(_collider)
		{
			_collider.enabled = true;
		}
		else
		{
			if (GetComponentInChildren<Collider2D>() is Collider2D col)
				col.enabled = true;
		}

		if(_mover != null)
		{
			_mover.OnOutofBounds -= HandleOutofBounds;
			_mover.OnOutofBounds += HandleOutofBounds;
		}
	}

	public virtual void OnDespawn()
	{
		enabled = false;
		StopAllCoroutines();
		if(_collider != null && _collider.enabled)
			_collider.enabled = false;

		if (_mover != null)
			_mover.OnOutofBounds -= HandleOutofBounds;
	}

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("Player")) return;

		Player player = col.GetComponentInParent<Player>();
		if (player)
		{
			DealDamage(player);
		}
	}

	private void HandleOutofBounds(ObjectMover mover)
		=> EnemyPoolingManager.Instance.Despawn(this);
}
