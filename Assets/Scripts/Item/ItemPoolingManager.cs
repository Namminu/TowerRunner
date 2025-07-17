using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPoolingManager : MonoBehaviour
{
	public static ItemPoolingManager Instance { get; private set; }

	[SerializeField]
	private ItemDatabase itemDB;
	
	private Dictionary<ItemData, ObjectPool<ItemPickup>> itemPools;

	private void Awake()
	{
		Instance = this;
		itemPools = new Dictionary<ItemData, ObjectPool<ItemPickup>>();
	}

	private void Start()
	{
		foreach (var data in itemDB.entries)
		{
			var prefab = data.data.pickupPrefab;
			itemPools[data.data] = new ObjectPool<ItemPickup>(prefab, data.poolSize, transform);
		}
	}

	private void HandleOutofBounds(ObjectMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		Despawn(mover.GetComponent<ItemPickup>());
	}

	public ItemPickup Spawn(ItemData data, Vector3 pos)
	{
		var item = itemPools[data].Spawn(pos, Quaternion.identity);
		var mover = data.GetComponent<ObjectMover>();
		mover.OnOutofBounds += HandleOutofBounds;
		return item;
	}

	public void Despawn(ItemPickup pickUp)
	{
		itemPools[pickUp.Data].Despawn(pickUp);
	}

	public void Init()
	{
		
	}
}