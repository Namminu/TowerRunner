using UnityEngine;

[Tooltip("Component Class to Specific Item Prefabs")]
public class ItemPickup : MonoBehaviour, IPoolable
{
	[SerializeField]
	public ItemData Data { get; }

	private Collider2D _col;
	private SpriteRenderer _sprite;
	private ObjectMover _mover;

	private void Awake()
	{
		_col = GetComponent<Collider2D>();
		_col.isTrigger = true;
		_sprite = GetComponent<SpriteRenderer>();
		_mover = GetComponent<ObjectMover>();
		if (_mover == null)
			_mover = gameObject.AddComponent<ObjectMover>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("PLAYER"))
		{
			Data.Apply(other.GetComponent<Player>());
			ItemPoolingManager.Instance.Despawn(this);
		}
	}

	public void OnSpawn()
	{
		gameObject.SetActive(true);
		_col.enabled = true;
		_sprite.enabled = true;

		if (_mover != null)
		{
			_mover.OnOutofBounds -= HandleOutOfBound;
			_mover.OnOutofBounds += HandleOutOfBound;
		}
	}

	public void OnDespawn()
	{
		if (_mover != null)
			_mover.OnOutofBounds -= HandleOutOfBound;

		_col.enabled = false;
		_sprite.enabled = false;
		gameObject.SetActive(false);
	}

	private void HandleOutOfBound(ObjectMover mover)
		=> ItemPoolingManager.Instance.Despawn(this);
}
