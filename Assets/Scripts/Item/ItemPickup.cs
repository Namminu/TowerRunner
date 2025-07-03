using UnityEngine;

[Tooltip("Component Class to Specific Item Prefabs")]
public class ItemPickup : MonoBehaviour, IPoolable
{
	[SerializeField]
	public ItemData Data { get;}

	private Collider2D col;
	private SpriteRenderer sprite;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		sprite = GetComponent<SpriteRenderer>();
		col.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("PLAYER"))
		{
			Data.Apply(other.GetComponent<Player>());
			/* Item Pooling Manager to Despawn this Item */
		}
	}

	public void OnSpawn()
	{
		gameObject.SetActive(true);
		col.enabled = true;
		sprite.enabled = true;
	}

	public void OnDespawn()
	{
		gameObject.SetActive(false);
		col.enabled = false;
		sprite.enabled = false;
	}
}
