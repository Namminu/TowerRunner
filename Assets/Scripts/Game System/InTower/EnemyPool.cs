using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyPool
{
	readonly AssetReferenceGameObject prefabRef;
	readonly Transform parent;
	readonly int initialSize;

	private ObjectPool<BaseEnemy> innerPool;
	private bool isReady = false;

	public EnemyPool(AssetReferenceGameObject prefabRef, 
		int initialSize, Transform parent = null)
	{
		this.prefabRef = prefabRef;
		this.initialSize = initialSize;
		this.parent = parent;

		prefabRef.LoadAssetAsync<GameObject>().Completed += OnPrefabLoaded;
	}

	private void OnPrefabLoaded(AsyncOperationHandle<GameObject> handle)
	{
		var prefabGo = handle.Result;
		var prefab = prefabGo.GetComponent<BaseEnemy>();
		innerPool = new ObjectPool<BaseEnemy>(prefab, initialSize, parent);
		isReady = true;
	}

	public BaseEnemy Spawn(Vector3 pos, Quaternion rot)
	{
		if(!isReady)
		{
			Debug.LogWarning($"Pool for {prefabRef.RuntimeKey} Not Ready");
		}

		var enemy = innerPool.Spawn(pos, rot);
		enemy.PrefabRef = prefabRef;
		return enemy;
	}

	public void Despawn(BaseEnemy inst)
	{
		if (!isReady) return;
		innerPool.Despawn(inst);
	}
}