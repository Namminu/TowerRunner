using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyPoolingManager : MonoBehaviour, IInitializable
{
	public static EnemyPoolingManager Instance { get; private set; }

	[SerializeField]
	private EnemyData enemyData;

	// 매니저에서 핸들과 풀을 보관
	private Dictionary<AssetReferenceGameObject, EnemyPool> _pools;
	private Dictionary<AssetReferenceGameObject, AsyncOperationHandle<GameObject>> _handles;

	private void Awake()
	{
		Instance = this;

		_pools = new Dictionary<AssetReferenceGameObject, EnemyPool>();
		_handles = new Dictionary<AssetReferenceGameObject, AsyncOperationHandle<GameObject>>();
	}

	public void Init()
	{
		StartCoroutine(EnemyPoolingBoot());
	}

	private IEnumerator EnemyPoolingBoot()
	{
		yield return StartCoroutine(BootRoutine());
		// 준비 완료 시그널 전송
		GameBus.Publish(new SubsystemReady(SubsystemId.Enemy));
	}

	private IEnumerator BootRoutine()
	{
		if (enemyData == null)
		{
			Debug.LogWarning("[EnemyPoolingManager] EnemyData not assigned");
			GameBus.Publish(new SubsystemFailed(SubsystemId.Enemy, "EnemyData not assigned in Enemy Pooling Manager"));
			yield break;
		}

		foreach (var entry in enemyData.entries)
		{
			if (entry.prefabRef == null)
			{
				Debug.LogWarning($"[EnemyPoolingManager] prefabRef null for entry");
				continue;
			}

			var handle = entry.prefabRef.LoadAssetAsync<GameObject>();
			_handles[entry.prefabRef] = handle;
			yield return handle;

			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[EnemyPoolingManager] Failed to load prefab for {entry.prefabRef.RuntimeKey}: {handle.OperationException}");
				_handles.Remove(entry.prefabRef);
				continue;
			}

			AddressablesTracker.Track(handle, isPersistent: false);

			var prefabGO = handle.Result;
			var enemyComp = prefabGO.GetComponent<BaseEnemy>();
			if (enemyComp == null)
			{
				Debug.LogError($"[EnemyPoolingManager] Prefab for {entry.prefabRef.RuntimeKey} has no BaseEnemy component");
				if (handle.IsValid()) Addressables.Release(handle);
				_handles.Remove(entry.prefabRef);
				continue;
			}

			var pool = new EnemyPool(enemyComp, Mathf.Max(1, entry.poolSize), transform);
			_pools[entry.prefabRef] = pool;
		}

		// 모든 풀 준비 대기 (타임아웃)
		float timeout = 5f;
		float t0 = Time.realtimeSinceStartup;
		while (true)
		{
			bool allReady = true;
			foreach (var kv in _pools)
			{
				if (!kv.Value.IsReady)
				{
					allReady = false;
					break;
				}
			}
			if (allReady) break;
			if (Time.realtimeSinceStartup - t0 > timeout)
			{
				Debug.LogError("[EnemyPoolingManager] Pool initialization timed out");
				GameBus.Publish(new SubsystemFailed(SubsystemId.Enemy, "Enemy Pool init timeout"));
				yield break;
			}
			yield return null;
		}

		// 풀 준비 완료 : EnemyManager에 데이터 바인딩 호출
		if (EnemyManager.Instance != null)
		{
			EnemyManager.Instance.SetData(enemyData);
		}
		else
		{
			Debug.LogWarning("[EnemyPoolingManager] EnemyManager.Instance is null when pools ready");
		}
	}

	public BaseEnemy Spawn(AssetReferenceGameObject prefabRef, Vector3 pos, Quaternion rot)
	{
		if (!_pools.TryGetValue(prefabRef, out var pool))
		{
			Debug.LogError($"Pool not Found for {prefabRef.RuntimeKey}");
			return null;
		}
		var enemy = pool.Spawn(pos, rot);
		if (enemy != null)
			enemy.PrefabRef = prefabRef;
		return enemy;
	}

	public void Despawn(BaseEnemy instance)
	{
		if (instance == null) return;
		if (!_pools.TryGetValue(instance.PrefabRef, out var pool))
		{
			Debug.LogWarning("[EnemyPoolingManager] No pool found for Despawn");
			return;
		}
		pool.Despawn(instance);
	}

	private void OnDestroy()
	{
		// Addressables 핸들 해제
		if (_handles != null)
		{
			foreach (var kv in _handles)
			{
				if (kv.Value.IsValid()) Addressables.Release(kv.Value);
			}
			_handles.Clear();
		}
		_pools?.Clear();

		if (Instance == this) Instance = null;
	}
}