using UnityEngine;

public class Arrow : MonoBehaviour, IDamageable, IDamageDealer, IProjectile
{
	public float ArrowDamage { get; private set; }

	private void Start()
	{
		if (!TryGetComponent(out ObjectMover mover))
			mover = gameObject.AddComponent<ObjectMover>();
		mover.OnOutofBounds += HandleOutOfBound;
	}

	public void InitializeProjectile(Transform tr, float damage)
	{
		ArrowDamage = damage;
		transform.SetPositionAndRotation(tr.position, tr.rotation);
	}

	public void DealDamage(IDamageable target)
	{
		target.TakeDamage(ArrowDamage);
	}

	public void TakeDamage(float amount)
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("PLAYER")) return;

		if (col.TryGetComponent<Player>(out var player))
		{
			DealDamage(player);
			Destroy(gameObject);
		}
	}

	private void HandleOutOfBound(ObjectMover mover)
	{
		Destroy(mover.gameObject);
	}
}
