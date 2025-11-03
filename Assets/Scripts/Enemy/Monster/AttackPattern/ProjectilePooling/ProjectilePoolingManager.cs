using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ProjectilePoolingManager : MonoBehaviour
{
	public static ProjectilePoolingManager Instance { get; private set; }

	[SerializeField]
	private ProjectileDatabase projectileDB;

	// 내부 풀/핸들링 매핑
	private readonly Dictionary<ProjectileType, ObjectPool<BaseProjectile>> _pools = new();
	private readonly Dictionary<ProjectileType, AsyncOperationHandle<GameObject>> _handles= new();


	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start()
	{
		if (projectileDB == null)
		{
			Debug.LogWarning("[ProjectilePoolingManager] ProjectileDatabase not assigned");
			yield break;
		}

		// 비동기 로드 및 풀 생성
		foreach (var entry in projectileDB.entries)
		{
			if (entry.prefabRef == null)
			{
				Debug.LogWarning($"[ProjectilePoolingManager] prefabRef null for {entry.type}");
				continue;
			}

			var handle = entry.prefabRef.LoadAssetAsync<GameObject>();
			_handles[entry.type] = handle;
			yield return handle;

			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[ProjectilePoolingManager] Failed to load prefab for {entry.type}: {handle.OperationException}");
				_handles.Remove(entry.type);
				continue;
			}

			var prefabGO = handle.Result;
			var projComp = prefabGO.GetComponent<BaseProjectile>();
			if (projComp == null)
			{
				Debug.LogError($"[ProjectilePoolingManager] Prefab for {entry.type} has no BaseProjectile component");
				Addressables.Release(handle);
				_handles.Remove(entry.type);
				continue;
			}

			var pool = new ObjectPool<BaseProjectile>(projComp, Mathf.Max(1, entry.poolSize), transform);
			_pools[entry.type] = pool;
		}
	}

	/// <summary>
	/// id(type)로 spawn 하고 내부에서 InitializeProjectile, OnSpawn 까지 처리
	/// </summary>
	public BaseProjectile Spawn(ProjectileType type, Transform shotPoint, float damage)
	{
		if (!_pools.TryGetValue(type, out var pool))
		{
			Debug.LogWarning($"[ProjectilePoolingManager] Pool not ready or not found for {type}");
			return null;
		}

		var inst = pool.Spawn(shotPoint.position, shotPoint.rotation);
		if (inst == null) return null;

		// 인스턴스가 어떤 타입에서 왔는지 기록
		inst.ProjectileType = type;

		// 초기화와 활성화
		inst.InitializeProjectile(shotPoint, damage);
		// OnSpawn은 ObjectPool.Spawn에서 이미 호출하지만, 안전을 위해 호출 보장
		inst.OnSpawn();

		return inst;
	}

	/// <summary>
	/// Pool로 반환. BaseProjectile.OnDespawn()을 외부에서 직접 호출하지 말고 이 API를 사용하세요.
	/// </summary>
	public void Despawn(BaseProjectile proj)
	{
		if (proj == null) return;

		var type = proj.ProjectileType;
		if (!_pools.TryGetValue(type, out var pool))
		{
			Debug.LogWarning($"[ProjectilePoolingManager] No pool found for type {type} on Despawn; destroying as fallback");
			// 안전망: 파괴(풀 관리가 아니라면)
			Destroy(proj.gameObject);
			return;
		}

		// OnDespawn 처리와 큐 삽입은 ObjectPool.Despawn이 담당
		pool.Despawn(proj);
	}

	private void OnDestroy()
	{
		// Addressables 핸들 정리
		foreach (var kv in _handles)
		{
			if (kv.Value.IsValid()) Addressables.Release(kv.Value);
		}
		_handles.Clear();
		_pools.Clear();

		if (Instance == this) Instance = null;
	}
}
