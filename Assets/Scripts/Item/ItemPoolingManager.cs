using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
		foreach (var data in itemDB.allItems)
		{
			var prefab = data.pickupPrefab;
			itemPools[data] = new ObjectPool<ItemPickup>(prefab, initialSize : 10, transform);
		}
	}

	public ItemPickup Spawn(ItemData data, Vector3 pos)
	{
		return itemPools[data].Spawn(pos, Quaternion.identity);
	}

	public void Despawn(ItemPickup pickUp)
	{
		itemPools[pickUp.Data].Despawn(pickUp);
	}
}