using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[Tooltip("Component Class to Specific Item Prefabs")]
public class ItemPickup : MonoBehaviour, IPoolable
{
	[SerializeField]
	private ItemData data;
	public ItemData Data => data;

	private Collider2D _col;
	private SpriteRenderer _sprite;
	private ObjectMover _mover;

	/* coin item */
	private Transform magnetTarget;
	private float pullSpeed = 10f;

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
		//if(other.CompareTag("Player"))

		if(other.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
		{
			Player player = other.GetComponentInParent<Player>();
			if(player)
			{
				data.Apply(player);
				ItemPoolingManager.Instance.Despawn(this);
			}
		}
	}

	private void Update()
	{
		if(magnetTarget != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, 
				magnetTarget.position, pullSpeed * Time.deltaTime);
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
		if(magnetTarget != null)
			magnetTarget = null;

		if (_mover != null)
			_mover.OnOutofBounds -= HandleOutOfBound;

		_col.enabled = false;
		_sprite.enabled = false;
		gameObject.SetActive(false);
	}

	private void HandleOutOfBound(ObjectMover mover)
		=> ItemPoolingManager.Instance.Despawn(this);

	public void StartMagnet(Transform target)
		=> magnetTarget = target;

}
