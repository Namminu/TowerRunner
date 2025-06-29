using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

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

public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
	protected T prefab;
	private Queue<T> pool = new Queue<T>();
	private Transform parent;

	public ObjectPool(T prefab, int initialSize, Transform parent = null)
	{
		this.prefab = prefab;
		this.parent = parent;
		for (int i = 0; i < initialSize; i++)
			pool.Enqueue(CreateNew());
	}

	protected virtual T CreateNew()
	{
		var inst = GameObject.Instantiate(prefab, parent);
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

	public T Spawn()
	{
		if (pool.Count == 0) pool.Enqueue(CreateNew());
		var item = pool.Dequeue();
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

public class EnemyPool : ObjectPool<BaseEnemy>
{
	public EnemyPool(BaseEnemy prefab, int size, Transform parent = null)
		: base(prefab, size, parent) { }

	protected override BaseEnemy CreateNew()
	{
		var inst = base.CreateNew();
		inst.Prefab = prefab;
		return inst;
	}
}