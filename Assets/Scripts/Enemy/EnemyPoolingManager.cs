using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyPoolingManager : MonoBehaviour
{
	public static EnemyPoolingManager Instance { get; private set; }

	//[SerializeField]
	//EnemyData enemyData;
	Dictionary<AssetReferenceGameObject, EnemyPool> pools;

	private void Awake()
	{
		Instance = this;
		pools = new Dictionary<AssetReferenceGameObject, EnemyPool>();
		//AddressablesInitManager.OnInitialized += (enemyData, _) => DataInitialize(enemyData.entries);
	}

	private void HandleOutofBounds(ObjectMover mover)
	{
		mover.OnOutofBounds -= HandleOutofBounds;
		Despawn(mover.GetComponent<BaseEnemy>());
	}

	public void DataInitialize(List<EnemyData.EnemyEntry> entries)
	{
		foreach (var entry in entries)
		{
			pools[entry.prefabRef] =
				new EnemyPool(entry.prefabRef, entry.poolSize, transform);
		}
	}

	public BaseEnemy Spawn(AssetReferenceGameObject prefabRef, Vector3 pos, Quaternion rot)
	{
		if(!pools.TryGetValue(prefabRef, out var pool))
		{
			Debug.LogError($"Pool not Found for {prefabRef.RuntimeKey}");
			return null;
		}
		var enemy = pool.Spawn(pos, rot);
		return enemy;
	}

	public void Despawn(BaseEnemy instance)
		=> pools[instance.PrefabRef].Despawn(instance);

	public void Init()
	{

	}
}