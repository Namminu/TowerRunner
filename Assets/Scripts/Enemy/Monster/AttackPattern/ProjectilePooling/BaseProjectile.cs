using Unity.VisualScripting;
using UnityEngine;

public class BaseProjectile : MonoBehaviour, IDamageable, IDamageDealer, IProjectile, IPoolable
{
	public float ProjectileDamage { get; private set; }

	private ObjectMover mover;

	public ProjectileType ProjectileType { get; set; }

	private void Awake()
	{
		mover = GetComponent<ObjectMover>();
		if(mover == null)
			gameObject.AddComponent<ObjectMover>();
	}

	private void HandleOutOfBound(ObjectMover mover)
		=> Despawn();

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("Player")) return;

		//if (col.TryGetComponent<Player>(out var player))
		//{
		//	DealDamage(player);
		//	Despawn();
		//}

		Player player = col.GetComponentInParent<Player>();
		if(player)
		{
			DealDamage(player);
			Despawn();
		}
	}

	public void InitializeProjectile(Transform tr, float damage)
	{
		ProjectileDamage = damage;
		transform.SetPositionAndRotation(tr.position, tr.rotation);
	}

	public void DealDamage(IDamageable target)
	{
		target.TakeDamage(ProjectileDamage);
	}

	public void OnDespawn()
	{
		enabled = false;
		mover.OnOutofBounds -= HandleOutOfBound;
		StopAllCoroutines();

		//if (GetComponent<Collider2D>() is Collider2D col)
		//	col.enabled = false;
		
		gameObject.SetActive(false);
	}

	public void OnSpawn()
	{
		gameObject.SetActive(true);
		enabled = true;
		mover.OnOutofBounds += HandleOutOfBound;

		if (GetComponent<Collider2D>() is Collider2D col)
			col.enabled = true;
	}

	public void TakeDamage(float amount)
	{
		Despawn();
	}

	private void Despawn()
		=> ProjectilePoolingManager.Instance.Despawn(this);
}
