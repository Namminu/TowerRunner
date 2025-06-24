using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPoolable
{
	/// <summary>
	/// 활성화 직후 초기화
	/// </summary>
	void OnSpawn();

	/// <summary>
	/// 비활성화 직후 해제
	/// </summary>
	void OnDespawn();
}

public class ObjectPool<T> where T : BaseEnemy, IPoolable
{
	private T prefab;
	private Queue<T> pool = new Queue<T>();
	private Transform parent;

	public ObjectPool(T prefab, int initialSize, Transform parent = null)
	{
		this.prefab = prefab;
		this.parent = parent;
		for (int i = 0; i < initialSize; i++)
			pool.Enqueue(CreateNew());
	}

	private T CreateNew()
	{
		var inst = GameObject.Instantiate(prefab, parent);
		inst.Prefab = prefab;
		inst.gameObject.SetActive(false);
		return inst;
	}

	public T Spawn(Vector3 pos, Quaternion rot)
	{
		if (pool.Count == 0) pool.Enqueue(CreateNew());
		var item = pool.Dequeue();
		item.transform.SetPositionAndRotation(pos, rot);
		item.OnSpawn();
		item.gameObject.SetActive(true);
		return item;
	}

	public void Despawn(T item)
	{
		item.OnDespawn();
		item.gameObject.SetActive(false);
		pool.Enqueue(item);
	}
}

public class EnemyPoolingManager : MonoBehaviour
{
	public static EnemyPoolingManager Instance { get; private set; }

	[Serializable]
	struct PoolInfo
	{
		public BaseEnemy prefab;
		public int initialSize;
	}

	[SerializeField]
	PoolInfo[] pools;

	private Dictionary<BaseEnemy, ObjectPool<BaseEnemy>> lookup;

	private void Awake()
	{
		Instance = this;
		lookup = new Dictionary<BaseEnemy, ObjectPool<BaseEnemy>>();
		foreach(var pi in pools)
		{
			lookup[pi.prefab] =
				new ObjectPool<BaseEnemy>(pi.prefab, pi.initialSize, transform);
		}
	}

	private void HandleOutofBounds(EnemyMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		Despawn(mover.GetComponent<BaseEnemy>());
	}


	public BaseEnemy Spawn(BaseEnemy prefab, Vector3 pos, Quaternion rot)
	{
		var enemy = lookup[prefab].Spawn(pos, rot);
		var mover = enemy.GetComponent<EnemyMover>();
		mover.OnOutofBounds += HandleOutofBounds;
		return enemy;
	}

	public void Despawn(BaseEnemy instance)
		=> lookup[instance.Prefab].Despawn(instance);

}