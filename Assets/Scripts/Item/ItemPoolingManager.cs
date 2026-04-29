using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemPoolingManager : MonoBehaviour, IInitializable
{
	public static ItemPoolingManager Instance { get; private set; }

	[SerializeField]
	private ItemDatabase itemDB;

	private Dictionary<ItemData, AsyncOperationHandle<GameObject>> _handles;
	private Dictionary<ItemData, ObjectPool<ItemPickup>> _pools;

	private void Awake()
	{
		Instance = this;

		_handles = new Dictionary<ItemData, AsyncOperationHandle<GameObject>>();
		_pools = new Dictionary<ItemData, ObjectPool<ItemPickup>>();
	}

	public void Init()
	{
		StartCoroutine(ItemPoolingBoot());
	}

	private IEnumerator ItemPoolingBoot()
	{
		yield return StartCoroutine(BootRoutine());
		// 준비 완료 시그널 전송
		GameBus.Publish(new SubsystemReady(SubsystemId.Item));
	}

	private IEnumerator BootRoutine()
	{
		if(itemDB == null)
		{
			Debug.LogWarning("[EnemyPoolingManager] ItemDatabase not assigned");
			GameBus.Publish(new SubsystemFailed(SubsystemId.Item, "ItemDatabase not assigned in Item Pooling Manager"));
			yield break;
		}

		foreach (var entry in itemDB.entries)
		{
			var data = entry.data;
			var size = entry.poolSize;

			var handle = data.pickupPrefab.LoadAssetAsync<GameObject>();
			_handles[data] = handle;
			yield return handle;

			if(handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[ItemPooling] Failed to Load Prefab for {data.itemName} : {handle.OperationException}");
				continue;
			}

			AddressablesTracker.Track(handle, isPersistent: false);

			var prefabGO = handle.Result;
			var prefabPickUp = prefabGO.GetComponent<ItemPickup>();
			if(prefabPickUp == null)
			{
				Debug.LogError($"[ItemPooling] Prefab for {data.itemName} has no ItemPickup Component");
				if (handle.IsValid()) Addressables.Release(handle);
				_handles.Remove(entry.data);
				continue;
			}

			_pools[data] = new ObjectPool<ItemPickup>(prefabPickUp, Mathf.Max(1, size), transform);
		}

		// 풀 준비 완료
		if (ItemManager.Instance != null)
		{
			ItemManager.Instance.SetData(itemDB);
		}
		else
		{
			Debug.LogWarning("[ItemPoolingManager] ItemManager.Instance is null when pools ready");
		}
	}

	public ItemPickup Spawn(ItemData data, Vector3 pos)
	{
		if(!_pools.TryGetValue(data, out var pool))
		{
			Debug.LogWarning($"[ItemPooling] Pool not Ready for {data?.itemName}");
			return null;
		}
		var item = pool.Spawn(pos, Quaternion.identity);
		return item;
	}

	public void Despawn(ItemPickup pickUp)
	{
		if (pickUp == null) return;
		if(!_pools.TryGetValue(pickUp.Data, out var pool))
		{
			Debug.LogWarning("[ItemPooling] No Pool found for Despawn");
			return;
		}
		pool.Despawn(pickUp);
	}

	private void OnDestroy()
	{
		if(_handles != null)
		{
			foreach (var kv in _handles)
				if (kv.Value.IsValid()) Addressables.Release(kv.Value);
			_handles.Clear();
		}
		_pools?.Clear();

		if(Instance == this) Instance = null;
	}
}