using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemPoolingManager : MonoBehaviour
{
	public static ItemPoolingManager Instance { get; private set; }

	[SerializeField]
	private ItemDatabase itemDB;

	private Dictionary<ItemData, AsyncOperationHandle<GameObject>> prefabHandle;
	private Dictionary<ItemData, ObjectPool<ItemPickup>> itemPools;

	private void Awake()
	{
		Instance = this;

		prefabHandle = new Dictionary<ItemData, AsyncOperationHandle<GameObject>>();
		itemPools = new Dictionary<ItemData, ObjectPool<ItemPickup>>();
	}

	private IEnumerator Start()
	{
		foreach (var entry in itemDB.entries)
		{
			var data = entry.data;
			var size = entry.poolSize;

			var handle = data.pickupPrefab.LoadAssetAsync<GameObject>();
			prefabHandle[data] = handle;
			yield return handle;

			if(handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[ItemPooling] Failed to Load Prefab for {data.itemName} : {handle.OperationException}");
				continue;
			}

			var prefabGO = handle.Result;
			var prefabPickUp = prefabGO.GetComponent<ItemPickup>();
			if(prefabPickUp == null)
			{
				Debug.LogError($"[ItemPooling] Prefab for {data.itemName} has no ItemPickup Component");
				continue;
			}

			itemPools[data] = new ObjectPool<ItemPickup>(prefabPickUp, size, transform);
		}
	}

	private void HandleOutofBounds(ObjectMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		var pickup = mover.GetComponent<ItemPickup>();
		if (pickup != null)
			Despawn(pickup);
	}

	public ItemPickup Spawn(ItemData data, Vector3 pos)
	{
		if(!itemPools.TryGetValue(data, out var pool))
		{
			Debug.LogWarning($"[ItemPooling] Pool not Ready for {data?.itemName}");
			return null;
		}
		var item = pool.Spawn(pos, Quaternion.identity);

		var mover = item.GetComponent<ObjectMover>();
		if (mover != null)
			mover.OnOutofBounds += HandleOutofBounds;

		return item;
	}

	public void Despawn(ItemPickup pickUp)
	{
		//itemPools[pickUp.Data].Despawn(pickUp);

		if (pickUp == null) return;
		if(!itemPools.TryGetValue(pickUp.Data, out var pool))
		{
			Debug.LogWarning("[ItemPooling] No Pool found for Despawn");
			return;
		}
		pool.Despawn(pickUp);
	}

	public void Init()
	{
		
	}

	private void OnDestroy()
	{
		foreach (var kv in prefabHandle)
			if (kv.Value.IsValid()) Addressables.Release(kv.Value);
		prefabHandle.Clear();
	}
}