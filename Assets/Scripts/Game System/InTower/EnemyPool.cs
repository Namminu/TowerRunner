using UnityEngine;

/// <summary>
/// 간단한 풀 래퍼: prefab(BaseEnemy)을 받아 ObjectPool<BaseEnemy>를 생성/관리.
/// Addressables 핸들 관리는 매니저(EnemyPoolingManager)가 담당합니다.
/// </summary>
public class EnemyPool
{
	private readonly Transform parent;
	private readonly int initialSize;

	private ObjectPool<BaseEnemy> innerPool;

	public EnemyPool(BaseEnemy prefab, int initialSize, Transform parent = null)
	{
		this.initialSize = initialSize;
		this.parent = parent;

		if (prefab == null)
		{
			Debug.LogError("[EnemyPool] prefab is null");
			return;
		}

		innerPool = new ObjectPool<BaseEnemy>(prefab, Mathf.Max(1, initialSize), parent);
	}

	public bool IsReady => innerPool != null;

	public BaseEnemy Spawn(Vector3 pos, Quaternion rot)
	{
		if (!IsReady)
		{
			Debug.LogWarning("[EnemyPool] Spawn requested but pool not ready");
			return null;
		}
		return innerPool.Spawn(pos, rot);
	}

	public void Despawn(BaseEnemy inst)
	{
		if (!IsReady || inst == null) return;
		innerPool.Despawn(inst);
	}
}