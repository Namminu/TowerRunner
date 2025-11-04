using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ProjectilePoolingManager : MonoBehaviour, IInitializable
{
	public static ProjectilePoolingManager Instance { get; private set; }

	[SerializeField]
	private ProjectileDatabase projectileDB;

	private Dictionary<ProjectileType, ObjectPool<BaseProjectile>> _pools;
	private Dictionary<ProjectileType, AsyncOperationHandle<GameObject>> _handles;

	private void Awake()
	{
		Instance = this;

		_pools = new Dictionary<ProjectileType, ObjectPool<BaseProjectile>>();
		_handles = new Dictionary<ProjectileType, AsyncOperationHandle<GameObject>>();
	}

	public void Init()
	{
		StartCoroutine(ProjectileBoot());
	}

	private IEnumerator ProjectileBoot()
	{
		yield return StartCoroutine(BootRoutine());
		// 준비 완료 시그널 전송
		GameBus.Publish(new SubsystemReady(SubsystemId.Projectile));
	}

	private IEnumerator BootRoutine()
	{
		if (projectileDB == null)
		{
			Debug.LogWarning("[ProjectilePoolingManager] ProjectileDatabase not assigned");
			GameBus.Publish(new SubsystemFailed(SubsystemId.Projectile, "ProjectileDatabase not assigned in Projectile Pooling Manager"));
			yield break;
		}

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
	public BaseProjectile[] Spawn(ProjectileType type, Transform[] shotPoint, float damage)
	{
		List<BaseProjectile> spawnedProjectiles = new List<BaseProjectile>();
		foreach (Transform tr in shotPoint)
		{
			if (!_pools.TryGetValue(type, out var pool))
			{
				Debug.LogWarning($"[ProjectilePoolingManager] Pool not ready or not found for {type}");
				return null;
			}

			var inst = pool.Spawn(tr.position, tr.rotation);
			if (inst == null) return null;

			// 인스턴스 타입 기록
			inst.ProjectileType = type;

			// 초기화 + 활성화
			inst.InitializeProjectile(tr, damage);
			// 안전 호출 보장
			inst.OnSpawn();

			spawnedProjectiles.Add(inst);
		}
		return spawnedProjectiles.ToArray();
	}

	/// <summary>
	/// Pool로 반환
	/// </summary>
	public void Despawn(BaseProjectile proj)
	{
		if (proj == null) return;

		var type = proj.ProjectileType;
		if (!_pools.TryGetValue(type, out var pool))
		{
			Debug.LogWarning($"[ProjectilePoolingManager] No pool found for type {type} on Despawn; destroying as fallback");
			return;
		}

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
